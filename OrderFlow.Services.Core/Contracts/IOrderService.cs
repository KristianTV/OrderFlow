using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Services.Core.Contracts
{
    public interface IOrderService : IRepository
    {
        IQueryable<Order> GetAll();
        Task<Order?> GetOrderByIdAsync(Guid? orderId);
        Task<IEnumerable<Order>> GetAllByUserIdAsync(Guid? userId);
        Task<IEnumerable<Order>> GetAllByUserIdAndStatusAsync(Guid? userId, OrderStatus status);
        Task<IEnumerable<Order>> GetAllByStatusAsync(OrderStatus status);
        Task<bool> CancelOrderAsync(Guid? orderId, Guid? userId, bool save = true);
        Task<bool> ChangeOrderStatusAsync(Guid? orderId, string? status, bool save = true);
        Task<bool> ChangeStatusToCompletedAsync(Guid? orderID, bool save = true);
        Task<bool> ReactivateOrderAsync(Guid orderId, bool save = true);
        Task<bool> CreateOrderAsync(CreateOrderViewModel createOrderViewModel, Guid? userId, bool save = true);
        Task<bool> UpdateOrderAsync(CreateOrderViewModel createOrder, Guid? orderId, Guid? userId, bool save = true);
        Task CompleteOrderAsync(Guid orderID, ICourseOrderService truckOrderService, ITruckService _truckService, bool save = true);
    }
}
