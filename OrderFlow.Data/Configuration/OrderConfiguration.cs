using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
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

            builder.Property(o => o.IsCanceled)
                   .HasDefaultValue(false);

            builder.HasMany(o => o.Payments)
                   .WithOne(p => p.Order)
                   .HasForeignKey(p => p.OrderID);

            builder.HasOne(o => o.User)
                   .WithMany()
                   .HasForeignKey(o => o.UserID);

            builder.HasMany(o => o.Notifications)
                   .WithOne(p => p.Order)
                   .HasForeignKey(p => p.OrderID);

            builder.HasMany(o => o.CourseOrders)
                 .WithOne(p => p.Order)
                 .HasForeignKey(p => p.OrderID);

            builder.HasData(SeedOrders());
        }

        private List<Order> SeedOrders()
        {
            var customerIds = new[] { SeedDataIds.RegularUserId, SeedDataIds.CompanyUserId };
            var orders = new List<Order>();
            for (var customerIndex = 0; customerIndex < customerIds.Length; customerIndex++)
            {
                for (var orderIndex = 0; orderIndex < 10; orderIndex++)
                {
                    var seedIndex = (customerIndex * 10) + orderIndex;
                    var orderNumber = orderIndex + 1;
                    var orderDate = new DateTime(2026, 6, 1).AddDays(seedIndex + 1);
                    var courseIndex = SeedDataIds.GetCourseIndexForOrder(seedIndex);
                    var isCompleted = courseIndex.HasValue &&
                                      SeedDataIds.IsCourseDelivered(courseIndex.Value) &&
                                      seedIndex % 2 == 0;

                    orders.Add(new Order
                    {
                        OrderID = SeedDataIds.OrderIds[seedIndex],
                        UserID = customerIds[customerIndex],
                        OrderDate = orderDate,
                        DeliveryDate = isCompleted
                            ? new DateTime(2026, 6, 10).AddDays(courseIndex!.Value + 2)
                            : null,
                        PickupAddress = SeedDataIds.PickupAddresses[seedIndex],
                        DeliveryAddress = SeedDataIds.DeliveryAddresses[seedIndex],
                        LoadCapacity = 3 + (seedIndex % 3),
                        DeliveryInstructions = $"Seeded sample order {orderNumber}.",
                        Status = courseIndex.HasValue
                            ? isCompleted ? OrderStatus.Completed : OrderStatus.InProgress
                            : OrderStatus.Pending,
                        IsCanceled = false
                    });
                }
            }

            return orders;
        }
    }
}
