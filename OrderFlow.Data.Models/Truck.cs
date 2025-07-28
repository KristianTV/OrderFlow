using Microsoft.AspNet.Identity.EntityFramework;

namespace OrderFlow.Data.Models
{
    public class Truck
    {
        public Guid Id { get; set; }

        public Guid DriverId { get; set; }
        public IdentityUser Driver { get; set; } = null!;

        public string LicensePlate { get; set; } = string.Empty;

        public int Capacity { get; set; } = 0;

        public string Status { get; set; } = "Available";

        public ICollection<TruckOrder> truckOrders { get; set; } = new HashSet<TruckOrder>();

    }
}
