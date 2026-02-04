namespace OrderFlow.Data.Models
{
    public class Notification
    {
        public Guid NotificationID { get; set; } = Guid.NewGuid();

        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public bool CanRespond { get; set; } = true;

        public Guid ReceiverID { get; set; }
        public ApplicationUser Receiver { get; set; } = null!;

        public Guid? SenderID { get; set; }
        public ApplicationUser? Sender { get; set; }

        public Guid? OrderID { get; set; }
        public Order? Order { get; set; }

        public Guid? TruckID { get; set; }
        public Truck? Truck { get; set; }

        public Guid? CourseID { get; set; }
        public TruckCourse? Course { get; set; }

        public Guid? PaymentID { get; set; }
        public Payment? Payment { get; set; }

        public Guid? TruckSpendingID { get; set; }
        public TruckSpending? TruckSpending { get; set; }

        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
