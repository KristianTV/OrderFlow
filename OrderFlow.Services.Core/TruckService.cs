using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Repository;
using OrderFlow.Services.Core.Contracts;

namespace OrderFlow.Services.Core
{
    public class TruckService : BaseRepository, ITruckService
    {
        public TruckService(DbContext _context) : base(_context)
        {
        }
    }
}
