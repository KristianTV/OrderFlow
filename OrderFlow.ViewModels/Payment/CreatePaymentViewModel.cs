using OrderFlow.GCommon;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.Payment
{
    public class CreatePaymentViewModel
    {
        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Order ID")]
        public decimal Amount { get; set; }

        [StringLength(ValidationConstants.Payment.PaymentDescriptionMaxLength,
                     MinimumLength = ValidationConstants.Payment.PaymentDescriptionMinLength,
                     ErrorMessage = ErrorMessages.StringLengthRange)]
        [Display(Name = "Payment Description")]
        public string? PaymentDescription { get; set; }
    }
}
