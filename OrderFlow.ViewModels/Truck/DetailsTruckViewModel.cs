using OrderFlow.Data.Models;

namespace OrderFlow.ViewModels.Truck
{
    public class DetailsTruckViewModel
    {
        public Guid TruckID { get; set; }

        public string DriverName { get; set; } = null!;

        public string LicensePlate { get; set; } = string.Empty;

        public int Capacity { get; set; } = 0;

        public string Status { get; set; } = "Available";

        public ICollection<TruckOrder> TruckOrders { get; set; } = new HashSet<TruckOrder>();
    }
}
