namespace OrderFlow.ViewModels.Notification
{
    public class DriverDetailsNotificationViewModel : DetailsNotificationViewModel
    {
        public Guid? TruckId { get; set; }

        public Guid? CourseId { get; set; }

        public Guid? TruckSpendingId { get; set; }
    }
}
