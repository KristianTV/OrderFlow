using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OrderFlow.Data.Configuration;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;

namespace OrderFlow.Data
{
    public class OrderFlowDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        private static readonly Guid AdminRoleId = Guid.Parse("b91c3da6-5bde-478d-bf2c-257b2fc9567c");
        private static readonly Guid SpeditorRoleId = Guid.Parse("206f9e28-522f-4d34-8bfd-ec37b093a331");
        private static readonly Guid DriverRoleId = Guid.Parse("d349122c-ed4a-43ca-b939-b3a9ff9fa0fb");
        private static readonly Guid UserRoleId = Guid.Parse("782b39c1-d0e7-4f18-8a2b-1077a438c048");

        private static readonly Guid AdminUserId = Guid.Parse("922b605c-25eb-4ded-b2a7-7966b38b8685");
        private static readonly Guid SpeditorUserId = Guid.Parse("051eafea-deaa-4fbe-b442-25b7be18fa8e");
        private static readonly Guid DriverUserId = Guid.Parse("cca021da-da51-4bd7-b968-8294cf006dfd");
        private static readonly Guid RegularUserId = Guid.Parse("862c30cd-17ee-4557-8222-c926bebf0a66");

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
                    ConcurrencyStamp = null,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole<Guid>
                {
                    Id = SpeditorRoleId,
                    ConcurrencyStamp = null,
                    Name = "Speditor",
                    NormalizedName = "SPEDITOR"
                },
                new IdentityRole<Guid>
                {
                    Id = DriverRoleId,
                    ConcurrencyStamp = null,
                    Name = "Driver",
                    NormalizedName = "DRIVER"
                },
                new IdentityRole<Guid>
                {
                    Id = UserRoleId,
                    ConcurrencyStamp = null,
                    Name = "User",
                    NormalizedName = "USER"
                }
            );
        }

        protected void SeedUsers(ModelBuilder builder)
        {
            var adminUser = new ApplicationUser
            {
                Id = AdminUserId,
                UserName = "Admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@gmail.com",
                NormalizedEmail = "ADMIN@GMAIL.COM",
                EmailConfirmed = true,
                ConcurrencyStamp = "9d3d41db-b3d8-4e59-8a85-6c125ab6c6cf",
                SecurityStamp = "66545229-a1b8-408d-afd9-ce9ce3edc048",
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                AccountType = AccountType.Personal,
                PasswordHash = "AQAAAAIAAYagAAAAEHVYKQgsgG5GJLVN5GvdLD1kD2Vkx58fLxzi9jdmMH3wIbH4UsvW9+t3E4bfAXoVTQ=="
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
                ConcurrencyStamp = "df2b7fe5-0158-4cdf-8eec-c1f660590919",
                SecurityStamp = "77560b6c-d614-451f-8b10-aaa59d40108e",
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                AccountType = AccountType.Personal,
                PasswordHash = "AQAAAAIAAYagAAAAEL9Ps09TT5d/dtrX80yK6H/KdC0Kh9Rr+tFJtAvD2MrWfoSrwAMJ/jJqYYa1GXJwlQ=="
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
                ConcurrencyStamp = "1a645d87-508f-4081-89ca-e62d845060c8",
                SecurityStamp = "6aeebefd-d08b-4ade-8a87-91ebeba082f1",
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                AccountType = AccountType.Personal,
                PasswordHash = "AQAAAAIAAYagAAAAELc41rT3Vb6VnyQjCoTyd5iD6QA3wKspGtImItv6xC3ZdXKAOvBWj6ahbUytwMjo1A=="
            };
            builder.Entity<ApplicationUser>().HasData(driverUser);

            var regularUser = new ApplicationUser
            {
                Id = RegularUserId,
                UserName = "User",
                NormalizedUserName = "USER",
                Email = "user@gmail.com",
                NormalizedEmail = "USER@GMAIL.COM",
                EmailConfirmed = true,
                ConcurrencyStamp = "6edb66da-be0a-4a50-baf5-ca406103a6b4",
                SecurityStamp = "24472bcf-990a-49bf-b2e5-343897fe1c68",
                PhoneNumber = "1234567890",
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                AccountType = AccountType.Personal,
                PasswordHash = "AQAAAAIAAYagAAAAEC+0D9crqWZFGHRJK+X/2FSbmoWYbBzxZ6t0shCx3/3ALqljPRuCq9yw0U4gkWuh8A=="
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
