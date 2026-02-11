namespace OrderFlow.Services.Core.Commands
{
    public class NotificationCommand
    {
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public Guid ReceiverID { get; set; }
        public Guid? OrderID { get; set; }
        public Guid? CourseID { get; set; }
        public Guid? TruckID { get; set; }
        public bool CanRespond { get; set; } = false;
    }
}
