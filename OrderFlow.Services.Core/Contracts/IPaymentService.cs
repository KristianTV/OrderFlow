using OrderFlow.Data.Models;
using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Payment;

namespace OrderFlow.Services.Core.Contracts
{
    public interface IPaymentService : IRepository
    {
        IQueryable<Payment> GetAll();
        Task<bool> CreatePaymentAsync(CreatePaymentViewModel createPayment, Guid orderId);
        Task<bool> DeletePaymentAsync(Guid paymentId);
        Task<bool> UpdatePaymentAsync(Guid paymentId, CreatePaymentViewModel createPayment);
    }
}
