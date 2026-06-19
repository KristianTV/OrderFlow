using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.Data.Models.Enums;
using OrderFlow.GCommon;
namespace OrderFlow.Data.Configuration
{
    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.PaymentID);

            builder.Property(p => p.CreatedOn)
                   .IsRequired();

            builder.Property(p => p.PaymentDate)
                   .IsRequired(false);

            builder.Property(p => p.PaymentDescription)
                   .IsRequired(false)
                   .HasMaxLength(ValidationConstants.Payment.PaymentDescriptionMaxLength);

            builder.Property(p => p.Amount)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(p => p.PaymentMethod)
                   .IsRequired(false);

            builder.HasOne(p => p.Order)
                   .WithMany(o => o.Payments)
                   .HasForeignKey(o => o.OrderID);

            builder.HasData(SeedPayments());
        }

        private List<Payment> SeedPayments()
        {
            var payments = new List<Payment>();

            for (var orderIndex = 0; orderIndex < SeedDataIds.OrderIds.Length; orderIndex++)
            {
                var createdOn = new DateTime(2026, 6, 1).AddDays(orderIndex + 1);
                var courseIndex = SeedDataIds.GetCourseIndexForOrder(orderIndex);
                var isOrderCompleted = courseIndex.HasValue &&
                                       SeedDataIds.IsCourseDelivered(courseIndex.Value) &&
                                       orderIndex % 2 == 0;
                var paidOn = isOrderCompleted ? createdOn.AddDays(2) : (DateTime?)null;
                var paymentMethod = isOrderCompleted
                    ? orderIndex % 2 == 0 ? PaymentMethods.Card : PaymentMethods.Cash
                    : (PaymentMethods?)null;

                var paymentLines = new[]
                {
                    (Amount: 500m + (orderIndex * 25m), Description: "Transport fee"),
                    (Amount: 25m + (orderIndex % 3 * 5m), Description: "Parking fee"),
                    (Amount: 40m + (orderIndex % 4 * 10m), Description: "Toll fee"),
                    (Amount: -50m - (orderIndex % 2 * 10m), Description: "Customer discount")
                };

                for (var paymentIndex = 0; paymentIndex < paymentLines.Length; paymentIndex++)
                {
                    var line = paymentLines[paymentIndex];
                    var seedIndex = (orderIndex * paymentLines.Length) + paymentIndex;

                    payments.Add(new Payment
                    {
                        PaymentID = SeedDataIds.PaymentIds[seedIndex],
                        OrderID = SeedDataIds.OrderIds[orderIndex],
                        Amount = line.Amount,
                        CreatedOn = createdOn.AddMinutes(paymentIndex),
                        PaymentDescription = line.Description,
                        PaymentMethod = paymentMethod,
                        PaymentDate = paidOn
                    });
                }
            }

            return payments;
        }
    }
}
