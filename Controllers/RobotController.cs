using Microsoft.AspNetCore.Mvc;
using YMConnect;

[Route("[controller]")]
[ApiController]
public class RobotController : ControllerBase
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

    // METODO DE PRUEBAAA
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

    // METODO DE OBTENCION DE ESTADO EN TIEMPO REAL E INFO
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

    // TOMAR COORDENADAS EN TIEMPO REAL
    // este metodo nos trae las coordenadas del robot, forzosamente tiene que encontrarse en REMOTE MODE para poder leer sus datos
    [HttpGet("coordinates")]
    public IActionResult GetCoordinates()
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.ControlGroup.ReadPositionData(ControlGroupId.R1, CoordinateType.Pulse, 0, 0, out PositionData positionData);

            Console.WriteLine(positionData);
            CloseMotomanConnection(c);
            return Ok(positionData.AxisData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    // ESTOS METODOS PERMITEN MANIPULAR Y VER ARCHIVOS
    [HttpGet("jobList")]
    public IActionResult GetJobList()
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Files.ListFiles(FileType.Job_JBI, out List<string> fileList, true);

            c.CloseConnection();
            return Ok(fileList);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }

    }

    // esta funcion de aqui o metodo como le quieran llamar se utiliza para contar cuantos JOBs se tienen
    [HttpGet("countJobs")]
    public IActionResult GetJobsCount()
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Files.GetFileCount(FileType.Job_JBI, out Int32 count);

            c.CloseConnection();
            return Ok(count);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    // Omar AKA Chupaps quiere este metodo para previsualizar el contenido de los JOBs
    [HttpGet("getStringJob/{nombre}")]
    public IActionResult getStringJob(String nombre)
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Files.SaveFromControllerToString(nombre, out string jobContents);

            c.CloseConnection();
            return Ok(jobContents);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    // este metodo permite enviar los archivos por medio de YMConnect
    [HttpGet("uploadJob/{path}")]
    public IActionResult uploadJob(String path)
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Files.LoadToControllerFromPath(path);

            c.CloseConnection();
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    // este metodo permite borrar el JOB indicando el archivo
    [HttpDelete("deleteJob/{nombre}")]
    public IActionResult deleteJob(String nombre)
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Files.DeleteJobFile(nombre);

            c.CloseConnection();
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    // ESTOS METODOS PERMITEN EJECUTAR DETENER Y MONITOREAR JOBS
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
            status = c.ControlCommands.SetCycleMode(CycleMode.Cycle);
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

    // METODO QUE NOMAS NO QUIERE FUNCIONAR PORQUE NO REGRESA AL ROBOT
    [HttpGet("setInitialPosition")]
    public IActionResult SetInitialPosition()
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            PositionData destination = new PositionData();
            destination.AxisData = new double[] { 0.00, 0.00, 0.00, 0.00, 0.00, 0.00 };
            destination.CoordinateType = CoordinateType.Pulse;

            LinearMotion motion = new LinearMotion(ControlGroupId.R1, destination, 30, new MotionAccelDecel());

            // status = c.MotionManager.AddPointToTrajectory(motion);
            status = c.ControlCommands.SetCycleMode(CycleMode.Cycle);
            status = c.ControlCommands.SetServos(SignalStatus.ON);
            status = c.MotionManager.MotionStart();

            status = c.MotionManager.MotionStart();

            CloseMotomanConnection(c);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    // METODO PARA CAMBIAR EL CICLO, AQUI ES DE PRUEBA NADA MAS
    [HttpGet("changeCycle")]
    public IActionResult SetCycleMode()
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.ControlCommands.SetCycleMode(CycleMode.Automatic);

            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener alarmas del robot: " + ex.Message);
        }
    }

    // METODOS DE LAS ALARMAS SON CRUCIALES PORQUE SON PA TODA UNA PANTALLA AUNQUE EL DEL HISTORIAL ESTA MEDIO INNECESARIO
    [HttpGet("activeAlarms")] // este endpoint devuelve las alarmas activas del robot se imprime en consola por la depuracion, pero se omitira en la version final
    public IActionResult GetActiveAlarms()
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Faults.GetActiveAlarms(out ActiveAlarms alarms);
            Console.WriteLine(alarms);

            return Ok(alarms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener alarmas del robot: " + ex.Message);
        }
    }

    // esto te trae todo el historial de alarmas, si se limpian pos no esperes recibir nada
    [HttpGet("getAlarmsHistory")]
    public IActionResult GetAlarmsHistory()
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Faults.GetAlarmHistory(AlarmCategory.Major, 5, out AlarmHistory alarmHistoryData);
            Console.WriteLine(alarmHistoryData);

            return Ok(alarmHistoryData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener alarmas del robot: " + ex.Message);
        }
    }

    // esto de abajo cuenta como el reset, saludos a los fans de Penta el Zero Miedo
    [HttpGet("clearErrors")] // se limpian los errores 
    public IActionResult clearErrors()
    {
        try
        {
            StatusInfo status;
            var c = OpenMotomanConnection(robot_ip, out status);

            status = c.Faults.ClearAllFaults();

            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener alarmas del robot: " + ex.Message);
        }
    }
    // FIN XD
}

/*
         _
        (:)_
      ,'    `.
     :        :
     |        |              ___
     |       /|    ______   // _\
     ; -  _,' :  ,'      `. \\  -\
    /          \/          \ \\  :
   (            :  ------.  `-'  |
____\___    ____|______   \______|___________
        |::|           '--`           
        |::|
        |::|      Duermo fuera de casa,     
        |::|          como Snoopy
        |::;
        `:/

*/