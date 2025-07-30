using System.ComponentModel.DataAnnotations;

namespace OrderFlow.ViewModels.Truck
{
    public class CreateTruckViewModel
    {
        public Guid DriverID { get; set; }

        public string LicensePlate { get; set; } = string.Empty;

        [Range(0,int.MaxValue)]
        public int Capacity { get; set; } = 0;

        public IDictionary<Guid, string>? Drivers { get; set; } = new Dictionary<Guid, string>();
    }
}
