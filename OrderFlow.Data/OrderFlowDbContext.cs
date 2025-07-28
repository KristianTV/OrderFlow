using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace OrderFlow.Data
{
    public class OrderFlowDbContext : IdentityDbContext
    {
        public OrderFlowDbContext(DbContextOptions<OrderFlowDbContext> options)
          : base(options)
        {
        }
    }
}
