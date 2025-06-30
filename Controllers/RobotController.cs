using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Mvc;
using YMConnectApi.Services;
using YMConnect;

[Route("[controller]")]
[ApiController]
public class RobotController : ControllerBase
{
    private readonly RobotService _robotService; // se crea el objeto para utilizar el servicio de conexion al robot
    private readonly IHubContext<RobotHub> _hubContext; // de esta forma se inicializa el hub dentro del controlador

    public RobotController(RobotService robotService, IHubContext<RobotHub> hubContext) // este es un constructor de la clase para iniciar el objeto
    {
        _robotService = robotService;
        _hubContext = hubContext;
    }

    [HttpGet("msg")] // este metodo manda un mensaje al robot, el cual se muestra en la pantalla del pendant (es un metodo de prueba)
    public IActionResult SendMessage([FromQuery] string msg, [FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status); // metodo que realiza la conexion al robot
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion"); // se evalua si se pudo generar el objeto

            status = c.ControlCommands.DisplayStringToPendant(msg);

            _robotService.CloseConnection(c); // metodo de cierre de la conexion
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al mandar mensaje al robot: " + ex.Message);
        }
    }

    [HttpGet("status")] // este metodo se encarga de obtener el estado del robot, se devuelve un objeto de tipo ControllerStateData
    public async Task<IActionResult> GetRobotStatus([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.Status.ReadState(out ControllerStateData stateData);

            _robotService.CloseConnection(c);

            // para emitir en tiempo real el estado se usa esto
            await _hubContext.Clients.All.SendAsync("RobotStatusUpdated", stateData);

            return Ok(stateData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    // este metodo se encarga de obtener informacion del robot, se devuelve un objeto de tipo SystemInfoData
    [HttpGet("information")]
    public IActionResult GetRobotData([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);

            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.Status.ReadSystemInformation(SystemInfoId.R1, out SystemInfoData systemInfoData);
            _robotService.CloseConnection(c);

            return Ok(systemInfoData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

}

/*

INFORMACION IMPORTANTE
Los siguientes son los numeros que hacen referencia al ciclo y estado de control del robot:

CYVLE MODE
0 = STEP
1 = CYCLE
2 = AUTO

CONTROL MODE
0 = TEACH
1 = PLAY
2 = REMOTE 

*/