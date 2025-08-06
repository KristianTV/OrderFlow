using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Truck;

namespace OrderFlow.Services.Core
{
    public class TruckOrderService : BaseRepository, ITruckOrderService
    {
        private readonly IOrderService _orderService;
        private readonly INotificationService _notificationService;
        private readonly ITruckService _truckService;

        public TruckOrderService(OrderFlowDbContext _context, IOrderService orderService, INotificationService notificationService, ITruckService truckService) : base(_context)
        {
            _orderService = orderService;
            _notificationService = notificationService;
            _truckService = truckService;
        }

        public IQueryable<TruckOrder> GetAll()
        {
            return All<TruckOrder>().AsQueryable();
        }

        public async Task<int> AssignOrdersToTruckAsync(IEnumerable<OrderViewModel> assignOrders, Guid truckID)
        {
            bool added = false;

            if (assignOrders == null || !assignOrders.Any())
            {
                throw new ArgumentNullException(nameof(assignOrders));
            }

            int capacity = await _truckService.GetAll()
                                              .AsNoTracking()
                                              .Where(t => t.TruckID.Equals(truckID))
                                              .Select(t => t.Capacity)
                                              .SingleOrDefaultAsync();

            int loadedCapacity = await GetAll()
                                           .AsNoTracking()
                                           .Where(to => to.TruckID.Equals(truckID) &&
                                                        to.Status.Equals(TruckOrderStatus.Assigned))
                                           .Select(to => to.Order.LoadCapacity)
                                           .SumAsync();

            capacity -= loadedCapacity;

            foreach (var order in assignOrders)
            {

                capacity -= await _orderService.GetAll()
                                               .AsNoTracking()
                                               .Where(o => o.OrderID.Equals(order.OrderID))
                                               .Select(o => o.LoadCapacity)
                                               .SingleOrDefaultAsync();

                if (capacity < 0)
                {
                    throw new InvalidOperationException("Truck capacity is full.");
                }

                if (GetAll().Any(to => to.OrderID.Equals(order.OrderID) &&
                                            to.TruckID.Equals(truckID) &&
                                            to.DeliverAddress.Equals(order.DeliveryAddress)))
                {
                    continue;
                }

                await AddAsync(new TruckOrder
                {
                    OrderID = order.OrderID,
                    TruckID = truckID,
                    AssignedDate = DateTime.UtcNow,
                    DeliverAddress = order.DeliveryAddress,
                    Status = TruckOrderStatus.Assigned,
                });

                await _orderService.ChangeOrderStatusAsync(order.OrderID, OrderStatus.InProgress.ToString());
                added = true;
            }

            if (added)
            {
                await _notificationService.AddAsync(new Notification
                {
                    Title = "Orders Assigned",
                    Message = $"Orders have been assigned to truck {truckID}.",
                    CreatedAt = DateTime.UtcNow,
                    ReceiverId = GetAll()
                                     .AsNoTracking()
                                     .Include(to => to.Truck)
                                     .ThenInclude(t => t.Driver)
                                     .Where(to => to.TruckID.Equals(truckID) &&
                                                  to.Status.Equals(TruckOrderStatus.Assigned))
                                     .OrderByDescending(to => to.AssignedDate)
                                     .Select(to => to.Truck.Driver.Id)
                                     .FirstOrDefault(),
                    TruckId = truckID,
                });

                if (!_truckService.GetTruckStatus(truckID).Equals(TruckStatus.Unavailable.ToString()))
                {
                    _truckService.ChangeTruckStatus(truckID, TruckStatus.Unavailable.ToString());
                }
            }

            return await SaveChangesAsync();
        }

        public async Task<bool> RemoveOrderFromTruckAsync(Guid truckID, Guid orderID)
        {
            if (truckID == Guid.Empty || orderID == Guid.Empty)
            {
                throw new ArgumentException("Truck ID and Order ID cannot be empty.");
            }

            var truckOrder = await GetAll()
                                       .Where(to => to.OrderID.Equals(orderID) &&
                                                    to.TruckID.Equals(truckID) &&
                                                    to.Status.Equals(TruckOrderStatus.Assigned)).SingleOrDefaultAsync();

            if (truckOrder != null)
            {
                await _orderService.ChangeOrderStatusAsync(orderID, OrderStatus.Pending.ToString());

                await _notificationService.AddAsync(new Notification
                {
                    Title = "Order Remove",
                    Message = $"Order {orderID} have been remove from truck {truckID}.",
                    OrderId = orderID,
                    CreatedAt = DateTime.UtcNow,
                    ReceiverId = GetAll()
                                     .AsNoTracking()
                                     .Include(to => to.Truck)
                                     .ThenInclude(t => t.Driver)
                                     .Where(to => to.TruckID.Equals(truckID) &&
                                                  to.Status.Equals(TruckOrderStatus.Assigned))
                                     .OrderByDescending(to => to.AssignedDate)
                                     .Select(to => to.Truck.Driver.Id)
                                     .FirstOrDefault(),
                    TruckId = truckID,
                });

                Delete(truckOrder);

                await SaveChangesAsync();

                if (!GetAll().Any(t => t.TruckID.Equals(truckID) && t.Status.Equals(TruckOrderStatus.Assigned)))
                {
                    if (!_truckService.GetTruckStatus(truckID).Equals(TruckStatus.Available.ToString()))
                    {
                        _truckService.ChangeTruckStatus(truckID, TruckStatus.Available.ToString());
                        await SaveChangesAsync();
                    }
                }

                return true;
            }
            return false;
        }

        public async Task CompleteTruckOrderAsync(Guid orderID)
        {
            TruckOrder? order = await GetAll()
                                          .Include(to => to.Truck)
                                          .ThenInclude(t => t.Driver)
                                          .Where(x => x.OrderID.Equals(orderID) &&
                                                      x.Status.Equals(TruckOrderStatus.Assigned))
                                          .SingleOrDefaultAsync();

            if (order != null)
            {
                if (order.Status == TruckOrderStatus.Delivered)
                    return;

                order.Status = TruckOrderStatus.Delivered;
                order.DeliveryDate = DateTime.UtcNow;

                if (order.Truck.DriverID == Guid.Empty)
                    return;

                await _notificationService.AddAsync(new Notification
                {
                    Title = $"Order {orderID} has been delivered",
                    OrderId = order.OrderID,
                    Message = $"Order {orderID} has been delivered",
                    CreatedAt = DateTime.UtcNow,
                    ReceiverId = order.Truck.DriverID,
                });

                await SaveChangesAsync();
            }
        }
    }
}
