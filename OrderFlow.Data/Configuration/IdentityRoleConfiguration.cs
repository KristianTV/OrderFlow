using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderFlow.Data.Configuration
{
    internal class IdentityRoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
        {
            builder.HasData(SeedRoles());
        }

        private List<IdentityRole<Guid>> SeedRoles()
        {
            return new List<IdentityRole<Guid>>
            {
                CreateRole(SeedDataIds.AdminRoleId, "Admin"),
                CreateRole(SeedDataIds.SpeditorRoleId, "Speditor"),
                CreateRole(SeedDataIds.DriverRoleId, "Driver"),
                CreateRole(SeedDataIds.UserRoleId, "User")
            };
        }

        private static IdentityRole<Guid> CreateRole(Guid id, string name)
            => new()
            {
                Id = id,
                Name = name,
                NormalizedName = name.ToUpperInvariant(),
                ConcurrencyStamp = null
            };
    }
}
