using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace F1R.core.hubs;

public class LiveHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Console.WriteLine($"[SignalR] Client connected: {Context.ConnectionId}");
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"[SignalR] Client disconnected: {Context.ConnectionId}");
        return base.OnDisconnectedAsync(exception);
    }
}