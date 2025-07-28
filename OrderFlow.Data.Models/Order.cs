using Microsoft.AspNet.Identity.EntityFramework;

namespace OrderFlow.Data.Models
{
    public class Order
    {
        public Guid OrderID { get; set; } = Guid.NewGuid();

        public Guid UserID { get; set; }
        public IdentityUser User { get; set; } = null!;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime DeliveryDate { get; set; } = DateTime.UtcNow;

        public string DeliveryAddress { get; set; } = string.Empty;
        public string PickupAddress { get; set; } = string.Empty;

        public string? DeliveryInstructions { get; set; }

        public string Status { get; set; } = "Pending";
        
        public bool isCanceled { get; set; } = false;

        public ICollection<TruckOrder> truckOrders { get; set; } = new HashSet<TruckOrder>();
    }
}
