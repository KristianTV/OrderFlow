namespace OrderFlow.Data.Models
{
    public class TruckOrder
    {
        public Guid OrderID { get; set; }
        public Order Order { get; set; } = null!;

        public Guid TruckID { get; set; }
        public Truck Truck { get; set; } = null!;

        public string DeliverAddress { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }
    }
}