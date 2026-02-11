using OrderFlow.Data.Models.Enums;

namespace OrderFlow.ViewModels.Course
{
    public class IndexCourseViewModel
    {
        public Guid TruckCourseID { get; set; }

        public Guid? TruckID { get; set; }

        public string? PickupAddress { get; set; }

        public string? DeliverAddress { get; set; }

        public DateTime AssignedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public CourseStatus Status { get; set; }

        public decimal Income { get; set; }
    }
}
