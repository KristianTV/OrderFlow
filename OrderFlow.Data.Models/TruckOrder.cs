namespace OrderFlow.Data.Models
{
    public class TruckOrder
    {
        public Guid OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public Guid TruckId { get; set; }
        public Truck Truck { get; set; } = null!;
    }
}