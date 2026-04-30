namespace OrderFlow.ViewModels.Message
{
    public class DetailsNotificationMessageViewModel
    {
        public Guid MessageID { get; set; }

        public Guid SenderID { get; set; }

        public string SenderName { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; } = false;

    }
}
