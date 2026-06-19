using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Configuration;
using OrderFlow.Data.Models;

namespace OrderFlow.Data
{
    public class OrderFlowDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public OrderFlowDbContext(DbContextOptions<OrderFlowDbContext> options)
          : base(options)
        {
        }

        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<Truck> Trucks { get; set; } = null!;
        public virtual DbSet<CourseOrder> CoursesOrders { get; set; } = null!;
        public virtual DbSet<TruckCourse> TrucksCourses { get; set; } = null!;
        public virtual DbSet<TruckSpending> TrucksSpendings { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<PersonalProfile> PersonalProfiles { get; set; } = null!;
        public virtual DbSet<CompanyProfile> CompanyProfiles { get; set; } = null!;
        public virtual DbSet<Message> Messages { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(OrderConfiguration).Assembly);
        }
    }
}
