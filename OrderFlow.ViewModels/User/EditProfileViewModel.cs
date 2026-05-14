using OrderFlow.Data.Models.Enums;
using OrderFlow.GCommon;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.User
{
    public class EditProfileViewModel : IValidatableObject
    {
        [Required(ErrorMessage = ErrorMessages.UserNameIsRequired)]
        [StringLength(ValidationConstants.EditProfile.UserNameMaxLength,
            MinimumLength = ValidationConstants.EditProfile.UserNameMinLength,
            ErrorMessage = ErrorMessages.StringLengthRange)]
        [Display(Name = "Username")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = ErrorMessages.UserEmailIsRequired)]
        [EmailAddress(ErrorMessage = ErrorMessages.EmailAddressInvalidFormat)]
        [Display(Name = "Email Address")]
        public string UserEmail { get; set; } = string.Empty;

        [Phone(ErrorMessage = ErrorMessages.UserPhoneIsRequired)]
        [StringLength(ValidationConstants.EditProfile.UserPhoneMaxLength,
            MinimumLength = ValidationConstants.EditProfile.UserPhoneMinLength,
            ErrorMessage = ErrorMessages.StringLengthRange)]
        [Display(Name = "Phone Number")]
        public string? UserPhone { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public AccountType AccountType { get; set; }

        [Display(Name = "First Name")]
        [StringLength(ValidationConstants.PersonalProfile.FirstNameMaxLength, MinimumLength = ValidationConstants.PersonalProfile.FirstNameMinLength)]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(ValidationConstants.PersonalProfile.LastNameMaxLength, MinimumLength = ValidationConstants.PersonalProfile.LastNameMinLength)]
        public string? LastName { get; set; }

        [Display(Name = "Personal Number")]
        [StringLength(ValidationConstants.PersonalProfile.PersonalNumberMaxLength, MinimumLength = ValidationConstants.PersonalProfile.PersonalNumberMinLength)]
        public string? PersonalNumber { get; set; }

        [Display(Name = "Address")]
        [StringLength(ValidationConstants.PersonalProfile.AdressMaxLength, MinimumLength = ValidationConstants.PersonalProfile.AdressMinLength)]
        public string? Address { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date Of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Company Name")]
        [StringLength(ValidationConstants.CompanyProfile.CompanyNameMaxLength, MinimumLength = ValidationConstants.CompanyProfile.CompanyNameMinLength)]
        public string? CompanyName { get; set; }

        [Display(Name = "VAT Number")]
        [StringLength(ValidationConstants.CompanyProfile.VATNumberMaxLength, MinimumLength = ValidationConstants.CompanyProfile.VATNumberMinLength)]
        public string? VATNumber { get; set; }

        [Display(Name = "Company Address")]
        [StringLength(ValidationConstants.CompanyProfile.CompanyAdressMaxLength, MinimumLength = ValidationConstants.CompanyProfile.CompanyAdressMinLength)]
        public string? CompanyAddress { get; set; }

        [Display(Name = "Contact Person")]
        [StringLength(ValidationConstants.CompanyProfile.ContactPersonNameMaxLength, MinimumLength = ValidationConstants.CompanyProfile.ContactPersonNameMinLength)]
        public string? ContactPersonName { get; set; }

        [Phone]
        [Display(Name = "Contact Phone")]
        [StringLength(ValidationConstants.CompanyProfile.ContactPhoneMaxLength, MinimumLength = ValidationConstants.CompanyProfile.ContactPhoneMinLength)]
        public string? ContactPhone { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (AccountType == AccountType.Personal)
            {
                if (string.IsNullOrWhiteSpace(FirstName))
                {
                    yield return new ValidationResult(string.Format(ErrorMessages.PropertyIsRequired, "First Name"), [nameof(FirstName)]);
                }

                if (string.IsNullOrWhiteSpace(LastName))
                {
                    yield return new ValidationResult(string.Format(ErrorMessages.PropertyIsRequired, "Last Name"), [nameof(LastName)]);
                }

                if (string.IsNullOrWhiteSpace(PersonalNumber))
                {
                    yield return new ValidationResult(string.Format(ErrorMessages.PropertyIsRequired, "Personal Number"), [nameof(PersonalNumber)]);
                }

                if (string.IsNullOrWhiteSpace(Address))
                {
                    yield return new ValidationResult(string.Format(ErrorMessages.PropertyIsRequired, "Address"), [nameof(Address)]);
                }
            }

            if (AccountType == AccountType.Company)
            {
                if (string.IsNullOrWhiteSpace(CompanyName))
                {
                    yield return new ValidationResult(string.Format(ErrorMessages.PropertyIsRequired, "Company Name"), [nameof(CompanyName)]);
                }

                if (string.IsNullOrWhiteSpace(VATNumber))
                {
                    yield return new ValidationResult(string.Format(ErrorMessages.PropertyIsRequired, "VAT Number"), [nameof(VATNumber)]);
                }

                if (string.IsNullOrWhiteSpace(CompanyAddress))
                {
                    yield return new ValidationResult(string.Format(ErrorMessages.PropertyIsRequired, "Company Address"), [nameof(CompanyAddress)]);
                }

                if (string.IsNullOrWhiteSpace(ContactPersonName))
                {
                    yield return new ValidationResult(string.Format(ErrorMessages.PropertyIsRequired, "Contact Person"), [nameof(ContactPersonName)]);
                }

                if (string.IsNullOrWhiteSpace(ContactPhone))
                {
                    yield return new ValidationResult(string.Format(ErrorMessages.PropertyIsRequired, "Contact Phone"), [nameof(ContactPhone)]);
                }
            }
        }
    }
}
