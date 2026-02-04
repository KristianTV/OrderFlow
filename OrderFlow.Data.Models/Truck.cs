using OrderFlow.Data.Models.Enums;

namespace OrderFlow.Data.Models
{
    public class Truck
    {
        public Guid TruckID { get; set; } = Guid.NewGuid();

        public Guid DriverID { get; set; }
        public ApplicationUser Driver { get; set; } = null!;

        public string LicensePlate { get; set; } = string.Empty;

        public double Capacity { get; set; } = 0;

        public TruckStatus Status { get; set; } = TruckStatus.Available;

        public bool IsDeleted { get; set; } = false;

        public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();

        public ICollection<TruckCourse> TruckCourses { get; set; } = new HashSet<TruckCourse>();

        public ICollection<TruckSpending> TruckSpendings { get; set; } = new HashSet<TruckSpending>();
    }
}
