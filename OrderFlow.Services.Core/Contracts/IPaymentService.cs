using OrderFlow.Data.Models;
using OrderFlow.Data.Repository.Contracts;
using OrderFlow.ViewModels.Payment;

namespace OrderFlow.Services.Core.Contracts
{
    public interface IPaymentService : IRepository
    {
        IQueryable<Payment> GetAll();
        Task<bool> CreatePaymentAsync(CreatePaymentViewModel createPayment, Guid orderId);
        Task<CardPaymentViewModel?> GetCardPaymentAsync(Guid orderId, Guid userId);
        Task<bool> PayOrderByCardAsync(CardPaymentViewModel cardPayment, Guid userId);
        Task<bool> MarkPaymentAsCashAsync(Guid paymentId);
        Task<bool> MarkOrderPaymentsAsCashAsync(Guid orderId);
        Task<bool> DeletePaymentAsync(Guid paymentId);
        Task<bool> UpdatePaymentAsync(Guid paymentId, CreatePaymentViewModel createPayment);
        Task<Guid?> GetOrderIdByPaymentIdAsync(Guid paymentId);
        Task CreateCoursePayoutAsync(IEnumerable<Guid> orderIds, TruckCourse course, bool save);
    }
}
