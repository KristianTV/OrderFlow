namespace OrderFlow.ViewModels.Truck
{
    public class TruckOrderVewModel
    {
        public Guid OrderId { get; set; }

        public string DeliverAddress { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; }
        public DateTime? DeliveryDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}
