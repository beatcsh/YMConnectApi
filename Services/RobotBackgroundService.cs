using Microsoft.AspNetCore.SignalR;
using YMConnectApi.Services;
using YMConnect;

public class RobotBackgroundService : BackgroundService
{
    private readonly IHubContext<RobotHub> _hubContext;
    private readonly RobotService _robotService;

    public RobotBackgroundService(IHubContext<RobotHub> hubContext, RobotService robotService)
    {
        _hubContext = hubContext;
        _robotService = robotService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var connections = RobotHub.GetConnectionIps();

            // agrupamos por IP para no consultar IP repetida varias veces
            var ips = connections.Values.Distinct().ToList();

            foreach (var ip in ips)
            {
                try
                {
                    var ioCodes = new Dictionary<uint, string>
                    {
                        { 10020, "torch" },
                        { 80026, "pendantStop" },
                        { 80025, "externalStop" },
                        { 80027, "doorEmergencyStop" },
                        { 80013, "teachMode" },
                        { 80012, "playMode" },
                        { 80011, "remoteMode" },
                        { 80015, "hold" },
                        { 80016, "start" },
                        { 80017, "servosReady" }
                    };
                    var resultsIO = new Dictionary<string, bool>();

                    var c = _robotService.OpenConnection(ip, out var status);

                    if (c != null)
                    {
                        status = c.Status.ReadState(out ControllerStateData data);
                        foreach (var kvp in ioCodes)
                        {
                            status = c.IO.ReadBit(kvp.Key, out bool value);
                            resultsIO[kvp.Value] = value;
                        }

                         _robotService.CloseConnection(c);

                        // Envia solo a las conexiones que tienen esa IP
                        var clientsForIp = connections.Where(kvp => kvp.Value == ip).Select(kvp => kvp.Key);

                        foreach (var clientId in clientsForIp)
                        {
                            await _hubContext.Clients.Client(clientId).SendAsync("RobotStatusUpdated", data);
                            await _hubContext.Clients.Client(clientId).SendAsync("RobotDiagnostic", resultsIO);
                        }
                    }
                }
                catch
                {
                    // no se como manejar la excepcion todavia xd
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
        }
    }
}
