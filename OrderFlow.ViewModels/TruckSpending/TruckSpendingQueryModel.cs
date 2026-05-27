namespace OrderFlow.ViewModels.TruckSpending
{
    public class TruckSpendingQueryModel
    {
        public Guid? TruckID { get; set; } = null;

        public Guid? TruckCourseID { get; set; } = null;

        public string? Search { get; set; } = null;

        public string? SortOrder { get; set; } = null;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 12;
    }
}
