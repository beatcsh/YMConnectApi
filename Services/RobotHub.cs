using Microsoft.AspNetCore.SignalR;

public class RobotHub : Hub
{
    public async Task SendClientMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveClientMessage", message);
    }
}