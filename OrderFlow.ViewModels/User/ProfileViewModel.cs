using OrderFlow.Data.Models.Enums;

namespace OrderFlow.ViewModels.User
{
    public class ProfileViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPhone { get; set; } = string.Empty;
        public AccountType AccountType { get; set; }
        public string AccountTypeName => AccountType.ToString();
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PersonalNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string VATNumber { get; set; } = string.Empty;
        public string CompanyAddress { get; set; } = string.Empty;
        public string ContactPersonName { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
    }
}
