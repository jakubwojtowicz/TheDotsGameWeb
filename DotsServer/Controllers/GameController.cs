using DotsWebApi.Model;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
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

    [HttpPost("{id:guid}/make-move")]
    public ActionResult<GameState> MakeMove(string id, Move move)
    {
        _ = _gameService.MakeMove(id, move);
        var aiState = _gameService.MakeAIMove(id);
        return Ok(aiState);
    }
}