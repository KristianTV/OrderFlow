namespace OrderFlow.ViewModels.TruckSpending
{
    public class TruckSpendingQueryModel
    {
        public Guid? TruckID { get; set; }

        public Guid? TruckCourseID { get; set; }

        public string? Search { get; set; }

        public string? SortOrder { get; set; }
    }
}
