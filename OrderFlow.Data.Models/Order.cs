using OrderFlow.Data.Models.Enums;

namespace OrderFlow.Data.Models
{
    public class Order
    {
        public Guid OrderID { get; set; } = Guid.NewGuid();

        public Guid UserID { get; set; }
        public ApplicationUser User { get; set; } = null!;

        public DateTime OrderDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public string DeliveryAddress { get; set; } = string.Empty;
        public string PickupAddress { get; set; } = string.Empty;

        public string? DeliveryInstructions { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        
        public bool isCanceled { get; set; } = false;

        public TruckOrder? TruckOrder { get; set; }

        public ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();

        public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
    }
}
