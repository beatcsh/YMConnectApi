using Microsoft.AspNetCore.Mvc;
using YMConnectApi.Services;
using YMConnect;

[Route("[controller]")]
[ApiController]

public class ProcessController : ControllerBase
{

    private readonly RobotService _robotService; // se crea el objeto para utilizar el servicio de conexion al robot

    public ProcessController(RobotService robotService) // este es un constructor de la clase para iniciar el objeto
    {
        _robotService = robotService;
    }

    // este metodo se encarga de cambiar el trabajo activo, se devuelve el estado del robot [codigo 0 es que todo esta bien]
    [HttpGet("setJob")]
    public IActionResult SetJob([FromQuery] string nombre, [FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.Job.SetActiveJob(nombre, 0);

            _robotService.CloseConnection(c);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    /* por ahora no se usa, quizas mas adelante si
    // este metodo obtiene la informacion del JOB que se esta ejecutando en el momento
    [HttpGet("exeJob")]
    public IActionResult GetExecutingData([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.Job.GetExecutingJobInformation(InformTaskNumber.Master, out JobData jobData);
            _robotService.CloseConnection(c);
            return Ok(jobData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }
    */

    // este metodo se encarga de iniciar el JOB activo del robot
    [HttpGet("startJob")]
    public IActionResult StartJob([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.ControlCommands.SetCycleMode(CycleMode.Cycle);
            status = c.ControlCommands.SetServos(SignalStatus.ON);
            status = c.ControlCommands.StartJob();

            _robotService.CloseConnection(c);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    // funcionalidad para detener el robot mientras se esta ejecutando un JOB se detiene solamente apagando servos
    [HttpGet("stopJob")]
    public IActionResult StopJob([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.ControlCommands.SetServos(SignalStatus.OFF);

            _robotService.CloseConnection(c);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    /* fue el primer metodo que funciono, lo guardo por cariño
    // METODO PARA CAMBIAR EL CICLO, AQUI ES DE PRUEBA NADA MAS
    [HttpGet("changeCycle")]
    public IActionResult SetCycleMode([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);
            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.ControlCommands.SetCycleMode(CycleMode.Automatic);

            _robotService.CloseConnection(c);
            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener alarmas del robot: " + ex.Message);
        }
    }
    */

    [HttpGet("coordinates")] // este metodo nos trae las coordenadas del robot, forzosamente tiene que encontrarse en REMOTE MODE para poder leer sus datos
    public IActionResult GetCoordinates([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);

            if (c == null) return StatusCode(500, "No se pudo establecer una conexion");

            status = c.ControlGroup.ReadPositionData(ControlGroupId.R1, CoordinateType.Pulse, 1, 0, out PositionData positionData);

            Console.WriteLine(positionData);
            _robotService.CloseConnection(c);
            return Ok(positionData.AxisData);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al obtener el estado del robot: " + ex.Message);
        }
    }

    // y me estuve peleando y peleando con esto, pero por fin el robot se mueve con esta funcion, solo nos costo 2 meses y un curso de programacion robotica basica
    [HttpGet("move")]
    public IActionResult MoveRobot([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);

            if (c == null) return StatusCode(500, "No se pudo establcer la conexion");

            PositionData destination = new PositionData();
            PositionData home = new PositionData();

            destination.AxisData = new double[] { -13720.0, 29219.0, -24830.0, 1.0, 0.0, -6.0, 0.0, 0.0 };
            // destination.AxisData = new double[] { -9857.0, 29351.0, -2845.0, 97.0, 0.0, -6.0, 0.0, 0.0 };
            home.AxisData = new double[] { 0.0, 0.0, -0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

            // siempre voy a odiar al roobot Scara que me hizo perder la fé en estas funciones
            JointMotion r1Motion = new JointMotion(ControlGroupId.R1, destination, 15.0, new MotionAccelDecel());
            JointMotion homeMotion = new JointMotion(ControlGroupId.R1, home, 15.0, new MotionAccelDecel());

            status = c.ControlCommands.SetServos(SignalStatus.ON);

            var i = 0;

            // ponganle el contador bien, luego ciclan al robot y van a tener que empezar a rezar
            while (i < 3)
            {
                status = c.MotionManager.AddPointToTrajectory(r1Motion);
                status = c.MotionManager.MotionStart();

                status = c.MotionManager.AddPointToTrajectory(homeMotion);
                status = c.MotionManager.MotionStart();

                i++;
            }

            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al tratar de mover el robot: " + ex.Message);
        }
    }

    // esta funcion, de haber funcionado bien antes, nos habria salvado de poner la constante de home en la conversion de los DxF probablemente
    [HttpGet("home")]
    public IActionResult HomeRobot([FromQuery] string robot_ip)
    {
        try
        {
            var c = _robotService.OpenConnection(robot_ip, out StatusInfo status);

            if (c == null) return StatusCode(500, "No se pudo establcer la conexion");

            PositionData destination = new PositionData();

            destination.AxisData = new double[] { 0.0, 0.0, -0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

            JointMotion r1Motion = new JointMotion(ControlGroupId.R1, destination, 15.0, new MotionAccelDecel());

            status = c.MotionManager.AddPointToTrajectory(r1Motion);

            status = c.ControlCommands.SetServos(SignalStatus.ON);
            status = c.MotionManager.MotionStart();

            return Ok(status);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error al tratar de mover el robot: " + ex.Message);
        }
    }

}

/*

INFORMACION IMPORTANTE

En la funcion de las coordenadas se devuelve un objeto de la clase AxisData que luce como el ejemplo de abajo.

Si quieres generar tu propio movimiento, siguiendo coordenadas, necesitas saber hasta cual maneja tu robot,
ya que AxisData considera incluso rotaciones (es decir, todos los ejes posibles) y robots como el Scara solo
manejan 4 ejes, mientras que el resto de coordenadas son un 0 estatico. Para este robot se quedan 4 ceros al final 
(puntos de rotacion) y se modifican los 4 primeros para indicarle a donde ir, mientras que en un GP25-12 solo quedan
2 ceros fijos al final.

AxisData
    S: 0 pulse
    L: 0 pulse
    U: 0 pulse
    R: 0 pulse
    B: 0 pulse
    T: 0 pulse
    E: 0 pulse
    W: 0 pulse
    
*/