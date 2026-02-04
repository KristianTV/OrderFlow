using Microsoft.AspNetCore.Identity;
using OrderFlow.Data.Models.Enums;

namespace OrderFlow.Data.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public AccountType AccountType { get; set; }

        public PersonalProfile? PersonalProfile { get; set; }
        public CompanyProfile? CompanyProfile { get; set; }
    }
}
