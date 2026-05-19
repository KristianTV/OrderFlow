namespace OrderFlow.ViewModels.Order
{
    public class PaymentViewModel
    {
        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? PaymentDescription { get; set; }
    }
}
