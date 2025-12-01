using DotsWebApi.Middleware;
using DotsWebApi.Services;
using DotsWebApi.Repositories;
using DotsWebApi.Services.AI;
using Serilog;
using DotsWebApi.Services.AI.Heuristics;
using DotsWebApi.DTO;
using DotsWebApi.Services.GameEngine;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DI services
builder.Services.AddSingleton<IGameResultProvider, GameResultProvider>();
builder.Services.AddSingleton<IEnclosureDetector, EnclosureDetector>();
builder.Services.AddSingleton<IMoveGenerator, MoveGenerator>();
builder.Services.AddSingleton<IStateEvaluator, StateEvaluator>();
builder.Services.AddSingleton<IGameEngine, GameEngine>();
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

//Minimal api endpoints

var gameApi = app.MapGroup("/api/game");

gameApi.MapPost("/new", (int boardSize, IGameService gameService) =>
{
    var id = gameService.CreateGame(boardSize);
    var state = gameService.GetGameState(id);
    return Results.Created($"/api/game/{id}", state);
});

gameApi.MapGet("/{id:guid}", (string id, IGameService gameService) =>
{
    var state = gameService.GetGameState(id);
    return Results.Ok(state);
});

gameApi.MapPut("/{id:guid}/make-move",
    async (string id, MoveDto move, IGameService gameService) =>
{
    var state = await gameService.MakeMoveAsync(id, move);
    return Results.Ok(state);
});

gameApi.MapPut("/{id:guid}/make-ai-move",
    async (string id, IGameService gameService) =>
{
    var state = await gameService.MakeAIMoveAsync(id);
    return Results.Ok(state);
});

// Configure the HTTP request pipeline.
app.UseErrorHandlingMiddleware();

app.Run();
