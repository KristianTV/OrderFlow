namespace OrderFlow.ViewModels.Truck
{
    public class TruckQueryModel
    {
        public string? Search { get; set; } = null;

        public string? Status { get; set; } = null;

        public string? SortOrder { get; set; } = null;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 12;
    }
}
