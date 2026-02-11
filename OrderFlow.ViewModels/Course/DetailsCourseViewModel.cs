using OrderFlow.Data.Models.Enums;
using OrderFlow.ViewModels.Order;
using System.ComponentModel;

namespace OrderFlow.ViewModels.Course
{
    public class DetailsCourseViewModel
    {
        public Guid TruckCourseID { get; set; }

        public Guid? TruckID { get; set; }

        [DisplayName("Truck")]
        public string? TruckPlates { get; set; }

        [DisplayName("Pickup Address")]
        public string? PickupAddress { get; set; }

        [DisplayName("Delivery Address")]
        public string? DeliverAddress { get; set; }

        [DisplayName("Assigned Date")]
        public DateTime AssignedDate { get; set; }

        [DisplayName("Delivery Date")]
        public DateTime? DeliveryDate { get; set; }

        public CourseStatus Status { get; set; }

        public decimal Income { get; set; }

        [DisplayName("Assined Orders")]
        public ICollection<IndexOrderViewModel> AssinedOrders { get; set; } = new HashSet<IndexOrderViewModel>();
    }
}
