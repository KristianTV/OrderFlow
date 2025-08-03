namespace OrderFlow.ViewModels.Order
{
    public class DetailsOrderViewModel
    {
        public Guid OrderID { get; set; } = Guid.NewGuid();

        public string UserName { get; set; } = string.Empty;

        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public string DeliveryAddress { get; set; } = string.Empty;

        public string PickupAddress { get; set; } = string.Empty;

        public string? DeliveryInstructions { get; set; }

        public string Status { get; set; } = string.Empty;

        public bool isCanceled { get; set; }

        public ICollection<string?> TrucksLicensePlates { get; set; } = new List<string?>();

        public ICollection<OrderFlow.Data.Models.Payment> Payments { get; set; } = new HashSet<OrderFlow.Data.Models.Payment>();

        public decimal TotalPrice { get; set; }
    }
}
