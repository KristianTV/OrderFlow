using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels;

namespace OrderFlow.Services.Core.Contracts
{
    public interface IOrderService : IRepository
    {
        Task<bool> CreateOrderAsync(CreateOrderViewModel createOrderViewModel, string? userId);
    }
}
