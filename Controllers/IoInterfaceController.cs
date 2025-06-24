using Microsoft.AspNetCore.Mvc;
using YMConnectApi.Services;
using YMConnect;

[Route("[controller]")]
[ApiController]

public class IoInterfaceController : ControllerBase
{
    private readonly RobotService _robotService;

    public IoInterfaceController(RobotService robotService)
    {
        _robotService = robotService;
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
    public IActionResult WriteIo([FromQuery] string robot_ip, [FromQuery] bool value)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.IO.WriteBit(10010, value);

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