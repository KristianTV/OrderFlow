using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.User
{
    public class ResetPasswordViewModel
    {
        [Required]
        [StringLength(20, MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; } = null!;

        [Compare(nameof(NewPassword))]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        public string ConfirmNewPassword { get; set; } = null!;
        public string? Email { get; set; }
        public string? Token { get; set; }
    }
}
