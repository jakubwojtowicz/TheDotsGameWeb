using DotsServer.Services;
using DotsWebApi.DTO;
using DotsWebApi.Exceptions;
using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services.AI;
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
    private readonly Dictionary<string, GameState> _games = new();
    private readonly IGameRules _gameRules;
    private readonly IAIStrategy _aIStrategy;
    private readonly IMoveApplier _moveApplier;
    public GameService(IGameRules gameRules, IAIStrategy aIStrategy, IMoveApplier moveApplier)
    {
        _gameRules = gameRules;
        _aIStrategy = aIStrategy;
        _moveApplier = moveApplier;
    }
    public string CreateGame(int boardSize, Player startingPlayer = Player.Human)
    {
        string gameId = Guid.NewGuid().ToString();

        _games[gameId] = new GameState(boardSize, startingPlayer);

        return gameId;
    }

    public GameState GetGameState(string gameId)
    {
        if(!_games.ContainsKey(gameId))
            throw new GameNotFoundException();

        return _games[gameId];
    }

    public GameState MakeMove(string gameId, MoveDto moveDto)
    {
        if (!_games.TryGetValue(gameId, out var state))
            throw new GameNotFoundException();

        var move = new Move { Player = Player.Human, X = moveDto.X, Y = moveDto.Y };

        var validation = _gameRules.GetMoveValidation(state, move);

        if (!validation.IsValid)
            throw new InvalidMoveException(validation.Message);

        UpdateGameState(state, move);

        return state;
    }

    public GameState MakeAIMove(string gameId)
    {
        if (!_games.TryGetValue(gameId, out var state))
            throw new GameNotFoundException();

        if (state.IsGameOver)
            throw new InvalidOperationException("Game is already over.");
        if (state.CurrentPlayer != Player.AI)
            throw new InvalidOperationException("It's not AI's turn to play.");

        var move = _aIStrategy.GetNextMove(state);

        UpdateGameState(state, move);

        return state;
    }

    private void UpdateGameState(GameState state, Move move)
    {
        _moveApplier.ApplyMove(state, move);

        var moveResult = _gameRules.GetMoveResult(state, move.Player, move.Player == Player.Human ? Player.AI : Player.Human);

        _moveApplier.CaptureDots(state, moveResult);

        state.IsGameOver = _gameRules.IsGameOver(state);
        state.Winner = _gameRules.GetWinner(state);
        state.CurrentPlayer = _gameRules.GetNextPlayer(state);
        state.LastMoveResult = moveResult;
        state.Scores[move.Player] += moveResult.Score;
        state.LastMove = move;
    }
       
}