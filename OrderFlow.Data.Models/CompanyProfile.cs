using Microsoft.AspNetCore.Identity;

namespace OrderFlow.Data.Models
{
    public class CompanyProfile
    {
        public Guid UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;

        [ProtectedPersonalData]
        public string CompanyName { get; set; } = null!;

        [ProtectedPersonalData]
        public string VATNumber { get; set; } = null!;

        [ProtectedPersonalData]
        public string CompanyAdress { get; set; } = null!;

        [ProtectedPersonalData]
        public string ContactPersonName { get; set; } = null!;

        [ProtectedPersonalData]
        public string ContactPhone { get; set; } = null!;
    }
}