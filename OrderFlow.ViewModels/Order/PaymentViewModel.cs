namespace OrderFlow.ViewModels.Order
{
    public class PaymentViewModel
    {
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? PaymentDescription { get; set; }

        public string? PaymentMethod { get; set; }

        public DateTime? PaymentDate { get; set; }

        public bool IsPaid => PaymentDate.HasValue;
    }
}
