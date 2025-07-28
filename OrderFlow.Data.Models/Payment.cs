namespace OrderFlow.Data.Models
{
    public class Payment
    {
        public Guid Id { get; set; }

        public Guid OrderID { get; set; }
        public Order Order { get; set; } = null!;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentDescription { get; set; } = string.Empty;

    }
}
