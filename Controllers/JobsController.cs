using Microsoft.AspNetCore.Mvc;
using YMConnectApi.Services;
using YMConnect;
using System;
using System.IO;

[Route("[controller]")]
[ApiController]

public class JobsController : ControllerBase
{

    private readonly RobotService _robotService; // se crea el objeto para utilizar el servicio de conexion al robot

    public JobsController(RobotService robotService) // este es un constructor de la clase para iniciar el objeto
    {
        _robotService = robotService;
    }

    [HttpGet("jobList")] // trae la lista de archivos, por configuracion del robot sule devolver un archivo vacio, se soluciona omitiendo valores nulos
    public IActionResult GetJobList([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.Files.ListFiles(FileType.Job_JBI, out List<string> fileList, true);

            _robotService.CloseConnection(c);
            return Ok(fileList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }

    }

    [HttpGet("getStringJob")] // Omar AKA Chupaps quiere este metodo para previsualizar el contenido de los JOBs
    public IActionResult getStringJob([FromQuery] string nombre, [FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.Files.SaveFromControllerToString(nombre, out string jobContents);

            _robotService.CloseConnection(c);
            return Ok(new { content = jobContents });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    /* se podra usar quizas por eso no la borro
    [HttpDelete("deleteJob")] // este metodo permite borrar el JOB indicando el archivo
    public IActionResult deleteJob([FromQuery] String nombre, [FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.Files.DeleteJobFile(nombre);

            _robotService.CloseConnection(c);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }
    */

}