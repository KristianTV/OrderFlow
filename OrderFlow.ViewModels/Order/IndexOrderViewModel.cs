namespace OrderFlow.ViewModels.Order
{
    public class IndexOrderViewModel
    {
        public Guid OrderID { get; set; } = Guid.NewGuid();

        public DateTime OrderDate { get; set; }

        public string DeliveryAddress { get; set; } = string.Empty;
        public string PickupAddress { get; set; } = string.Empty;

        public string Status { get; set; } = "Pending";

        public bool isCanceled { get; set; } = false;
    }
}
