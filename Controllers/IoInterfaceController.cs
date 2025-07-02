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

}