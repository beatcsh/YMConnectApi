# 🤖 API SERVICE - YMCONNECT

Esta es una interfaz de comunicacion desarrollada con C# (.NET) para permitir leer datos y ejecutar JOBS en un Robot Yaskawa, se implementa la tecnologia de YMConnect propia de la empresa para agilizar el desarrollo. Este proyecto se integra con el siguiente repositorio https://github.com/beatcsh/pantografo_codes .

---

## 🧩 Tecnologías utilizadas

- ⚙️ .NET 8 (ASP.NET Web API)
- 🔌 TCP/IP (comunicación directa con el robot)
- 🗼 Yaskawa Motoman (YMConnect)

---

## 🚀 Cómo iniciar el proyecto

### 📦 Requisitos

- [.NET SDK 8+](https://dotnet.microsoft.com/download)
- YMConnect (DLL) configurado en el proyecto de C#
- Comunicacion TCP/IP al robot (revisar el manual oficial del controlador en uso para determinar la configuracion TCP/IP)

### 🏃 Iniciar servicio

```PowerShell o CMD
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run
dotnet watch run  ------  En caso de requerir que se inicie con HotReload
