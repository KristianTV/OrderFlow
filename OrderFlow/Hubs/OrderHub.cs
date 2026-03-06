using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace OrderFlow.Hubs
{
    [Authorize]
    public class OrderHub : Hub
    {

    }
}
