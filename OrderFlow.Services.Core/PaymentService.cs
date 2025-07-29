using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;

namespace OrderFlow.Services.Core
{
    public class PaymentService : BaseRepository, IPaymentService
    {
        public PaymentService(DbContext _context) : base(_context)
        {
        }
    }
}
