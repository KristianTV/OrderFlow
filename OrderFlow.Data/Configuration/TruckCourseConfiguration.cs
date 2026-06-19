using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
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

            builder.HasData(SeedTruckCourses());
        }

        private List<TruckCourse> SeedTruckCourses()
        {
            var courses = new List<TruckCourse>();
            for (var truckIndex = 0; truckIndex < SeedDataIds.TruckIds.Length; truckIndex++)
            {
                for (var courseIndex = 0; courseIndex < 3; courseIndex++)
                {
                    var seedIndex = (truckIndex * 3) + courseIndex;
                    var courseNumber = courseIndex + 1;
                    var assignedDate = new DateTime(2026, 6, 10).AddDays(seedIndex + 1);
                    var firstOrderIndex = seedIndex * 2;
                    var hasOrders = firstOrderIndex < SeedDataIds.AssignedOrdersCount;
                    var isDelivered = SeedDataIds.IsCourseDelivered(seedIndex);

                    courses.Add(new TruckCourse
                    {
                        TruckCourseID = SeedDataIds.CourseIds[seedIndex],
                        TruckID = SeedDataIds.TruckIds[truckIndex],
                        PickupAddress = hasOrders
                            ? SeedDataIds.PickupAddresses[firstOrderIndex]
                            : SeedDataIds.PickupAddresses[seedIndex % SeedDataIds.PickupAddresses.Length],
                        DeliverAddress = hasOrders
                            ? SeedDataIds.DeliveryAddresses[firstOrderIndex]
                            : SeedDataIds.DeliveryAddresses[seedIndex % SeedDataIds.DeliveryAddresses.Length],
                        AssignedDate = assignedDate,
                        DeliveryDate = isDelivered ? assignedDate.AddDays(1) : null,
                        Status = isDelivered ? CourseStatus.Delivered : CourseStatus.Assigned,
                        Income = 700 + (courseNumber * 150)
                    });
                }
            }

            return courses;
        }
    }
}
