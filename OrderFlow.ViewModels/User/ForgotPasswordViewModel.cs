using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.User
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
