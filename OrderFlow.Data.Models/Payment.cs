using OrderFlow.Data.Models.Enums;

namespace OrderFlow.Data.Models
{
    public class Payment
    {
        public Guid PaymentID { get; set; } = Guid.NewGuid();

        public Guid OrderID { get; set; }
        public Order Order { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime CreatedOn { get; set; }

        public string PaymentDescription { get; set; } = string.Empty;

        public PaymentMethods? PaymentMethod { get; set; }

        public DateTime? PaymentDate { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    }
}
