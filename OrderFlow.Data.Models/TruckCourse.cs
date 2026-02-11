using OrderFlow.Data.Models.Enums;

namespace OrderFlow.Data.Models
{
    public class TruckCourse
    {
        public Guid TruckCourseID { get; set; } = Guid.NewGuid();

        public Guid? TruckID { get; set; }
        public Truck? Truck { get; set; }

        public string PickupAddress { get; set; } = string.Empty;

        public string DeliverAddress { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public CourseStatus Status { get; set; } = CourseStatus.Pending;

        public decimal Income { get; set; } = 0;

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

        public ICollection<CourseOrder> CourseOrders { get; set; } = new List<CourseOrder>();

        public ICollection<TruckSpending> TruckSpendings { get; set; } = new List<TruckSpending>();
    }
}
