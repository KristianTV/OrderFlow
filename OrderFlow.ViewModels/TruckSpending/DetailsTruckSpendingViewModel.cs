using OrderFlow.Data.Models.Enums;

namespace OrderFlow.ViewModels.TruckSpending
{
    public class DetailsTruckSpendingViewModel
    {
        public Guid TruckSpendingID { get; set; }

        public Guid? TruckID { get; set; }

        public string TruckLicensePlate { get; set; } = string.Empty;

        public Guid? TruckCourseID { get; set; }

        public string CourseDescription { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public TruckSpendingsType SpendingType { get; set; }

        public string PaymentDescription { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;
    }
}
