using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.Notification
{
    public class AdminCreateNotificationViewModel : CreateNotificationViewModel
    {
        [Display(Name = "Truck Id")]
        public Guid? TruckId { get; set; }

        public IDictionary<Guid, string>? Trucks { get; set; } = new Dictionary<Guid, string>();

        [Display(Name = "Course")]
        public Guid? CourseId { get; set; }
        public IDictionary<Guid, string>? Courses { get; set; } = new Dictionary<Guid, string>();

        [Display(Name = "Payment")]
        public Guid? PaymentId { get; set; }
        public IDictionary<Guid, string>? Payments { get; set; } = new Dictionary<Guid, string>();

        [Display(Name = "Truck Spending")]
        public Guid? TruckSpendingId { get; set; }
        public IDictionary<Guid, string>? TruckSpendings { get; set; } = new Dictionary<Guid, string>();

        [Display(Name = "Enable Response")]
        public bool IsResponseEnabled { get; set; } = true;
    }
}
