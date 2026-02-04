using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
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
        }
    }
}
