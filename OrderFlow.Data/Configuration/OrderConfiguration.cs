using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.GCommon;

namespace OrderFlow.Data.Configuration
{
    internal class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.OrderID);

            builder.Property(o => o.OrderDate)
                   .IsRequired();

            builder.Property(o => o.DeliveryDate);

            builder.Property(o => o.DeliveryAddress)
                   .IsRequired()
                   .HasMaxLength(ValidationConstants.Order.DeliveryAddressMaxLength);

            builder.Property(o => o.PickupAddress)
                   .IsRequired()
                   .HasMaxLength(ValidationConstants.Order.PickupAddressMaxLength);

            builder.Property(o => o.DeliveryInstructions)
                   .HasMaxLength(ValidationConstants.Order.DeliveryInstructionsMaxLength);

            builder.Property(o => o.Status)
                   .IsRequired();

            builder.Property(o => o.LoadCapacity)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(o => o.isCanceled)
                   .HasDefaultValue(false);

            builder.HasMany(o => o.Payments)
                   .WithOne(p => p.Order)
                   .HasForeignKey(p => p.OrderID);

            builder.HasOne(o => o.User)
                   .WithMany()
                   .HasForeignKey(o => o.UserID);

            builder.HasMany(o => o.OrderTrucks)
                   .WithOne(to => to.Order)
                   .HasForeignKey(to => to.OrderID);
        }
    }
}
