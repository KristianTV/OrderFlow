namespace OrderFlow.ViewModels.Truck
{
    public class OrderViewModel
    {
        public Guid OrderID { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public string PickupAddress { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public bool IsSelected { get; set; } = false;
    }
}