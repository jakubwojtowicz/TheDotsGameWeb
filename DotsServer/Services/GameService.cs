using DotsWebApi.DTO;
using DotsWebApi.Exceptions;
using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services.AI;
using DotsWebApi.Repositories;
using DotsWebApi.Services.GameEngine;
namespace DotsWebApi.Services;

public interface IGameService
{
    string CreateGame(int boardSize, Player startingPlayer = Player.Human);
    GameState GetGameState(string gameId);
    Task<GameState> MakeMoveAsync(string gameId, MoveDto moveDto);
    Task<GameState> MakeAIMoveAsync(string gameId);
}

public class GameService : IGameService
{
    private readonly IGameRepository _gameRepository;
    private readonly IGameEngine _gameEngine;
    private readonly IAIStrategy _aIStrategy;
    public GameService(IGameEngine gameEngine, 
        IAIStrategy aIStrategy, 
        IGameRepository gameRepository)
    {
        _gameEngine = gameEngine;
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

    public async Task<GameState> MakeMoveAsync(string gameId, MoveDto moveDto)
    {
        var state = _gameRepository.Get(gameId);

        if (state == null)
            throw new GameNotFoundException();

        var move = new Move { Player = Player.Human, X = moveDto.X, Y = moveDto.Y };

        var validation = _gameEngine.ValidateMove(state, move);

        if (!validation.IsValid)
            throw new InvalidMoveException(validation.Message);

        var newState = await Task.Run(() =>  
            _gameEngine.ApplyMove(state, move));

        _gameRepository.Update(gameId, newState);

        return await Task.FromResult(newState);
    }

    public async Task<GameState> MakeAIMoveAsync(string gameId)
    {
        var state = _gameRepository.Get(gameId);

        if (state == null)
            throw new GameNotFoundException();

        if (state.IsGameOver)
            throw new InvalidOperationException("Game is already over.");
        if (state.CurrentPlayer != Player.AI)
            throw new InvalidOperationException("It's not AI's turn to play.");

        var newState = await Task.Run(() =>
        {
            var move = _aIStrategy.GetNextMove(state);
            return _gameEngine.ApplyMove(state, move);
        });

        _gameRepository.Update(gameId, newState);

        return newState;
    }
}