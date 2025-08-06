using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Models;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using OrderFlow.ViewModels.Payment;

namespace OrderFlow.Services.Core
{
    public class PaymentService : BaseRepository, IPaymentService
    {
        public PaymentService(OrderFlowDbContext _context) : base(_context)
        {
        }

        public IQueryable<Payment> GetAll()
        {
            return All<Payment>().AsQueryable();
        }

        public async Task<bool> CreatePaymentAsync(CreatePaymentViewModel createPayment, Guid orderId)
        {
            await AddAsync(new Payment
            {

                Amount = createPayment.Amount,
                PaymentDescription = createPayment.PaymentDescription,
                OrderID = orderId,
                PaymentDate = DateTime.UtcNow
            });

            return await SaveChangesAsync() > 0;
        }

        public async Task<bool> DeletePaymentAsync(Guid paymentId)
        {
            if (paymentId == Guid.Empty)
            {
                throw new ArgumentException("Payment ID cannot be empty.", nameof(paymentId));
            }

            var payment = All<Payment>().SingleOrDefault(p => p.Id == paymentId);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");
            }

            Delete(payment);

            return await SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdatePaymentAsync(Guid paymentId, CreatePaymentViewModel createPayment)
        {
            if (paymentId == Guid.Empty)
            {
                throw new ArgumentException("Payment ID cannot be empty.", nameof(paymentId));
            }
            if (createPayment == null)
            {
                throw new ArgumentNullException(nameof(createPayment), "CreatePaymentViewModel cannot be null.");
            }

            var payment = await All<Payment>().SingleOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");
            }

            payment.Amount = createPayment.Amount;
            payment.PaymentDescription = createPayment.PaymentDescription;

            return await SaveChangesAsync() > 0;

        }
    }
}
