using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.GCommon;

namespace OrderFlow.Data.Configuration
{
    public class TruckSpendingConfiguration : IEntityTypeConfiguration<TruckSpending>
    {
        public void Configure(EntityTypeBuilder<TruckSpending> builder)
        {
            builder.HasKey(ts => ts.TruckSpendingID);

            builder.Property(ts => ts.Amount)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(ts => ts.PaymentDate)
                   .IsRequired();

            builder.Property(ts => ts.PaymentDescription)
                   .IsRequired()
                   .HasMaxLength(ValidationConstants.TruckSpending.PaymentDescriptionMaxLength);

            builder.Property(ts => ts.PaymentMethod)
                   .IsRequired();

            builder.HasOne(ts => ts.Truck)
                   .WithMany(t => t.TruckSpendings)
                   .HasForeignKey(ts => ts.TruckID)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(ts => ts.TruckCourse)
                   .WithMany(tc => tc.TruckSpendings)
                   .HasForeignKey(ts => ts.TruckCourseID)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(ts => ts.Notifications)
                   .WithOne(n => n.TruckSpending)
                   .HasForeignKey(n => n.TruckSpendingID)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
