using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.GCommon;
namespace OrderFlow.Data.Configuration
{
    internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.PaymentID);

            builder.Property(p => p.PaymentDate)
                   .IsRequired();

            builder.Property(p => p.PaymentDescription)
                   .IsRequired(false)
                   .HasMaxLength(ValidationConstants.Payment.PaymentDescriptionMaxLength);

            builder.Property(p => p.Amount)
                   .IsRequired()
                   .HasPrecision(18, 2);

            builder.Property(p => p.PaymentMethod)
                   .IsRequired();

            builder.HasOne(p => p.Order)
                   .WithMany(o => o.Payments)
                   .HasForeignKey(o => o.OrderID);

        }

    }
}
