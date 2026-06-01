namespace OrderFlow.Services.Contracts
{
    public interface IRealtimeNotifier
    {
        Task EntityChangedAsync(RealtimeEntityChanged change);

        Task NotificationCountChangedAsync(Guid userId);
    }
}
