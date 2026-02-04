using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.GCommon;

namespace OrderFlow.Data.Configuration
{
    public class TruckCourseConfiguration : IEntityTypeConfiguration<TruckCourse>
    {
        public void Configure(EntityTypeBuilder<TruckCourse> builder)
        {
            builder.HasKey(to => to.TruckCourseID);

            builder.Property(to => to.DeliverAddress)
                   .IsRequired()
                   .HasMaxLength(ValidationConstants.TruckCourse.DeliverAddressMaxLength);

            builder.Property(to => to.PickupAddress)
                   .IsRequired()
                   .HasMaxLength(ValidationConstants.TruckCourse.PickupAddressAddressMaxLength);

            builder.Property(to => to.AssignedDate)
                   .IsRequired();

            builder.Property(to => to.DeliveryDate)
                   .IsRequired(false);

            builder.Property(to => to.Status)
                   .IsRequired();

            builder.Property(to => to.Income)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.HasMany(tc => tc.CourseOrders)
                   .WithOne(co => co.TruckCourse)
                   .HasForeignKey(co => co.TruckCourseID)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(tc => tc.TruckSpendings)
                   .WithOne(ts => ts.TruckCourse)
                   .HasForeignKey(ts => ts.TruckCourseID)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(tc => tc.Notifications)
                   .WithOne(n => n.Course)
                   .HasForeignKey(n => n.CourseID)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
