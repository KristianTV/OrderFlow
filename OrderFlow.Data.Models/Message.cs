namespace OrderFlow.Data.Models
{
    public class Message
    {
        public Guid MessageID { get; set; } = Guid.NewGuid();

        public Guid SenderID { get; set; }
        public ApplicationUser Sender { get; set; } = null!;

        public Guid ReceiverID { get; set; }
        public ApplicationUser Receiver { get; set; } = null!;

        public Guid? NotificationID { get; set; }
        public Notification? Notification { get; set; }

        public string Content { get; set; } = string.Empty;

        public DateTime SentAt { get; set; }

        public bool IsRead { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

    }
}
