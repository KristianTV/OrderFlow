namespace OrderFlow.ViewModels.Notification
{
    public class IndexNotificationViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; } = false;

        public string? SenderName { get; set; }

        public string? OrderId { get; set; }
    }
}
