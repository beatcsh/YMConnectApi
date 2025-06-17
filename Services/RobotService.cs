using System.Net.NetworkInformation;
using YMConnect;

namespace YMConnectApi.Services
{
    public class RobotService
    {

        // funcion para manejar que una ip este disponible, realiza un ping de 5 segundos y si no obtiene respuesta no devuelve nada
        private bool IsHostReachable(string ip, int timeoutMs = 5000)
        {
            try
            {
                using var ping = new Ping(); // se crea un objeto de Ping 
                var reply = ping.Send(ip, timeoutMs); // se manda y se almacena el resultado
                return reply.Status == IPStatus.Success; // se envia el estado de la conexion
            }
            catch
            {
                return false; // por si no existe se manda esto
            }
        }

        // se agrego una funcion que maneja la conexion por medio de la IP, esto se realiza por medio de TCP/IP
        public MotomanController? OpenConnection(string robot_ip, out StatusInfo status)
        {
            status = new StatusInfo();

            if (!IsHostReachable(robot_ip)) // checamos que la ip dada si exista y este disponible en la red local
            {
                return null;
            }

            return MotomanController.OpenConnection(robot_ip, out status);
        }

        // este metodo se encarga de cerrar la conexion con el robot, aunque primero verifica que le pase un controlador valido
        public void CloseConnection(MotomanController c)
        {
            c?.CloseConnection();
        }

    }
}