﻿using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Payment;

namespace OrderFlow.Services.Core.Contracts
{
    public interface IPaymentService : IRepository
    {
        Task CreatePaymentAsync(CreatePaymentViewModel createPayment, Guid orderId);
        Task DeletePaymentAsync(Guid paymentId);
        Task UpdatePaymentAsync(Guid paymentId, CreatePaymentViewModel createPayment);
    }
}
