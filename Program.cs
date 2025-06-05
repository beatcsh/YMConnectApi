using YMConnectApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RobotService>(); // registro del servicio para abrir y cerrar la conexion

builder.Services.AddCors(options => // Habilitar CORS globalmente
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    options.AddDefaultPolicy(builder =>
    {
        builder
            .WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("AllowAll"); // configurar CORS antes de los middlewares que lo necesiten

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

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
        |::|           
        |::|          
        |::;
        `:/
*/