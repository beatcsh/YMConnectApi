using Microsoft.AspNetCore.Mvc;
using YMConnect;

namespace YMConnectApi.Services
{
    public class RobotService
    {

        // se agrego una funcion que maneja la conexion por medio de la IP, esto se realiza por medio de TCP/IP
        public MotomanController OpenConnection(string robot_ip, out StatusInfo status)
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