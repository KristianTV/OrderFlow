namespace OrderFlow.ViewModels.Notification
{
    public class CreateNotificationViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public Guid ReceiverId { get; set; }

        public Guid? OrderId { get; set; }

        public IDictionary<Guid, string>? Orders { get; set; } = new Dictionary<Guid, string>();

        public IDictionary<Guid, string>? Receivers { get; set; } = new Dictionary<Guid, string>();

    }
}
