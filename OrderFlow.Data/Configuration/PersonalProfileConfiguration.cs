using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
using OrderFlow.GCommon;

namespace OrderFlow.Data.Configuration
{
    public class PersonalProfileConfiguration : IEntityTypeConfiguration<PersonalProfile>
    {
        public void Configure(EntityTypeBuilder<PersonalProfile> builder)
        {
            builder.HasKey(pp => pp.UserId);

            builder.Property(pp => pp.FirstName)
                   .IsRequired()
                   .HasMaxLength(ValidationConstants.PersonalProfile.FirstNameMaxLength);

            builder.Property(pp => pp.LastName)
                   .IsRequired()
                   .HasMaxLength(ValidationConstants.PersonalProfile.LastNameMaxLength);

            builder.Property(pp => pp.PersonalNumber)
                   .IsRequired()
                   .HasMaxLength(ValidationConstants.PersonalProfile.PersonalNumberMaxLength);

            builder.Property(pp => pp.Adress)
                   .IsRequired()
                   .HasMaxLength(ValidationConstants.PersonalProfile.AdressMaxLength);

            builder.Property(builder => builder.DateOfBirth)
                   .IsRequired(false);

            builder.HasOne(pp => pp.User)
                   .WithOne(u => u.PersonalProfile)
                   .HasForeignKey<PersonalProfile>(pp => pp.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
