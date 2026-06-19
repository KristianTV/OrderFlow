using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;

namespace OrderFlow.Data.Configuration
{
    public class CourseOrderConfiguration : IEntityTypeConfiguration<CourseOrder>
    {
        public void Configure(EntityTypeBuilder<CourseOrder> builder)
        {
            builder.HasKey(co => new { co.OrderID, co.TruckCourseID });

            builder.HasOne(co => co.Order)
                   .WithMany(o => o.CourseOrders)
                   .HasForeignKey(co => co.OrderID);

            builder.HasOne(co => co.TruckCourse)
                 .WithMany(tc => tc.CourseOrders)
                 .HasForeignKey(co => co.TruckCourseID);

            builder.HasData(SeedCourseOrders());
        }

        private List<CourseOrder> SeedCourseOrders()
        {
            return Enumerable.Range(0, SeedDataIds.AssignedOrdersCount)
                .Select(orderIndex => new CourseOrder
                {
                    TruckCourseID = SeedDataIds.CourseIds[orderIndex / 2],
                    OrderID = SeedDataIds.OrderIds[orderIndex]
                })
                .ToList();
        }
    }
}
