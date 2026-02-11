using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.Services.Core.Extensions;
using OrderFlow.ViewModels.Order;
using Order = OrderFlow.Data.Models.Order;

namespace OrderFlow.Services.Core
{
    public class OrderService : BaseRepository, IOrderService
    {
        private readonly INotificationService _notificationService;

        public OrderService(OrderFlowDbContext _context, INotificationService notificationService) : base(_context)
        {
            _notificationService = notificationService;
        }

        public async Task<bool> CancelOrderAsync(Guid? orderId, Guid? userId, bool save = true)
        {
            if (orderId == null || userId == null || orderId == Guid.Empty || userId == Guid.Empty)
                return false;

            Order? order = await this.GetAll()
                                     .Include(o => o.CourseOrders)
                                     .Where(x => x.OrderID.Equals(orderId) &&
                                                 x.UserID.Equals(userId))
                                     .SingleOrDefaultAsync();

            if (order != null)
            {
                if (order.IsCanceled)
                    return true;

                order.IsCanceled = true;
                order.Status = OrderStatus.Cancelled;

                //Todo remove orders from courses

                await _notificationService.SendSystemNotificationAsync(order.ToNotification($"Order {orderId} has been Cancelled",
                                                                                            $"Order {orderId} has been Cancelled"),
                                                                                            false);

                await this.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ChangeOrderStatusAsync(Guid? orderId, string? status, bool save = true)
        {
            if (orderId == null || string.IsNullOrEmpty(status) || orderId == Guid.Empty)
                return false;

            if (!Enum.TryParse<OrderStatus>(status, true, out var orderStatus))
            {
                return false;
            }

            Order? order = await this.GetTrackingOrderByIdAsync(orderId);

            if (order != null)
            {
                if (order.Status.Equals(orderStatus))
                    return true;

                order.Status = orderStatus;

                await _notificationService.SendSystemNotificationAsync(order.ToNotification($"Order status changed to {orderStatus}",
                                                                                            $"Order status changed to {orderStatus}"),
                                                                                           false);

                if (save)
                {
                    await this.SaveChangesAsync();
                }

                return true;
            }

            return false;
        }

        public async Task<bool> ChangeStatusToCompletedAsync(Guid? orderID, bool save = true)
        {
            if (orderID == null || orderID == Guid.Empty)
                return false;

            Order? order = await this.GetTrackingOrderByIdAsync(orderID);

            if (order != null)
            {
                if (order.Status == OrderStatus.Completed)
                    return true;

                order.Status = OrderStatus.Completed;
                order.DeliveryDate = DateTime.UtcNow;

                await _notificationService.SendSystemNotificationAsync(order.ToNotification($"Order status changed to {OrderStatus.Completed}",
                                                                                            $"Order status changed to {OrderStatus.Completed}"),
                                                                                           false);
                if (save)
                {
                    await this.SaveChangesAsync();
                }

                return true;
            }

            return false;
        }

        public IQueryable<Order> GetAll()
        {
            return this.All<Order>().AsQueryable();
        }

        public async Task<Order?> GetOrderByIdAsync(Guid? orderId)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .Where(x => x.OrderID.Equals(orderId))
                             .SingleOrDefaultAsync();
        }

        private async Task<Order?> GetTrackingOrderByIdAsync(Guid? orderId)
        {
            return await this.GetAll()
                             .Where(x => x.OrderID.Equals(orderId))
                             .SingleOrDefaultAsync();
        }
        private async Task<Order?> GetTrackingOrderByIdAndUserIdAsync(Guid? orderId, Guid? userId)
        {
            return await this.GetAll()
                             .Where(x => x.OrderID.Equals(orderId) &&
                                         x.UserID.Equals(userId))
                             .SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Order>> GetAllByUserIdAsync(Guid? userId)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .Where(x => x.UserID.Equals(userId))
                             .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllByUserIdAndStatusAsync(Guid? userId, OrderStatus status)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .Where(x => x.UserID.Equals(userId) && x.Status.Equals(status))
                             .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllByStatusAsync(OrderStatus status)
        {
            return await this.GetAll()
                             .AsNoTracking()
                             .Where(x => x.Status.Equals(status))
                             .ToListAsync();
        }

        public async Task<bool> ReactivateOrderAsync(Guid orderId, bool save = true)
        {
            if (orderId == null || orderId == Guid.Empty)
                return false;

            Order? order = await this.GetTrackingOrderByIdAsync(orderId);

            if (order != null)
            {
                if (!order.IsCanceled)
                    return true;

                order.IsCanceled = false;
                order.Status = OrderStatus.Pending;

                await _notificationService.SendSystemNotificationAsync(order.ToNotification($"Order {orderId} has been Reactivated",
                                                                                            $"Order {orderId} has been Reactivated"),
                                                                                            false);
                if (save)
                {
                    await this.SaveChangesAsync();
                }
                return true;
            }

            return false;
        }

        public async Task<bool> CreateOrderAsync(CreateOrderViewModel createOrderViewModel, Guid? userId, bool save = true)
        {
            if (createOrderViewModel == null || !userId.HasValue)
                return false;

            Order newOrder = new Order
            {
                UserID = (Guid)userId,
                OrderDate = DateTime.UtcNow,
                DeliveryAddress = createOrderViewModel.DeliveryAddress,
                PickupAddress = createOrderViewModel.PickupAddress,
                DeliveryInstructions = createOrderViewModel.DeliveryInstructions,
                Status = OrderStatus.Pending,
                LoadCapacity = createOrderViewModel.LoadCapacity,
            };

            await this.AddAsync(newOrder);

            if (save)
            {
                int changes = await this.SaveChangesAsync();
                return changes > 0;
            }

            return true;
        }

        public async Task<bool> UpdateOrderAsync(CreateOrderViewModel createOrder, Guid? orderId, Guid? userId, bool save = true)
        {
            if (orderId == null || userId == null || createOrder == null || orderId == Guid.Empty || userId == Guid.Empty)
                return false;

            Order? order = await this.GetTrackingOrderByIdAndUserIdAsync(orderId, userId);

            if (order != null)
            {
                if (order.Status.Equals(OrderStatus.Completed))
                    return false;

                order.DeliveryAddress = createOrder.DeliveryAddress;
                order.PickupAddress = createOrder.PickupAddress;
                order.DeliveryInstructions = createOrder.DeliveryInstructions;
                order.LoadCapacity = createOrder.LoadCapacity;

                if (save)
                {
                    int changes = await this.SaveChangesAsync();
                    return changes > 0;
                }

                return true;
            }
            return false;
        }

        public async Task CompleteOrderAsync(Guid orderID, ICourseOrderService _courseOrderService, ITruckService _truckService, bool save = true)
        {
            if (orderID == Guid.Empty)
                throw new ArgumentNullException(nameof(orderID));

            Order? order = await this.GetAll()
                                     .Include(o => o.CourseOrders)
                                     .ThenInclude(co => co.TruckCourse)
                                     .Where(x => x.OrderID.Equals(orderID))
                                     .SingleOrDefaultAsync();

            //if (order != null)
            //{
            //    if (order.CourseOrders == null || !order.CourseOrders.Any(to => to..Equals(CourseStatus.Assigned)))
            //        throw new ArgumentNullException(nameof(order.CourseOrders));

            //    Guid truckID = order.OrderTrucks.Where(to => to.Status.Equals(CourseStatus.Assigned))
            //                                                             .Select(to => to.TruckID)
            //                                                             .FirstOrDefault();

            //    if (order.DeliveryAddress.Equals(
            //                order.OrderTrucks.Where(to => to.Status.Equals(CourseStatus.Assigned))
            //                                 .Select(to => to.DeliverAddress).FirstOrDefault()))
            //    {
            //        await this.ChangeStatusToCompletedAsync(orderID);
            //        await _truckOrderService.CompleteTruckOrderAsync(orderID);
            //    }
            //    else
            //    {
            //        await this.ChangeOrderStatusAsync(orderID, OrderStatus.OnHold.ToString());
            //        await _truckOrderService.CompleteTruckOrderAsync(orderID);
            //    }

            //    if (!_truckService.GetAll()
            //                      .AsNoTracking()
            //                      .Include(t => t.TruckOrders)
            //                      .Any(t => t.TruckID.Equals(truckID) &&
            //                                t.TruckOrders.Any(to => to.Status.Equals(CourseStatus.Assigned))))
            //    {
            //        if (!_truckService.GetTruckStatus(truckID).Equals(TruckStatus.Available.ToString()))
            //        {
            //            _truckService.ChangeTruckStatus(truckID, TruckStatus.Available.ToString());
            //            await this.SaveChangesAsync();
            //        }
            //    }
            //}
            //else
            //{
            //    throw new ArgumentNullException(nameof(order));
            //}

        }

    }
}
