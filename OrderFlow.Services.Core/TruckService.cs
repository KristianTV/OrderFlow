using Microsoft.EntityFrameworkCore;
using OrderFlow.Data;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;

namespace OrderFlow.Services.Core
{
    public class TruckService : BaseRepository, ITruckService
    {
        public TruckService(OrderFlowDbContext _context) : base(_context)
        {
        }
    }
}
