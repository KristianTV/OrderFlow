namespace OrderFlow.ViewModels.Order
{
    public class InvoiceViewModel
    {
        public string InvoiceId { get; set; } = string.Empty;

        public string DisplayInvoiceId => $"INV-{Order.OrderID.ToString("N")[..8].ToUpperInvariant()}";

        public DateTime? Issued { get; set; }

        public InvoiceOrderViewModel Order { get; set; } = new InvoiceOrderViewModel();

        public string UserName { get; set; } = string.Empty;

        public string BuyerType { get; set; } = string.Empty;

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PersonalNumber { get; set; }

        public string? CompanyName { get; set; }

        public string? VATNumber { get; set; }

        public string BuyerDisplayName
        {
            get
            {
                string personalName = $"{FirstName} {LastName}".Trim();

                if (!string.IsNullOrWhiteSpace(CompanyName))
                {
                    return CompanyName;
                }

                return !string.IsNullOrWhiteSpace(personalName) ? personalName : UserName;
            }
        }

        public decimal TotalPrice { get; set; }

        public decimal PaidTotal => Payments.Where(payment => payment.IsPaid).Sum(payment => payment.Amount);

        public decimal PendingTotal => Payments.Where(payment => !payment.IsPaid).Sum(payment => payment.Amount);

        public IEnumerable<PaymentViewModel> Payments { get; set; } = new List<PaymentViewModel>();
    }
}
