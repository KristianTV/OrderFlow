using Microsoft.IdentityModel.Tokens;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels;

namespace OrderFlow.Services.Core
{
    public class OrderService : BaseRepository, IOrderService
    {
        public OrderService(OrderFlowDbContext _context) : base(_context)
        {
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
