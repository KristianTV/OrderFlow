using OrderFlow.GCommon;
using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.Truck
{
    public class OrderViewModel
    {
        public Guid OrderID { get; set; }

        [StringLength(ValidationConstants.Order.DeliveryAddressMaxLength,
                      MinimumLength = ValidationConstants.Order.DeliveryAddressMinLength,
                      ErrorMessage = ErrorMessages.StringLengthRange)]
        [Required(ErrorMessage = ErrorMessages.PropertyIsRequired)]
        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; } = string.Empty;
        public string PickupAddress { get; set; } = string.Empty;
        public int LoadCapacity { get; set; } = 0;
        public string OrderStatus { get; set; } = string.Empty;
        public bool IsSelected { get; set; } = false;
    }
}