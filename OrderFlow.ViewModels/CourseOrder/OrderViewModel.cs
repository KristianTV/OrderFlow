using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.CourseOrder
{
    public class OrderViewModel
    {
        public Guid OrderID { get; set; }

        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; } = string.Empty;

        [Display(Name = "Pickup Address")]
        public string PickupAddress { get; set; } = string.Empty;

        [Display(Name = "Load Capacity")]
        public double LoadCapacity { get; set; } = 0;

        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; } = string.Empty;

        [Display(Name = "Select")]
        public bool IsSelected { get; set; } = false;
    }
}