using Microsoft.AspNetCore.Mvc;
using YMConnectApi.Services;
using YMConnect;

[Route("[controller]")]
[ApiController]
public class RobotController : ControllerBase
{
    private readonly RobotService _robotService; // se crea el objeto para utilizar el servicio de conexion al robot

    public RobotController(RobotService robotService) // este es un constructor de la clase para iniciar el objeto
    {
        _robotService = robotService;
    }

    [HttpGet("msg/{msg}")] // este metodo manda un mensaje al robot, el cual se muestra en la pantalla del pendant (es un metodo de prueba)
    public IActionResult SendMessage(string msg)
    {
        try
        {
            var c = _robotService.OpenConnection(out StatusInfo status); // metodo que realiza la conexion al robot
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
    public IActionResult GetRobotStatus()
    {
        try
        {
            var c = _robotService.OpenConnection(out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.Status.ReadState(out ControllerStateData stateData);

            _robotService.CloseConnection(c);
            return Ok(stateData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    // este metodo se encarga de obtener informacion del robot, se devuelve un objeto de tipo SystemInfoData
    [HttpGet("information")]
    public IActionResult GetRobotData()
    {
        try
        {
            var c = _robotService.OpenConnection(out StatusInfo status);

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

    [HttpGet("coordinates")] // este metodo nos trae las coordenadas del robot, forzosamente tiene que encontrarse en REMOTE MODE para poder leer sus datos
    public IActionResult GetCoordinates()
    {
        try
        {
            var c = _robotService.OpenConnection(out StatusInfo status);

            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.ControlGroup.ReadPositionData(ControlGroupId.R1, CoordinateType.Pulse, 0, 0, out PositionData positionData);

            Console.WriteLine(positionData);
            _robotService.CloseConnection(c);
            return Ok(positionData.AxisData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

}

/*

INFORMACION IMPORTANTE:
Los siguientes son los numeros que hacen referencia al ciclo y estado de control del robot:

CYVLE MODE
0 = STEP
1 = CYCLE
2 = AUTO

CONTROL MODE
0 = TEACH
1 = PLAY
2 = REMOTE 

En la funcion de las coordenadas se devuelve un objeto de la clase AxisData que luce algo asi:
 
AxisData
    S: 4582 pulse
    L: -55615 pulse
    U: -7 pulse
    R: -7399 pulse
    B: 0 pulse
    T: 0 pulse
    E: 0 pulse
    W: 0 pulse

*/