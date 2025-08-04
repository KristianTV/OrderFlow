using OrderFlow.Data.Models;
using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Services.Core.Contracts
{
    public interface IOrderService : IRepository
    {
        IQueryable<Order> GetAll();
        Task<bool> CancelOrderAsync(Guid? orderId, Guid? userId);
        Task<bool> ChangeOrderStatusAsync(Guid orderId, string? status);
        Task<bool> ChangeStatusToCompletedAsync(Guid orderID);
        Task<bool> ReactivateOrderAsync(Guid orderId);
        Task<bool> CreateOrderAsync(CreateOrderViewModel createOrderViewModel, Guid? userId);
        Task<bool> CreateOrderAsync(AdminCreateOrderViewModel createOrderViewModel);
        Task<bool> UpdateOrderAsync(CreateOrderViewModel createOrder, Guid? orderId, Guid? userId);
        Task<bool> UpdateOrderAsync(AdminCreateOrderViewModel createOrder, Guid? orderId);
        Task CompleteOrderAsync(Guid orderID, ITruckOrderService truckOrderService, ITruckService _truckService);
    }
}
