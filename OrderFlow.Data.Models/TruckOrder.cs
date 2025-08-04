using OrderFlow.Data.Models.Enums;

namespace OrderFlow.Data.Models
{
    public class TruckOrder
    {
        public Guid TruckOrderId { get; set; } = Guid.NewGuid();

        public Guid OrderID { get; set; }
        public Order Order { get; set; } = null!;

        public Guid TruckID { get; set; }
        public Truck Truck { get; set; } = null!;

        public string DeliverAddress { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public TruckOrderStatus Status { get; set; } = TruckOrderStatus.Assigned;
    }
}