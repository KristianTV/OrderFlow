using OrderFlow.GCommon;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.Order
{
    public class CreateOrderViewModel
    {
        [StringLength(ValidationConstants.Order.DeliveryAddressMaxLength,
                      MinimumLength = ValidationConstants.Order.DeliveryAddressMinLength,
                      ErrorMessage = ErrorMessages.StringLengthRange)]
        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [StringLength(ValidationConstants.Order.PickupAddressMaxLength,
                      MinimumLength = ValidationConstants.Order.PickupAddressMinLength,
                      ErrorMessage = ErrorMessages.StringLengthRange)]
        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Pickup Address")]
        public string PickupAddress { get; set; } = string.Empty;

        [StringLength(ValidationConstants.Order.DeliveryInstructionsMaxLength,
                        MinimumLength = ValidationConstants.Order.DeliveryInstructionsMinLength,
                        ErrorMessage = ErrorMessages.StringLengthRange)]
        [Display(Name = "Delivery Instructions")]
        public string? DeliveryInstructions { get; set; }


        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Range(ValidationConstants.Order.LoadCapacityMinLength,
                   ValidationConstants.Order.LoadCapacityMaxLength,
                   ErrorMessage = ErrorMessages.PositiveNumber)]
        [Display(Name = "Load Capacity")]
        public int LoadCapacity { get; set; } = 0;
    }
}
