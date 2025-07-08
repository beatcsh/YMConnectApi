# 🤖 API SERVICE - YMCONNECT

Esta es una interfaz de comunicacion desarrollada con C# (.NET) para permitir leer datos y ejecutar JOBS en un Robot Yaskawa, se implementa la tecnologia de YMConnect propia de la empresa para agilizar el desarrollo. Este proyecto se integra con el siguiente repositorio https://github.com/beatcsh/pantografo_codes .

Para un mejor rendimiendo del proyecto se recomienda que sea usado con el robot en estado Remoto, ya que los tiempos de respuesta se ven reducidos enormemente y es mas factible para no presentar errores en alguno de los procesos.

---

## 🧩 Tecnologías utilizadas

- ⚙️ .NET 8 (ASP.NET Web API)
- 🔌 TCP/IP (comunicación directa con el robot)
- 🗼 YMConnect v1.1.3 (https://github.com/Yaskawa-Global/YMConnect/releases/tag/v1.1.3)
- 🤖 Robot industrial Yaskawa (hasta ahora probado con controladoress Dx200 y YRC-1000 Micro)

---

## 🚀 Cómo iniciar el proyecto

### 📦 Requisitos

- [.NET SDK 8+](https://dotnet.microsoft.com/download)
- YMConnect (DLL) configurado en el proyecto de C# (https://developer.motoman.com/en/YMConnect/CSharpWindows)
- Comunicacion TCP/IP al robot (revisar el manual oficial del controlador en uso para determinar la configuracion TCP/IP)

### 🏃 Iniciar servicio

```PowerShell o CMD
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run
dotnet watch run  ------  En caso de requerir que se inicie con HotReload
dotnet watch --launch-profile http  -----  Este comando se recomienda en caso de errores de comunicacion 
                                           al ejecutar la API.