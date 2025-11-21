using DotsWebApi.DTO;
using DotsWebApi.Model;
using DotsWebApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/game")]
public class GameController: ControllerBase
{
    IGameService _gameService;

    public GameController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpPost("new")]
    public ActionResult<GameState> CreateGame([FromQuery(Name = "board-size")] int boardSize)
    {
        var id = _gameService.CreateGame(boardSize);
        return Created($"api/game/{id}", _gameService.GetGameState(id));
    }

    [HttpGet("{id:guid}")]
    public ActionResult<GameState> GetById(string id)
    {
        var state = _gameService.GetGameState(id);
        return Ok(state);
    }

    [HttpPut("{id:guid}/make-move")]
    public ActionResult<GameState> MakeMove(string id, MoveDto move)
    {
        var state = _gameService.MakeMove(id, move);
        return Ok(state);
    }

    [HttpPut("{id:guid}/make-ai-move")]
    public ActionResult<GameState> MakeAIMove(string id)
    {
        var state = _gameService.MakeAIMove(id);
        return Ok(state);
    }
}