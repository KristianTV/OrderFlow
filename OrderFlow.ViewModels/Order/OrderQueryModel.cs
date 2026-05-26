namespace OrderFlow.ViewModels.Order
{
    public class OrderQueryModel
    {
        public bool HideCompleted { get; set; } = true;

        public string? SearchId { get; set; } = null;

        public string? StatusFilter { get; set; } = null;

        public string? SortOrder { get; set; } = "date_desc";

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 12;
    }
}
