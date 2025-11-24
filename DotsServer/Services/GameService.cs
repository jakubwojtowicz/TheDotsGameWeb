using DotsWebApi.DTO;
using DotsWebApi.Exceptions;
using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services.AI;
using DotsWebApi.Repositories;
namespace DotsWebApi.Services;

public interface IGameService
{
    string CreateGame(int boardSize, Player startingPlayer = Player.Human);
    GameState GetGameState(string gameId);
    GameState MakeMove(string gameId, MoveDto moveDto);
    GameState MakeAIMove(string gameId);
}

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameRules _gameRules;
    private readonly IAIStrategy _aIStrategy;
    public GameService(IGameRules gameRules, IAIStrategy aIStrategy, IGameRepository gameRepository)
    {
        _gameRules = gameRules;
        _aIStrategy = aIStrategy;
        _gameRepository = gameRepository;
    }
    public string CreateGame(int boardSize, Player startingPlayer = Player.Human)
    {
        var gameId = Guid.NewGuid().ToString();
        _gameRepository.Add(gameId, new GameState(boardSize, startingPlayer));
        return gameId;
    }

    public GameState GetGameState(string gameId)
    {
        var state = _gameRepository.Get(gameId);
        
        if (state == null)
            throw new GameNotFoundException();

        return state;
    }

    public GameState MakeMove(string gameId, MoveDto moveDto)
    {
        var state = _gameRepository.Get(gameId);

        if (state == null)
            throw new GameNotFoundException();

        var move = new Move { Player = Player.Human, X = moveDto.X, Y = moveDto.Y };

        var validation = _gameRules.GetMoveValidation(state, move);

        if (!validation.IsValid)
            throw new InvalidMoveException(validation.Message);

        var newState = _gameRules.ApplyMove(state, move);
        _gameRepository.Update(gameId, newState);

        return newState;
    }

    public GameState MakeAIMove(string gameId)
    {
        var state = _gameRepository.Get(gameId);

        if (state == null)
            throw new GameNotFoundException();

        if (state.IsGameOver)
            throw new InvalidOperationException("Game is already over.");
        if (state.CurrentPlayer != Player.AI)
            throw new InvalidOperationException("It's not AI's turn to play.");

        var move = _aIStrategy.GetNextMove(state);

        var newState = _gameRules.ApplyMove(state, move);
        _gameRepository.Update(gameId, newState);

        return newState;
    }
}