using Microsoft.AspNetCore.Identity;

namespace OrderFlow.Data.Models
{
    public class Truck
    {
        public Guid Id { get; set; }

        public Guid DriverID { get; set; }
        public ApplicationUser Driver { get; set; } = null!;

        public string LicensePlate { get; set; } = string.Empty;

        public int Capacity { get; set; } = 0;

        public string Status { get; set; } = "Available";

        public ICollection<TruckOrder> TruckOrders { get; set; } = new HashSet<TruckOrder>();

        public bool isDeleted { get; set; } = false;
    }
}
