using Microsoft.AspNetCore.SignalR;
using YMConnectApi.Services;
using YMConnect;

public class RobotBackgroundService : BackgroundService
{
    private readonly IHubContext<RobotHub> _hubContext;
    private readonly RobotService _robotService;

    // este es el constructor de la clase que se ejecuta en segundo plano
    public RobotBackgroundService(IHubContext<RobotHub> hubContext, RobotService robotService)
    {
        _hubContext = hubContext;
        _robotService = robotService;
    }

    // esta es la funcion que se va a ejecutar de forma constante, la idea es que haya un retraso de solo un segundo
    // el override aqui jala para evitar comportamientos no deseados
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // mientras el tolen sea valido se va a ejecutar todo lo demas
        while (!stoppingToken.IsCancellationRequested)
        {
            var connections = RobotHub.GetConnectionIps();

            // agrupamos por IP para no consultar IP repetida varias veces
            var ips = connections.Values.Distinct().ToList();

            foreach (var ip in ips) // aqui ciclamos todas las ip, siempre va a ser una entonces no hay falla
            {
                try
                {
                    // esto es un diccionario con los codigos de los IO y su nombre para irlos agrupando
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
                    var resultsIO = new Dictionary<string, bool>(); // en este almacenamos los resultados de la lectura mas adelante

                    var c = _robotService.OpenConnection(ip, out var status); // abrimos una conexion

                    if (c != null)
                    {
                        status = c.Status.ReadState(out ControllerStateData statusData); // leemos el estado del robot

                        foreach (var kvp in ioCodes) // ciclamos todas las señales IO para obtenerlas todas
                        {
                            status = c.IO.ReadBit(kvp.Key, out bool value); // leemos la señal
                            resultsIO[kvp.Value] = value; // almacenamos el resultado
                        }

                        status = c.Faults.GetActiveAlarms(out ActiveAlarms alarms);

                        _robotService.CloseConnection(c); // se cierra la conexion

                        // envia solo a las conexiones que tienen esa IP
                        var clientsForIp = connections.Where(kvp => kvp.Value == ip).Select(kvp => kvp.Key);

                        foreach (var clientId in clientsForIp) // para cada cliente conectado con su respectivo ID le mandamos por notificacion push los resultados obtenidos con esta funcion
                        {
                            await _hubContext.Clients.Client(clientId).SendAsync("RobotStatusUpdated", statusData);
                            await _hubContext.Clients.Client(clientId).SendAsync("RobotDiagnostic", resultsIO);
                            await _hubContext.Clients.Client(clientId).SendAsync("ActiveAlarms", alarms);
                        }
                    }
                }
                catch
                {
                    // no se como manejar la excepcion todavia xd
                    Console.WriteLine("we have an issue");
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken); // se repetira este proceso cada 2 segundos, esto nos permitra tener lo mas cerca que se pueda el estado en tiempo real
        }
    }
}
