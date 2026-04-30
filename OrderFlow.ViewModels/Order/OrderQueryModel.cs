namespace OrderFlow.ViewModels.Order
{
    public class OrderQueryModel
    {
        public bool HideCompleted { get; set; }

        public string? SearchId { get; set; }

        public string? StatusFilter { get; set; }

        public string? SortOrder { get; set; }
    }
}
