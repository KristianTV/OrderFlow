using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;

namespace OrderFlow.Data.Configuration
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.AccountType)
                   .IsRequired();

            builder.HasOne(u => u.PersonalProfile)
                   .WithOne(p => p.User)
                   .HasForeignKey<PersonalProfile>(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(u => u.CompanyProfile)
                   .WithOne(c => c.User)
                   .HasForeignKey<CompanyProfile>(c => c.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(SeedUsers());
        }

        private List<ApplicationUser> SeedUsers()
        {
            return new List<ApplicationUser>
            {
                CreateUser(SeedDataIds.AdminUserId, "Admin", "admin@gmail.com", "1234567890", AccountType.Personal, "admin",
                    "AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1BZG1pbjA5PifeCHRqVmoLRpJeI0xoYRKND7a+WeS4Sp75s4JzsA=="), // Admin123!
                CreateUser(SeedDataIds.SpeditorUserId, "Speditor", "speditor@gmail.com", "1234567890", AccountType.Personal, "speditor",
                    "AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1TcGVkaXSSvU91xg3cFsOcphJ7ZWFCRJLpoDgFkVc6JF0woYnPow=="), // Speditor123!
                CreateUser(SeedDataIds.DriverUserId, "Driver", "driver@gmail.com", "1234567890", AccountType.Personal, "driver",
                    "AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1Ecml2ZXKMfnSIV9cEheURCTgDayjrZhIOatwnpr+eCOv9tIbz1Q=="), // Driver123!
                CreateUser(SeedDataIds.RegularUserId, "User", "user@gmail.com", "1234567890", AccountType.Personal, "user",
                    "AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1Vc2VyMDAiHf5dIXjh1QwjRFIRIMb1wl2FH5K0ZPkGmRa0p3RBJA=="), // User123!
                CreateUser(SeedDataIds.CompanyUserId, "CompanyUser", "company.user@gmail.com", "0888123456", AccountType.Company, "company-user",
                    "AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1Db21wYW6WF4dt7DOmRu3CVZWpcl9cjFmtsLRrJ7wv3OhfkgWqPg=="), // CompanyUser123!
                CreateUser(SeedDataIds.DriverTwoUserId, "DriverTwo", "driver.two@gmail.com", "0888654321", AccountType.Personal, "driver-two",
                    "AQAAAAIAAYagAAAAEE9yZGVyRmxvdy1Ecml2ZXKmM70g+ZLq3lK0EY9ly3Kx+5ZB6Yx85Ajv+WSevCIv5g==") // DriverTwo123!
            };
        }

        private static ApplicationUser CreateUser(
            Guid id,
            string userName,
            string email,
            string phoneNumber,
            AccountType accountType,
            string stampPrefix,
            string passwordHash)
            => new()
            {
                Id = id,
                UserName = userName,
                NormalizedUserName = userName.ToUpperInvariant(),
                Email = email,
                NormalizedEmail = email.ToUpperInvariant(),
                EmailConfirmed = true,
                PhoneNumber = phoneNumber,
                PhoneNumberConfirmed = true,
                TwoFactorEnabled = false,
                LockoutEnabled = true,
                AccessFailedCount = 0,
                AccountType = accountType,
                PasswordHash = passwordHash,
                SecurityStamp = $"{stampPrefix}-security-stamp",
                ConcurrencyStamp = $"{stampPrefix}-concurrency-stamp"
            };
    }
}
