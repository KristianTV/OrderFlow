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

        public TruckOrderService(OrderFlowDbContext _context, IOrderService orderService, INotificationService notificationService) : base(_context)
        {
            _orderService = orderService;
            _notificationService = notificationService;
        }

        public IQueryable<TruckOrder> GetAll()
        {
            return this.All<TruckOrder>().AsQueryable();
        }

        public async Task<int> AssignOrdersToTruckAsync(IEnumerable<OrderViewModel> assignOrders, Guid truckID)
        {
            bool added = false;
            if (assignOrders == null || assignOrders == null || !assignOrders.Any())
            {
                throw new ArgumentNullException(nameof(assignOrders));
            }

            foreach (var order in assignOrders)
            {

                if (this.GetAll().Any(to => to.OrderID.Equals(order.OrderID) &&
                                            to.TruckID.Equals(truckID) &&
                                            to.DeliverAddress.Equals(order.DeliveryAddress)))
                {
                    continue;
                }

                await this.AddAsync(new TruckOrder
                {
                    OrderID = order.OrderID,
                    TruckID = truckID,
                    AssignedDate = DateTime.UtcNow,
                    DeliverAddress = order.DeliveryAddress,
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
                    ReceiverId = this.GetAll()
                                     .Include(to => to.Truck)
                                     .ThenInclude(t => t.Driver)
                                     .Where(to => to.TruckID.Equals(truckID))
                                     .OrderByDescending(to => to.AssignedDate)
                                     .Select(to => to.Truck.Driver.Id)
                                     .FirstOrDefault()
                });
            }

            return await this.SaveChangesAsync();
        }

        public async Task RemoveOrderFromTruckAsync(Guid truckID, Guid orderID)
        {
            if (truckID == Guid.Empty || orderID == Guid.Empty)
            {
                throw new ArgumentException("Truck ID and Order ID cannot be empty.");
            }

            var truckOrder = await this.GetAll()
                                       .Where(o => o.OrderID.Equals(orderID) &&
                                                   o.TruckID.Equals(truckID)).SingleOrDefaultAsync();

            if (truckOrder != null)
            {
                this.Delete(truckOrder);

                await _orderService.ChangeOrderStatusAsync(orderID, OrderStatus.InProgress.ToString());

                await _notificationService.AddAsync(new Notification
                {
                    Title = "Order Remove",
                    Message = $"Order {orderID} have been remove from truck {truckID}.",
                    OrderId = orderID,
                    CreatedAt = DateTime.UtcNow,
                    ReceiverId = this.GetAll()
                                     .Include(to => to.Truck)
                                     .ThenInclude(t => t.Driver)
                                     .Where(to => to.TruckID.Equals(truckID))
                                     .OrderByDescending(to => to.AssignedDate)
                                     .Select(to => to.Truck.Driver.Id)
                                     .FirstOrDefault()
                });

                await this.SaveChangesAsync();
            }
        }
    }
}
