using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderFlow.Data;
using OrderFlow.Data.Models;
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

        public async Task<bool> CancelOrderAsync(Guid guid, string? v)
        {
            Order? order = await this.DbSet<Order>().Where(x => x.OrderID.Equals(guid)).SingleOrDefaultAsync();

            if (order != null)
            {
                if (order.isCanceled)
                {
                    return true;
                }

                order.isCanceled = true;
                order.Status = "Canceled";
                await this.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> CreateOrderAsync(CreateOrderViewModel createOrderViewModel, string? userId)
        {
            if (createOrderViewModel == null || userId.IsNullOrEmpty())
                return false;

            Order newOrder = new Order
            {
                UserID = Guid.Parse(userId),
                OrderDate = DateTime.UtcNow,
                DeliveryAddress = createOrderViewModel.DeliveryAddress,
                PickupAddress = createOrderViewModel.PickupAddress,
                DeliveryInstructions = createOrderViewModel.DeliveryInstructions,
                Status = "Pending",
            };

            await this.AddAsync(newOrder);

            int changes = await this.SaveChangesAsync();

            if (changes <= 0)
                return false;

            return true;
        }
    }
}
