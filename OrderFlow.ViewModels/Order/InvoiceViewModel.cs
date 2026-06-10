namespace OrderFlow.ViewModels.Order
{
    public class InvoiceViewModel
    {
        public string InvoiceId { get; set; } = string.Empty;

        public string DisplayInvoiceId => $"INV-{Order.OrderID.ToString("N")[..8].ToUpperInvariant()}";

        public DateTime? Issued { get; set; }

        public InvoiceOrderViewModel Order { get; set; } = new InvoiceOrderViewModel();

        public string UserName { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }

        public IEnumerable<PaymentViewModel> Payments { get; set; } = new List<PaymentViewModel>();
    }
}
