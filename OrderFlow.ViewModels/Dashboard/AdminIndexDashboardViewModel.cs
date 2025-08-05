namespace OrderFlow.ViewModels.Dashboard
{
    public class AdminIndexDashboardViewModel
    {
        public int TotalActiveOrders { get; set; }
        public int TotalCompletedOrders { get; set; }
        public int TotalCancelledOrders { get; set; }
        public int TotalActiveTrucks { get; set; }
        public int TotalInactiveTrucks { get; set; }
        public int TotalNotifications { get; set; }
        public int TotalActiveNotifications { get; set; }
        public int TotalActiveTruckOrders { get; set; }
        public int TotalCompletedTruckOrders { get; set; }
    }
}
