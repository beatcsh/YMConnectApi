using Microsoft.AspNetCore.Mvc;
using YMConnect;

[Route("[controller]")]
[ApiController]
public class RobotController: ControllerBase
{

    [HttpGet("msg")]
    public IActionResult SendMessage()
    {
        try
        {

            MotomanController c = MotomanController.OpenConnection("192.168.1.31", out StatusInfo status);

            status = c.ControlCommands.DisplayStringToPendant("comunicacion dada");

            Console.WriteLine(status);

            c.CloseConnection();

            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    [HttpGet("status")]
    public IActionResult GetRobotStatus()
    {
        try
        {

            MotomanController c = MotomanController.OpenConnection("192.168.1.31", out StatusInfo status);

            status = c.Status.ReadState(out ControllerStateData stateData);
            
            Console.WriteLine(stateData);

            c.CloseConnection();

            return Ok(stateData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    [HttpGet("setJob/{nombre}")]
    public IActionResult SetJob(string nombre)
    {
        try
        {

            MotomanController c = MotomanController.OpenConnection("192.168.1.31", out StatusInfo status);

            status = c.Job.SetActiveJob(nombre, 0);

            status = c.ControlCommands.SetServos(SignalStatus.ON);

            status = c.ControlCommands.StartJob();

            status = c.ControlCommands.SetServos(SignalStatus.OFF);

            c.CloseConnection();

            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    [HttpGet("coordinates")]
    public IActionResult GetCoordinates()
    {
        MotomanController c = MotomanController.OpenConnection("192.168.1.31", out StatusInfo status);

        status = c.ControlGroup.ReadPositionData(ControlGroupId.R1, CoordinateType.Pulse, 0, 0, out PositionData positionData);
        Console.WriteLine(positionData);

        /*
         
        EJEMPLO de datos que se regresan en el AxisData

        Se devulve un array en el return donde solo se acede por posicion
 
         AxisData:
            S: 4582 pulse
            L: -55615 pulse
            U: -7 pulse
            R: -7399 pulse
            B: 0 pulse
            T: 0 pulse
            E: 0 pulse
            W: 0 pulse
         
         */

        Console.WriteLine(status);

        c.CloseConnection();

        return Ok(positionData.AxisData);
    }

}
