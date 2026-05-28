namespace OrderFlow.ViewModels.Notification
{
    public class NotificationQueryModel
    {
        public string? SortBy { get; set; } = "all";

        public bool HideSystemNotifications { get; set; } = false;

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 12;
    }
}
