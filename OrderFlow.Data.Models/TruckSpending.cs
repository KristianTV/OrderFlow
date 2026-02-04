using OrderFlow.Data.Models.Enums;

namespace OrderFlow.Data.Models
{
    public class TruckSpending
    {
        public Guid TruckSpendingID { get; set; } = Guid.NewGuid();

        public Guid? TruckID { get; set; }
        public Truck? Truck { get; set; }

        public Guid? TruckCourseID { get; set; }
        public TruckCourse? TruckCourse { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public string PaymentDescription { get; set; } = string.Empty;

        public PaymentMethods PaymentMethod { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    }
}
