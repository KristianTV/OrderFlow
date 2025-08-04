using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.Notification
{
    public class AdminCreateNotificationViewModel : CreateNotificationViewModel
    {
        [Display(Name = "Truck Id")]
        public Guid? TruckId { get; set; }

        public IDictionary<Guid, string>? Trucks { get; set; } = new Dictionary<Guid, string>();
    }
}
