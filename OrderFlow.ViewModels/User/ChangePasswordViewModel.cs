using OrderFlow.GCommon;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.User
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [StringLength(ValidationConstants.ChangePassword.NewPasswordMaxLength,
            MinimumLength = ValidationConstants.ChangePassword.NewPasswordMinLength
            , ErrorMessage = ErrorMessages.StringLengthRange)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = ErrorMessages.ConfirmPasswordCompare)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
