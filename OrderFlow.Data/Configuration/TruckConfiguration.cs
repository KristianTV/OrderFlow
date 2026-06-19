using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.GCommon;

namespace OrderFlow.Data.Configuration
{
    internal class TruckConfiguration : IEntityTypeConfiguration<Truck>
    {
        public void Configure(EntityTypeBuilder<Truck> builder)
        {
            builder.HasKey(t => t.TruckID);

            builder.Property(t => t.LicensePlate)
                   .IsRequired()
                   .HasMaxLength(ValidationConstants.Truck.LicensePlateMaxLength);

            builder.Property(t => t.Capacity)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(t => t.Status)
                   .IsRequired();

            builder.HasOne(t => t.Driver)
                   .WithMany()
                   .HasForeignKey(t => t.DriverID);

            builder.Property(t => t.IsDeleted)
                   .HasDefaultValue(false);

            builder.HasMany(t => t.Notifications)
                   .WithOne(n => n.Truck)
                   .HasForeignKey(n => n.TruckID)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(t => t.TruckCourses)
                   .WithOne(c => c.Truck)
                   .HasForeignKey(c => c.TruckID)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(t => t.TruckSpendings)
                   .WithOne(ts => ts.Truck)
                   .HasForeignKey(ts => ts.TruckID)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasQueryFilter(t => !t.IsDeleted);

            builder.HasData(SeedTrucks());
        }

        private List<Truck> SeedTrucks()
        {
            var drivers = new[] { SeedDataIds.DriverUserId, SeedDataIds.DriverTwoUserId };
            var trucks = new List<Truck>();

            for (var driverIndex = 0; driverIndex < drivers.Length; driverIndex++)
            {
                for (var truckIndex = 0; truckIndex < 3; truckIndex++)
                {
                    var seedIndex = (driverIndex * 3) + truckIndex;
                    var truckNumber = truckIndex + 1;
                    trucks.Add(new Truck
                    {
                        TruckID = SeedDataIds.TruckIds[seedIndex],
                        DriverID = drivers[driverIndex],
                        LicensePlate = $"CB" + truckIndex.ToString() + (truckIndex + 5).ToString() + (truckIndex + 3).ToString() + (truckIndex + 4).ToString() + "HK",
                        Capacity = 12 + (truckNumber * 3),
                        Status = truckNumber == 3 ? TruckStatus.UnderMaintenance : TruckStatus.Available,
                        IsDeleted = false
                    });
                }
            }
            return trucks;
        }
    }
}
