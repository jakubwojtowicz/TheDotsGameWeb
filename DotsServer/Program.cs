using DotsWebApi.Middleware;
using DotsWebApi.Services;
using DotsWebApi.Repositories;
using DotsWebApi.Services.AI;
using Serilog;
using DotsWebApi.Services.AI.Heuristics;
using DotsWebApi.Services.StateProcessors;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Controllers
builder.Services.AddControllers();

// Register DI services
builder.Services.AddSingleton<IGameResultProvider, GameResultProvider>();
builder.Services.AddSingleton<IEnclosureDetector, EnclosureDetector>();
builder.Services.AddSingleton<IMoveValidator, MoveValidator>();
builder.Services.AddSingleton<IGameStateProcessor, GameStateProcessor>();
builder.Services.AddSingleton<IMoveGenerator, MoveGenerator>();
builder.Services.AddSingleton<IStateEvaluator, StateEvaluator>();
builder.Services.AddSingleton<IGameStateProcessor, GameStateProcessor>();
builder.Services.AddSingleton<IGameRepository, InMemoryGameRepository>();
builder.Services.AddSingleton<IHeuristic, ScoreHeuristic>();
builder.Services.AddSingleton<IHeuristic, GameOverHeuristic>();
builder.Services.AddSingleton<IHeuristic, TerritoryHeuristic>();
builder.Services.AddSingleton<IHeuristic, EnemyNeighboursHeuristic>();
builder.Services.AddSingleton<IAIStrategy, MinMaxAIStrategy>();
builder.Services.AddSingleton<IGameService, GameService>();

var app = builder.Build();

// Enable Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DotsServer API v1"));
}

// Configure the HTTP request pipeline.
app.UseErrorHandlingMiddleware();
app.UseCors("AllowReact");
app.MapControllers();

app.Run();
