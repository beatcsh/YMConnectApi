using Microsoft.AspNetCore.Mvc;
using YMConnect;

[Route("[controller]")]
[ApiController]
public class RobotController: ControllerBase
{
    // constante con la direccion IP del robot (viene predefinida en el sistema Dx200)
    private const string robot_ip = "192.168.1.31";

    // se agrego una funcion que maneja la conexion por medio de la IP, esto se realiza por medio de TCP/IP
    private MotomanController OpenMotomanConnection(string ip, out StatusInfo status)
    {
        return MotomanController.OpenConnection(ip, out status);
    }

    // este metodo se encarga de cerrar la conexion con el robot, aunque primero verifica que le pase un controlador valido
    private void CloseMotomanConnection(MotomanController controller)
    {
        controller?.CloseConnection();
    }

    // este metodo manda un mensaje al robot, el cual se muestra en la pantalla del pendant (es un metodo de depuracion)
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

    // este metodo se encarga de obtener el estado del robot, se devuelve un objeto de tipo ControllerStateData
    [HttpGet("status")]
    public IActionResult GetRobotStatus()
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Status.ReadState(out ControllerStateData stateData);
            /*
            cyclemode

            0 = STEP
            1 = CYCLE
            2 = AUTO

            controlmode

            0 = TEACH
            1 = PLAY
            2 = REMOTE            
            */
            CloseMotomanConnection(c);
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
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Status.ReadSystemInformation(SystemInfoId.R1, out SystemInfoData systemInfoData);

            CloseMotomanConnection(c);
            return Ok(systemInfoData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    /*
    
    Metodo Omitido: La lista no esta tomando los valores que deberia, mostranto una lista vacia hasta ahora
    el metodo de depuracion fue una condicional que evalua el largo de dicha lista, la documentacion no abarca
    el error que se presenta

    [HttpGet("jobList")]
    public IActionResult GetJobList()
    {
        try
        {
            StatusInfo status;

            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Job.GetJobStack(InformTaskNumber.Master, out List<string> jobStack);
            Console.WriteLine(status);

            if (jobStack.Count > 0)
            {
                Console.WriteLine("hay jobs");
            }
            else
            {
                Console.WriteLine("no hay jobs");
            }

            CloseMotomanConnection(c);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }

    }

    */

    // este metodo se encarga de cambiar el trabajo activo, se devuelve el estado del robot [codigo 0 es que todo esta bien]
    [HttpGet("setJob/{nombre}")]
    public IActionResult SetJob(string nombre)
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Job.SetActiveJob(nombre, 0);

            CloseMotomanConnection(c);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    // este metodo obtiene la informacion del JOB que se esta ejecutando en el momento
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

    // este metodo se encarga de iniciar el JOB activo del robot
    [HttpGet("startJob")]
    public IActionResult StartJob()
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.ControlCommands.SetServos(SignalStatus.ON);
            status = c.ControlCommands.StartJob();

            CloseMotomanConnection(c);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    // funcionalidad para detener el robot mientras se esta ejecutando un JOB se detiene solamente apagando servos
    [HttpGet("stopJob")]
    public IActionResult StopJob()
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.ControlCommands.SetServos(SignalStatus.OFF);

            CloseMotomanConnection(c);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    // este metodo nos trae las coordenadas del robot, forzosamente tiene que encontrarse en REMOTE MODE para poder leer sus datos
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