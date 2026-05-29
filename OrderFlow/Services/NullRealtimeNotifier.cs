namespace OrderFlow.Services
{
    public class NullRealtimeNotifier : IRealtimeNotifier
    {
        public static readonly NullRealtimeNotifier Instance = new NullRealtimeNotifier();

        private NullRealtimeNotifier()
        {
        }

        public Task EntityChangedAsync(RealtimeEntityChanged change)
        {
            return Task.CompletedTask;
        }

        public Task NotificationCountChangedAsync(Guid userId)
        {
            return Task.CompletedTask;
        }
    }
}
