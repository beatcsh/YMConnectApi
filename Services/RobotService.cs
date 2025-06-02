using Microsoft.AspNetCore.Mvc;
using YMConnect;

namespace YMConnectApi.Services
{
    public class RobotService
    {

        // constante con la direccion IP del robot (viene predefinida en el sistema Dx200)
        private const string robot_ip = "192.168.1.31";

        // se agrego una funcion que maneja la conexion por medio de la IP, esto se realiza por medio de TCP/IP
        public MotomanController OpenConnection(out StatusInfo status)
        {
            return MotomanController.OpenConnection(robot_ip, out status);
        }

        // este metodo se encarga de cerrar la conexion con el robot, aunque primero verifica que le pase un controlador valido
        public void CloseConnection(MotomanController c)
        {
            c?.CloseConnection();
        }

    }
}