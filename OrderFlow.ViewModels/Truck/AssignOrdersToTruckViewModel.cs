namespace OrderFlow.ViewModels.Truck
{
    public class AssignOrdersToTruckViewModel
    {
        public string LicensePlate { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public List<OrderViewModel> Orders { get; set; } = new List<OrderViewModel>();
    }
}
