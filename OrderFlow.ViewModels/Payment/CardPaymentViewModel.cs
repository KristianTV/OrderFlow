using OrderFlow.GCommon;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.Payment
{
    public class CardPaymentViewModel
    {
        public Guid OrderId { get; set; }

        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Range(0.01, 999999.99, ErrorMessage = ErrorMessages.RangeError)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Cardholder Name")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = ErrorMessages.StringLengthRange)]
        public string CardholderName { get; set; } = string.Empty;

        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Card Number")]
        [RegularExpression(@"^[\d\s-]{13,23}$", ErrorMessage = "Card number must contain only digits, spaces, or dashes.")]
        public string CardNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Expiry Date")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Expiry date must be in MM/YY format.")]
        public string ExpiryDate { get; set; } = string.Empty;

        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV must contain 3 or 4 digits.")]
        public string Cvv { get; set; } = string.Empty;

        [Display(Name = "Demo payment succeeds")]
        public bool PaymentSuccess { get; set; } = true;
    }
}
