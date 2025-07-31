using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;

namespace OrderFlow.Data.Configuration
{
    internal class TruckOrderConfiguration : IEntityTypeConfiguration<TruckOrder>
    {
        public void Configure(EntityTypeBuilder<TruckOrder> builder)
        {
            builder.HasKey(to => new { to.OrderID, to.TruckID });

            builder.HasOne(to => to.Truck)
                   .WithMany(t => t.TruckOrders)
                   .HasForeignKey(to => to.TruckID);

            builder.HasOne(to => to.Order)
                   .WithOne(o => o.TruckOrder)
                   .HasForeignKey<TruckOrder>(to => to.OrderID)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(t => !t.Truck.isDeleted);
        }
    }
}
