namespace OrderFlow.Services
{
    public interface IRealtimeNotifier
    {
        Task EntityChangedAsync(RealtimeEntityChanged change);

        Task NotificationCountChangedAsync(Guid userId);
    }
}
