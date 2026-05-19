namespace OrderFlow.ViewModels.Order
{
    public class InvoiceOrderViewModel
    {
        public Guid OrderID { get; set; }

        public string DeliveryAddress { get; set; } = string.Empty;

        public string PickupAddress { get; set; } = string.Empty;

        public double LoadCapacity { get; set; } = 0;

        public DateTime OrderDate { get; set; }

        public DateTime? DeliveryDate { get; set; }

    }
}
