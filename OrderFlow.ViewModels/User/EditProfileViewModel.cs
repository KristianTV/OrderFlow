using OrderFlow.GCommon;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.User
{
    public class EditProfileViewModel
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
        public string UserPhone { get; set; } = string.Empty;
    }
}
