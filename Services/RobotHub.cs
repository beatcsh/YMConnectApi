using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class RobotHub : Hub
{
    // guarda las ips que se mandaron
    private static ConcurrentDictionary<string, string> connectionIps = new();

    public Task SetRobotIp(string ip)
    {
        connectionIps[Context.ConnectionId] = ip;
        return Task.CompletedTask;
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        connectionIps.TryRemove(Context.ConnectionId, out _);
        return base.OnDisconnectedAsync(exception);
    }

    // método para que el BackgroundService pueda obtener el diccionario
    public static ConcurrentDictionary<string, string> GetConnectionIps() => connectionIps;
}
