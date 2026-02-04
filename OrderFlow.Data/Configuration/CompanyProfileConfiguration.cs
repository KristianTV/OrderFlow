using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.GCommon;

namespace OrderFlow.Data.Configuration
{
    public class CompanyProfileConfiguration : IEntityTypeConfiguration<CompanyProfile>
    {
        public void Configure(EntityTypeBuilder<CompanyProfile> builder)
        {
            builder.HasKey(cp => cp.UserId);

            builder.Property(cp => cp.CompanyName)
                .IsRequired()
                .HasMaxLength(ValidationConstants.CompanyProfile.CompanyNameMaxLength);

            builder.Property(cp => cp.VATNumber)
                .IsRequired()
                .HasMaxLength(ValidationConstants.CompanyProfile.VATNumberMaxLength);

            builder.Property(cp => cp.CompanyAdress)
                .IsRequired()
                .HasMaxLength(ValidationConstants.CompanyProfile.CompanyAdressMaxLength);

            builder.Property(cp => cp.ContactPersonName)
                .IsRequired()
                .HasMaxLength(ValidationConstants.CompanyProfile.ContactPersonNameMaxLength);

            builder.Property(cp => cp.ContactPhone)
                .IsRequired()
                .HasMaxLength(ValidationConstants.CompanyProfile.ContactPhoneMaxLength);

            builder.HasOne(cp => cp.User)
                .WithOne(u => u.CompanyProfile)
                .HasForeignKey<CompanyProfile>(cp => cp.UserId);
        }
    }
}
