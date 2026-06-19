using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
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

            builder.Property(ts => ts.SpendingType)
                   .IsRequired();

            builder.Property(ts => ts.PaymentDescription)
                   .IsRequired(false)
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

            builder.HasData(SeedTruckSpendings());
        }

        private List<TruckSpending> SeedTruckSpendings()
        {
            return SeedDataIds.CourseIds.Select((courseId, index) =>
            {
                var courseNumber = (index % 3) + 1;

                return new TruckSpending
                {
                    TruckSpendingID = SeedDataIds.TruckSpendingIds[index],
                    TruckID = SeedDataIds.TruckIds[index / 3],
                    TruckCourseID = courseId,
                    Amount = 90 + (courseNumber * 35),
                    PaymentDate = new DateTime(2026, 6, 10).AddDays(index + 1),
                    SpendingType = courseNumber % 2 == 0 ? TruckSpendingsType.Tolls : TruckSpendingsType.Fuel,
                    PaymentDescription = $"Seed spending for course {courseNumber}",
                    PaymentMethod = courseNumber % 2 == 0 ? PaymentMethods.Cash : PaymentMethods.Card
                };
            }).ToList();
        }
    }
}
