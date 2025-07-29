using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Services.Core.Contracts
{
    public interface IOrderService : IRepository
    {
        Task<bool> CancelOrderAsync(Guid guid, string? v);
        Task<bool> CreateOrderAsync(CreateOrderViewModel createOrderViewModel, string? userId);
    }
}
