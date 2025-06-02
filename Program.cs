using YMConnectApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RobotService>();

// Habilitar CORS globalmente
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar CORS antes de los middlewares que lo necesiten
app.UseCors("AllowAll");

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