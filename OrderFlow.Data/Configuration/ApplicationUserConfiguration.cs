using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderFlow.Data.Models;
public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.AccountType)
               .IsRequired();

        builder.HasOne(u => u.PersonalProfile)
               .WithOne(p => p.User)
               .HasForeignKey<PersonalProfile>(p => p.UserId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(u => u.CompanyProfile)
               .WithOne(c => c.User)
               .HasForeignKey<CompanyProfile>(c => c.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}