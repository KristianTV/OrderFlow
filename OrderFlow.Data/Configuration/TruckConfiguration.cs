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
                   .IsRequired()
                   .HasMaxLength(ValidationConstants.Truck.StatusMaxLength);

            builder.HasOne(t => t.Driver)
                   .WithMany()
                   .HasForeignKey(t => t.DriverID);

            builder.HasMany(t => t.TruckOrders)
                   .WithOne(to => to.Truck)
                   .HasForeignKey(to => to.TruckID);

            builder.Property(t => t.isDeleted)
                   .HasDefaultValue(false);

            builder.HasQueryFilter(t => !t.isDeleted);
        }
    }
}
