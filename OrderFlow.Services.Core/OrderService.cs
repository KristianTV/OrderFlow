using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Services.Core
{
    public class OrderService : BaseRepository, IOrderService
    {
        private readonly INotificationService _notificationService;

        public OrderService(OrderFlowDbContext _context, INotificationService notificationService) : base(_context)
        {
            _notificationService = notificationService;
        }

        public async Task<bool> CancelOrderAsync(Guid? orderId, Guid? userId)
        {
            if (orderId == null || userId == null || orderId == Guid.Empty || userId == Guid.Empty)
                return false;

            Order? order = await this.GetAll()
                                     .Where(x => x.OrderID.Equals(orderId) &&
                                                 x.UserID.Equals(userId)).SingleOrDefaultAsync();

            if (order != null)
            {
                if (order.isCanceled)
                    return true;

                order.isCanceled = true;
                order.Status = OrderStatus.Cancelled;

                await _notificationService.AddAsync(new Notification
                {
                    Title = $"Order {orderId} has been Cancelled",
                    OrderId = order.OrderID,
                    Message = $"Order {orderId} has been Cancelled",
                    CreatedAt = DateTime.UtcNow,
                    ReceiverId = order.UserID
                });

                await this.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ChangeOrderStatusAsync(Guid orderId, string? status)
        {
            if (orderId == null || string.IsNullOrEmpty(status) || orderId == Guid.Empty)
                return false;

            if (!Enum.TryParse<OrderStatus>(status, true, out var result))
            {
                return false;
            }

            Order? order = await this.GetAll()
                                     .Where(x => x.OrderID.Equals(orderId))
                                     .SingleOrDefaultAsync();

            if (order != null)
            {
                if (order.Status.Equals(result))
                    return true;

                order.Status = result;

                await _notificationService.AddAsync(new Notification
                {
                    Title = $"Order status changed to {result}",
                    OrderId = order.OrderID,
                    Message = $"Order status changed to {result}",
                    CreatedAt = DateTime.UtcNow,
                    ReceiverId = order.UserID
                });

                await this.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ChangeStatusToCompletedAsync(Guid orderID)
        {
            if (orderID == Guid.Empty)
                return false;

            Order? order = await this.GetAll().Where(x => x.OrderID.Equals(orderID))
                                              .SingleOrDefaultAsync();

            if (order != null)
            {
                if (order.Status == OrderStatus.Completed)
                    return true;

                order.Status = OrderStatus.Completed;
                order.DeliveryDate = DateTime.UtcNow;

                await _notificationService.AddAsync(new Notification
                                            {
                                                Title = $"Order status changed to {OrderStatus.Completed}",
                                                OrderId = order.OrderID,
                                                Message = $"Order status changed to {OrderStatus.Completed}",
                                                CreatedAt = DateTime.UtcNow,
                                                ReceiverId = order.UserID
                                            });

                await this.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public IQueryable<Order> GetAll()
        {
            return this.All<Order>().AsQueryable();
        }

        public async Task<bool> ReactivateOrderAsync(Guid orderId)
        {
            if (orderId == null || orderId == Guid.Empty)
                return false;

            Order? order = await this.GetAll()
                                     .Where(x => x.OrderID.Equals(orderId))
                                     .SingleOrDefaultAsync();

            if (order != null)
            {
                if (!order.isCanceled)
                    return true;

                order.isCanceled = false;
                order.Status = OrderStatus.Pending;
                await this.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> CreateOrderAsync(CreateOrderViewModel createOrderViewModel, Guid? userId)
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

            int changes = await this.SaveChangesAsync();

            return changes > 0;
        }

        public async Task<bool> CreateOrderAsync(AdminCreateOrderViewModel createOrderViewModel)
        {
            if (createOrderViewModel == null)
                return false;

            Order newOrder = new Order
            {
                UserID = createOrderViewModel.UsersId,
                OrderDate = DateTime.UtcNow,
                DeliveryAddress = createOrderViewModel.DeliveryAddress,
                PickupAddress = createOrderViewModel.PickupAddress,
                DeliveryInstructions = createOrderViewModel.DeliveryInstructions,
                Status = OrderStatus.Pending,
                LoadCapacity = createOrderViewModel.LoadCapacity,
            };

            await this.AddAsync(newOrder);

            int changes = await this.SaveChangesAsync();

            return changes > 0;
        }

        public async Task<bool> UpdateOrderAsync(CreateOrderViewModel createOrder, Guid? orderId, Guid? userId)
        {
            if (createOrder == null || orderId == Guid.Empty || userId == Guid.Empty)
                return false;

            Order? order = await this.GetAll().Where(x => x.OrderID.Equals(orderId) &&
                                                                x.UserID.Equals(userId))
                                                    .SingleOrDefaultAsync();

            if (order != null)
            {
                if (order.Status.Equals(OrderStatus.Completed))
                    return false;

                order.DeliveryAddress = createOrder.DeliveryAddress;
                order.PickupAddress = createOrder.PickupAddress;
                order.DeliveryInstructions = createOrder.DeliveryInstructions;
                order.LoadCapacity = createOrder.LoadCapacity;
                int changes = await this.SaveChangesAsync();
                return changes > 0;
            }
            return false;
        }

        public async Task<bool> UpdateOrderAsync(AdminCreateOrderViewModel createOrder, Guid? orderId)
        {
            if (createOrder == null || orderId == Guid.Empty)
                return false;

            Order? order = await this.GetAll().Where(x => x.OrderID.Equals(orderId) &&
                                                          x.UserID.Equals(createOrder.UsersId))
                                                    .SingleOrDefaultAsync();

            if (order != null)
            {
                if (order.Status.Equals(OrderStatus.Completed))
                    return false;

                order.UserID = createOrder.UsersId;
                order.DeliveryAddress = createOrder.DeliveryAddress;
                order.PickupAddress = createOrder.PickupAddress;
                order.DeliveryInstructions = createOrder.DeliveryInstructions;
                order.LoadCapacity = createOrder.LoadCapacity;
                int changes = await this.SaveChangesAsync();
                return changes > 0;
            }
            return false;
        }
    }
}
