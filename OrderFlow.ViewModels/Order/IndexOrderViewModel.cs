namespace OrderFlow.ViewModels.Order
{
    public class IndexOrderViewModel
    {
        public Guid OrderID { get; set; } = Guid.NewGuid();

        public string DisplayOrderId => $"ORD-{OrderID.ToString("N")[..8].ToUpperInvariant()}";

        public DateTime OrderDate { get; set; }

        public string DeliveryAddress { get; set; } = string.Empty;
        public string PickupAddress { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public double LoadCapacity { get; set; } = 0;

        public bool isCanceled { get; set; } = false;
    }
}
