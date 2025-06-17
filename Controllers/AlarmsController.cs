using Microsoft.AspNetCore.Mvc;
using YMConnectApi.Services;
using YMConnect;

[Route("[controller]")]
[ApiController]

public class AlarmsController : ControllerBase
{

    private readonly RobotService _robotService; // se crea el objeto para utilizar el servicio de conexion al robot

    public AlarmsController(RobotService robotService) // este es un constructor de la clase para iniciar el objeto
    {
        _robotService = robotService;
    }

    [HttpGet("activeAlarms")] // este endpoint devuelve las alarmas activas del robot se imprime en consola por la depuracion, pero se omitira en la version final
    public IActionResult GetActiveAlarms([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.Faults.GetActiveAlarms(out ActiveAlarms alarms);
            _robotService.CloseConnection(c);
            return Ok(alarms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener alarmas del robot: " + ex.Message);
        }
    }

    [HttpGet("getAlarmsHistory")] // esto te trae todo el historial de alarmas, si se limpian pos no esperes recibir nada
    public IActionResult GetAlarmsHistory([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.Files.SaveFromControllerToString("ALMHIST.DAT", out string almHistory);

            _robotService.CloseConnection(c);
            return Ok(almHistory);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener alarmas del robot: " + ex.Message);
        }
    }

    [HttpGet("clearErrors")] // esto cuenta como el reset, saludos a los fans de Penta el Zero Miedo
    public IActionResult ClearErrors([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.Faults.ClearAllFaults();
            _robotService.CloseConnection(c);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener alarmas del robot: " + ex.Message);
        }
    }

    [HttpGet("readSpecificIO")]
    public IActionResult GetIoData([FromQuery] uint code, [FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.IO.ReadBit(code, out bool value);

            _robotService.CloseConnection(c);
            return Ok(value);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener alarmas del robot: " + ex.Message);
        }
    }

    [HttpGet("writeIO")]
    public IActionResult WriteIo([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.IO.WriteBit(10010, false);

            _robotService.CloseConnection(c);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener alarmas del robot: " + ex.Message);
        }
    }

    [HttpGet("readIO")]
    public IActionResult GetMultipleIoData([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexión");

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

            var results = new Dictionary<string, bool>();

            foreach (var kvp in ioCodes)
            {
                status = c.IO.ReadBit(kvp.Key, out bool value);
                results[kvp.Value] = value;
            }

            _robotService.CloseConnection(c);
            return Ok(results);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener los estados IO del robot: " + ex.Message);
        }
    }


}