using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OrderFlow.Data.Configuration
{
    internal class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<Guid>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder)
        {
            builder.HasData(SeedUserRoles());
        }

        private List<IdentityUserRole<Guid>> SeedUserRoles()
        {
            return new List<IdentityUserRole<Guid>>
            {
                new IdentityUserRole<Guid> { UserId = SeedDataIds.AdminUserId, RoleId = SeedDataIds.AdminRoleId },
                new IdentityUserRole<Guid> { UserId = SeedDataIds.SpeditorUserId, RoleId = SeedDataIds.SpeditorRoleId },
                new IdentityUserRole<Guid> { UserId = SeedDataIds.DriverUserId, RoleId = SeedDataIds.DriverRoleId },
                new IdentityUserRole<Guid> { UserId = SeedDataIds.DriverTwoUserId, RoleId = SeedDataIds.DriverRoleId },
                new IdentityUserRole<Guid> { UserId = SeedDataIds.RegularUserId, RoleId = SeedDataIds.UserRoleId },
                new IdentityUserRole<Guid> { UserId = SeedDataIds.CompanyUserId, RoleId = SeedDataIds.UserRoleId }
            };
        }
    }
}
