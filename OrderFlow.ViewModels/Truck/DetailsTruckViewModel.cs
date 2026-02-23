namespace OrderFlow.ViewModels.Truck
{
    public class DetailsTruckViewModel
    {
        public Guid TruckID { get; set; }

        public string DriverName { get; set; } = null!;

        public string LicensePlate { get; set; } = string.Empty;

        public double Capacity { get; set; } = 0;

        public string Status { get; set; } = "Available";
    }
}
