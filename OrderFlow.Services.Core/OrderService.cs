using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Services.Core
{
    public class OrderService : BaseRepository, IOrderService
    {
        public OrderService(DbContext _context) : base(_context)
        {
        }
    }
}
