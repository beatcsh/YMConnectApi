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
    public IActionResult GetActiveAlarms()
    {
        try
        {
            var c = _robotService.OpenConnection(out StatusInfo status);
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
    public IActionResult GetAlarmsHistory()
    {
        try
        {
            var c = _robotService.OpenConnection(out StatusInfo status);
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
    public IActionResult ClearErrors()
    {
        try
        {
            var c = _robotService.OpenConnection(out StatusInfo status);
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

    [HttpGet("readIO")]
    public IActionResult GetIoData()
    {
        try
        {
            var c = _robotService.OpenConnection(out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.IO.ReadBit(81320, out bool value);

            _robotService.CloseConnection(c);
            return Ok(value);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener alarmas del robot: " + ex.Message);
        }
    }

}