namespace OrderFlow.ViewModels
{
    public class CreateOrderViewModel
    {
        public string DeliveryAddress { get; set; } = string.Empty;
        public string PickupAddress { get; set; } = string.Empty;
        public string? DeliveryInstructions { get; set; }
    }
}
