using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.GCommon;

namespace OrderFlow.Data.Configuration
{
    internal class TruckOrderConfiguration : IEntityTypeConfiguration<TruckOrder>
    {
        public void Configure(EntityTypeBuilder<TruckOrder> builder)
        {
            builder.HasKey(to => to.TruckOrderId);

            builder.Property(to => to.OrderID)
                   .IsRequired();

            builder.Property(to => to.TruckID)
                  .IsRequired();

            builder.Property(to => to.DeliverAddress)
                   .IsRequired()
                   .HasMaxLength(ValidationConstants.TruckOrder.DeliverAddressMaxLength);

            builder.Property(to => to.AssignedDate)
                   .IsRequired();

            builder.Property(to => to.DeliveryDate)
                   .IsRequired(false);

            builder.HasOne(to => to.Truck)
                   .WithMany(t => t.TruckOrders)
                   .HasForeignKey(to => to.TruckID)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(to => to.Order)
                   .WithMany(o => o.OrderTrucks)
                   .HasForeignKey(to => to.OrderID)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.Property(to => to.Status)
                   .IsRequired();

            builder.HasQueryFilter(t => !t.Truck.isDeleted);
        }
    }
}
