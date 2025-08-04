using OrderFlow.Data.Models.Enums;

namespace OrderFlow.Data.Models
{
    public class Truck
    {
        public Guid TruckID { get; set; }

        public Guid DriverID { get; set; }
        public ApplicationUser Driver { get; set; } = null!;

        public string LicensePlate { get; set; } = string.Empty;

        public int Capacity { get; set; } = 0;

        public TruckStatus Status { get; set; } = TruckStatus.Available;

        public ICollection<TruckOrder> TruckOrders { get; set; } = new HashSet<TruckOrder>();

        public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();

        public bool isDeleted { get; set; } = false;
    }
}
