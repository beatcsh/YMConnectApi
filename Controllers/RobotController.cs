using Microsoft.AspNetCore.Mvc;
using YMConnect;

[Route("[controller]")]
[ApiController]
public class RobotController: ControllerBase
{

    private const string robot_ip = "192.168.1.31";

    private MotomanController OpenMotomanConnection(string ip, out StatusInfo status)
    {
        return MotomanController.OpenConnection(ip, out status);
    }

    private void CloseMotomanConnection(MotomanController controller)
    {
        controller?.CloseConnection();
    }

    [HttpGet("msg/{msg}")]
    public IActionResult SendMessage(string msg)
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.ControlCommands.DisplayStringToPendant(msg);
            CloseMotomanConnection(c);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al mandar mensaje al robot: " + ex.Message);
        }
    }

    [HttpGet("status")]
    public IActionResult GetRobotStatus()
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Status.ReadState(out ControllerStateData stateData);
            CloseMotomanConnection(c);
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
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.ControlCommands.SetServos(SignalStatus.ON);
            status = c.Job.SetActiveJob(nombre, 0);
            status = c.ControlCommands.StartJob();

            CloseMotomanConnection(c);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    [HttpGet("exeJob")]
    public IActionResult GetExecutingData()
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Job.GetExecutingJobInformation(InformTaskNumber.Master, out JobData jobData);
            CloseMotomanConnection(c);
            return Ok(jobData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    [HttpGet("jobStack")]
    public IActionResult GetRobotJobStack()
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Job.GetJobStack(InformTaskNumber.Master, out List<string> jobStack);
            CloseMotomanConnection(c);
            return Ok(jobStack);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    [HttpGet("coordinates")]
    public IActionResult GetCoordinates()
    {
        StatusInfo status;
        var c = OpenMotomanConnection(robot_ip, out status);

        status = c.ControlGroup.ReadPositionData(ControlGroupId.R1, CoordinateType.Pulse, 0, 0, out PositionData positionData);

        CloseMotomanConnection(c);
        return Ok(positionData.AxisData);
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
    }

}
