using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
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

        public async Task CreatePaymentAsync(CreatePaymentViewModel createPayment, Guid orderId)
        {
            await  this.AddAsync(new Data.Models.Payment
                {

                    Amount = createPayment.Amount,
                    PaymentDescription = createPayment.PaymentDescription,
                    OrderID = orderId,
                    PaymentDate = DateTime.UtcNow
            });

            await this.SaveChangesAsync();
        }

        public async Task DeletePaymentAsync(Guid paymentId)
        {
           if (paymentId == Guid.Empty)
            {
                throw new ArgumentException("Payment ID cannot be empty.", nameof(paymentId));
            }

            var payment = this.All<Data.Models.Payment>().SingleOrDefault(p => p.Id == paymentId);

            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");
            }
            this.Delete(payment);

            await this.SaveChangesAsync();
        }

        public async Task UpdatePaymentAsync(Guid paymentId, CreatePaymentViewModel createPayment)
        {
            if (paymentId == Guid.Empty)
            {
                throw new ArgumentException("Payment ID cannot be empty.", nameof(paymentId));
            }
            if (createPayment == null)
            {
                throw new ArgumentNullException(nameof(createPayment), "CreatePaymentViewModel cannot be null.");
            }

            var payment = await this.All<Data.Models.Payment>().SingleOrDefaultAsync(p => p.Id == paymentId);
            
            if (payment == null)
            {
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");
            }

            payment.Amount = createPayment.Amount;
            payment.PaymentDescription = createPayment.PaymentDescription;

            await this.SaveChangesAsync();

        }
    }
}
