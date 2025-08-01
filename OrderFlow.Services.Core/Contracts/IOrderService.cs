﻿using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Order;

namespace OrderFlow.Services.Core.Contracts
{
    public interface IOrderService : IRepository
    {
        Task<bool> CancelOrderAsync(Guid? orderId, Guid? userId);
        Task<bool> ChangeOrderStatusAsync(Guid orderId, string? status);
        Task<bool> ChangeStatusToCompletedAsync(Guid orderID);
        Task<bool> CreateOrderAsync(CreateOrderViewModel createOrderViewModel, Guid? userId);
        Task<bool> ReactivateOrderAsync(Guid orderId);
        Task<bool> UpdateOrderAsync(CreateOrderViewModel createOrder, Guid? orderId, Guid? userId);
    }
}
