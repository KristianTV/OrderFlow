using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;

namespace OrderFlow.Services.Core
{
    public class TruckOrderService : BaseRepository, ITruckOrderService
    {
        public TruckOrderService(DbContext _context) : base(_context)
        {
        }
    }
}
