using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Configuration;
using OrderFlow.Data.Models;

namespace OrderFlow.Data
{
    public class OrderFlowDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        private readonly Guid AdminRoleId = Guid.NewGuid();
        private readonly Guid SpeditorRoleId = Guid.NewGuid();
        private readonly Guid DriverRoleId = Guid.NewGuid();
        private readonly Guid UserRoleId = Guid.NewGuid();

        private readonly Guid AdminUserId = Guid.NewGuid();
        private readonly Guid SpeditorUserId = Guid.NewGuid();
        private readonly Guid DriverUserId = Guid.NewGuid();
        private readonly Guid RegularUserId = Guid.NewGuid();

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

            SeedRoles(builder);
            SeedUsers(builder);
            SeedUserRoles(builder);
        }

        protected void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid>
                {
                    Id = AdminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole<Guid>
                {
                    Id = SpeditorRoleId,
                    Name = "Speditor",
                    NormalizedName = "SPEDITOR"
                },
                new IdentityRole<Guid>
                {
                    Id = DriverRoleId,
                    Name = "Driver",
                    NormalizedName = "DRIVER"
                },
                new IdentityRole<Guid>
                {
                    Id = UserRoleId,
                    Name = "User",
                    NormalizedName = "USER"
                }
            );
        }

        protected void SeedUsers(ModelBuilder builder)
        {
            var passwordHasher = new PasswordHasher<ApplicationUser>();

            var adminUser = new ApplicationUser
            {
                Id = AdminUserId,
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = passwordHasher.HashPassword(null, "Admin123!"),
                SecurityStamp = Guid.NewGuid().ToString(),
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            };
            builder.Entity<ApplicationUser>().HasData(adminUser);

            var speditorUser = new ApplicationUser
            {
                Id = SpeditorUserId,
                UserName = "Speditor",
                NormalizedUserName = "SPEDITOR",
                Email = "speditor@gmail.com",
                NormalizedEmail = "SPEDITOR@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = passwordHasher.HashPassword(null, "Speditor123!"),
                SecurityStamp = Guid.NewGuid().ToString(),
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            };
            builder.Entity<ApplicationUser>().HasData(speditorUser);

            var driverUser = new ApplicationUser
            {
                Id = DriverUserId,
                UserName = "Driver",
                NormalizedUserName = "DRIVER",
                Email = "driver@gmail.com",
                NormalizedEmail = "DRIVER@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = passwordHasher.HashPassword(null, "Driver123!"),
                SecurityStamp = Guid.NewGuid().ToString(),
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            };
            builder.Entity<ApplicationUser>().HasData(driverUser);

            var regularUser = new ApplicationUser // Renamed 'userUser' to 'regularUser'
            {
                Id = RegularUserId,
                UserName = "User",
                NormalizedUserName = "USER",
                Email = "user@gmail.com",
                NormalizedEmail = "USER@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = passwordHasher.HashPassword(null, "User123!"),
                SecurityStamp = Guid.NewGuid().ToString(),
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0
            };
            builder.Entity<ApplicationUser>().HasData(regularUser);
        }

        protected void SeedUserRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid> { UserId = AdminUserId, RoleId = AdminRoleId },
                new IdentityUserRole<Guid> { UserId = SpeditorUserId, RoleId = SpeditorRoleId },
                new IdentityUserRole<Guid> { UserId = DriverUserId, RoleId = DriverRoleId },
                new IdentityUserRole<Guid> { UserId = RegularUserId, RoleId = UserRoleId }
            );
        }
    }
}