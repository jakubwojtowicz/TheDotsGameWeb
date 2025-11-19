using DotsServer.Middleware;
using DotsWebApi.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

builder.Services.AddSingleton<IGameService, GameService>();

var app = builder.Build();

app.UseErrorHandlingMiddleware();

app.MapControllers();

app.Run();
