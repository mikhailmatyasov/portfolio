using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WeSafe.Web.Core.Hubs
{
    [Authorize]
    public class EventsHub : Hub
    {
    }
}