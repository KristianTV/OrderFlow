using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;

namespace OrderFlow.Services.Core
{
    public class OrderService : BaseRepository, IOrderService
    {
        public OrderService(OrderFlowDbContext _context) : base(_context)
        {
        }
    }
}
