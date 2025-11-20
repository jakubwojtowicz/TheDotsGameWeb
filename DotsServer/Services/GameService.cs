using DotsWebApi.Exceptions;
using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
namespace DotsWebApi.Services;

public interface IGameService
{
    string CreateGame(int boardSize, Player startingPlayer = Player.Human);
    GameState GetGameState(string gameId);
    GameState MakeMove(string gameId, Move move);
    GameState MakeAIMove(string gameId);
}

public class GameService : IGameService
{
    private readonly Dictionary<string, GameState> _games = new();
    private readonly IGameEvaluator _gameEvaluator;
    public GameService(IGameEvaluator gameEvaluator)
    {
        _gameEvaluator = gameEvaluator;
    }
    public string CreateGame(int boardSize, Player startingPlayer = Player.Human)
    {
        if(boardSize <= 0 || boardSize > 100)
            throw new InvalidOperationException("Board size must be between 1 and 100.");

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

    public GameState MakeMove(string gameId, Move move)
    {
        if(!_games.ContainsKey(gameId))
            throw new GameNotFoundException();

        var state = _games[gameId];

        if(state.GameEvaluation.IsGameOver)
            throw new InvalidOperationException("The game has already ended.");
        else if(state.CurrentPlayer != Player.Human)
            throw new InvalidOperationException("It's not the human player's turn.");
        else if(state.Board[move.X][move.Y] != Player.None)
            throw new InvalidOperationException("Cell is already occupied.");
        else if(move.X < 0 || move.X >= state.Board.Length || move.Y < 0 || move.Y >= state.Board.Length)
            throw new InvalidOperationException("Move is out of bounds.");

        state.Board[move.X][move.Y] = Player.Human;

        var evaluation = _gameEvaluator.EvaluateGame(state);
        state.GameEvaluation.IsGameOver = evaluation.IsGameOver;
        state.GameEvaluation.Winner = evaluation.Winner;
        state.GameEvaluation.Scores = evaluation.Scores;
        state.CurrentPlayer = Player.AI;

        return state;
    }

    public GameState MakeAIMove(string gameId)
    {
        if(!_games.ContainsKey(gameId))
            throw new GameNotFoundException();

        var state = _games[gameId];

        if(state.GameEvaluation.IsGameOver)
            throw new InvalidOperationException("The game has already ended.");
        else if(state.CurrentPlayer != Player.AI)
            throw new InvalidOperationException("It's not the AI player's turn.");

        for(int r = 0; r < state.Board.Length; r++)
        {
            for(int c = 0; c < state.Board[r].Length; c++)
            {
                if(state.Board[r][c] == Player.None)
                {
                    state.Board[r][c] = Player.AI;
                    var evaluation = _gameEvaluator.EvaluateGame(state);
                    state.GameEvaluation.IsGameOver = evaluation.IsGameOver;
                    state.GameEvaluation.Winner = evaluation.Winner;
                    state.GameEvaluation.Scores = evaluation.Scores;
                    state.CurrentPlayer = Player.Human;
                    return state;
                }
            }
        }
        
        throw new InvalidOperationException("No valid moves left for AI");
    }
}