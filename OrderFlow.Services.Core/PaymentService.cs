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
    }
}
