using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace OrderFlow.Hubs
{
    [Authorize]
    public class AppHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            string? userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrWhiteSpace(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, RealtimeGroups.User(userId));
            }

            foreach (Claim roleClaim in Context.User?.FindAll(ClaimTypes.Role) ?? Enumerable.Empty<Claim>())
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, RealtimeGroups.Role(roleClaim.Value));
            }

            await base.OnConnectedAsync();
        }
    }
}
