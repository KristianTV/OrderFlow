using Microsoft.AspNetCore.SignalR;

namespace OrderFlow.Hubs
{
    public class MessageHub : Hub
    {
        public async Task JoinNotification(string notificationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, notificationId);
        }

        public async Task LeaveNotification(string notificationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, notificationId);
        }
    }
}
