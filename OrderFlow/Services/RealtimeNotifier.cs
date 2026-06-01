using Microsoft.AspNetCore.SignalR;
using OrderFlow.Hubs;
using OrderFlow.Services.Contracts;
using OrderFlow.Services.Core.Contracts;

namespace OrderFlow.Services
{
    public class RealtimeNotifier : IRealtimeNotifier
    {
        private readonly IHubContext<AppHub> _hubContext;
        private readonly INotificationService _notificationService;

        public RealtimeNotifier(IHubContext<AppHub> hubContext, INotificationService notificationService)
        {
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        public async Task EntityChangedAsync(RealtimeEntityChanged change)
        {
            var payload = new
            {
                entity = change.Entity,
                action = change.Action,
                id = change.Id?.ToString(),
                relatedId = change.RelatedId?.ToString()
            };

            foreach (string role in change.Roles.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct())
            {
                await _hubContext.Clients.Group(RealtimeGroups.Role(role)).SendAsync("EntityChanged", payload);
            }

            foreach (Guid userId in change.UserIds.Where(id => id != Guid.Empty).Distinct())
            {
                await _hubContext.Clients.Group(RealtimeGroups.User(userId.ToString())).SendAsync("EntityChanged", payload);
            }
        }

        public async Task NotificationCountChangedAsync(Guid userId)
        {
            int unreadCount = await _notificationService.GetUnreadCountAsync(userId);

            await _hubContext.Clients
                .Group(RealtimeGroups.User(userId.ToString()))
                .SendAsync("NotificationCountChanged", new
                {
                    count = unreadCount,
                    display = unreadCount > 99 ? "99+" : unreadCount.ToString()
                });
        }
    }
}
