using OrderFlow.ViewModels.Message;

namespace OrderFlow.ViewModels.Notification
{
    public class DetailsNotificationViewModel
    {
        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; } = false;

        public string? SenderName { get; set; }

        public Guid? OrderId { get; set; }

        public IEnumerable<DetailsNotificationMessageViewModel>? Messages { get; set; }

        public bool isMarkable { get; set; } = true;

        public bool IsResponseEnabled { get; set; }

    }
}
