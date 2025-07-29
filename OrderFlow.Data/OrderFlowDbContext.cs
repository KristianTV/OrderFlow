using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Configuration;
using OrderFlow.Data.Models;

namespace OrderFlow.Data
{
    public class OrderFlowDbContext : IdentityDbContext<ApplicationUser,IdentityRole<Guid>,Guid>
    {
        public OrderFlowDbContext(DbContextOptions<OrderFlowDbContext> options)
          : base(options)
        {
        }

        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<TruckOrder> TrucksOrders { get; set; } = null!;
        public virtual DbSet<Truck> Trucks { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(OrderConfiguration).Assembly);
        }
    }
}
