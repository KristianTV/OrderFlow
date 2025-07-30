using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        public OrderService(OrderFlowDbContext _context) : base(_context)
        {
        }

        public async Task<bool> CancelOrderAsync(Guid? orderId, Guid? userId)
        {
            if(orderId == null || userId == null || orderId == Guid.Empty || userId == Guid.Empty)
                return false;

            Order? order = await this.DbSet<Order>().Where(x => x.OrderID.Equals(orderId) && x.UserID.Equals(userId)).SingleOrDefaultAsync();

            if (order != null)
            {
                if (order.isCanceled)
                             return true;

                order.isCanceled = true;
                order.Status = OrderStatus.Cancelled;
                await this.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ChangeStatusToCompletedAsync(Guid orderID)
        {
            if (orderID == Guid.Empty)
                return false;

            Order? order = await this.DbSet<Order>().Where(x => x.OrderID.Equals(orderID))
                                                    .SingleOrDefaultAsync();

            if (order != null)
            {
                if (order.Status == OrderStatus.Completed)
                    return true;

                order.Status = OrderStatus.Completed;
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
            };

            await this.AddAsync(newOrder);

            int changes = await this.SaveChangesAsync();

            return changes > 0;
        }

        public async Task<bool> UpdateOrderAsync(CreateOrderViewModel createOrder, Guid? orderId, Guid? userId)
        {
           if (createOrder == null || orderId == Guid.Empty || userId == Guid.Empty)
                return false;

            Order? order = await this.DbSet<Order>().Where(x => x.OrderID.Equals(orderId) && 
                                                                x.UserID.Equals(userId))
                                                    .SingleOrDefaultAsync();

            if (order != null)
            {
                order.DeliveryAddress = createOrder.DeliveryAddress;
                order.PickupAddress = createOrder.PickupAddress;
                order.DeliveryInstructions = createOrder.DeliveryInstructions;
                int changes = await this.SaveChangesAsync();
                return changes > 0;
            }
            return false;
        }
    }
}
