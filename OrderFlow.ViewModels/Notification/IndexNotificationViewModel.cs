namespace OrderFlow.ViewModels.Notification
{
    public class IndexNotificationViewModel
    {
        public Guid NotificationID { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; } = false;

        public string? SenderName { get; set; }

        public Guid? OrderId { get; set; }

        public bool isMarkable { get; set; } = true;
    }
}
