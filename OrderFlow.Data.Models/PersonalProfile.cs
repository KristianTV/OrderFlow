using Microsoft.AspNetCore.Identity;

namespace OrderFlow.Data.Models
{
    public class PersonalProfile
    {
        public Guid UserId { get; set; }
        public ApplicationUser? User { get; set; } = null!;

        [ProtectedPersonalData]
        public string FirstName { get; set; } = null!;

        [ProtectedPersonalData]
        public string LastName { get; set; } = null!;

        [ProtectedPersonalData]
        public string PersonalNumber { get; set; } = null!;

        [ProtectedPersonalData]
        public string Adress { get; set; } = null!;

        [ProtectedPersonalData]
        public DateTime? DateOfBirth { get; set; }
    }
}