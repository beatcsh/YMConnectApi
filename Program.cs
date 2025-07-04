using YMConnectApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<RobotService>();
builder.Services.AddHostedService<RobotBackgroundService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactClient", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // importante para credenciales
    });
});

builder.Services.AddSignalR();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors("AllowReactClient");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHub<RobotHub>("/robotHub");

app.Run();
