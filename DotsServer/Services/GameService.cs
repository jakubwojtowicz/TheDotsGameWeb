using DotsWebApi.Exceptions;
using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services;

public interface IGameService
{
    string CreateGame(int boardSize);
    GameState GetGameState(string gameId);
    GameState MakeMove(string gameId, Move move);
    GameState MakeAIMove(string gameId);
}

public class GameService : IGameService
{
    private readonly Dictionary<string, GameState> _games = new();
    public string CreateGame(int boardSize)
    {
        string gameId = Guid.NewGuid().ToString();
        _games[gameId] = new GameState(boardSize);

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

        if(move.X < 0 || move.X >= state.Board.Length || move.Y < 0 || move.Y >= state.Board.Length)
            throw new InvalidOperationException("Move is out of bounds.");
        else if(state.Board[move.X][move.Y] != Player.None)
            throw new InvalidOperationException("Cell is already occupied.");
        else if(state.CurrentPlayer != Player.Human)
            throw new InvalidOperationException("It's not the human player's turn.");
        else if(state.Result != GameResult.Ongoing)
            throw new InvalidOperationException("The game has already ended.");

        state.Board[move.X][move.Y] = Player.Human;

        state.CurrentPlayer = Player.AI;

        return state;
    }

    public GameState MakeAIMove(string gameId)
    {
        if(!_games.ContainsKey(gameId))
            throw new GameNotFoundException();

        var state = _games[gameId];

        if(state.CurrentPlayer != Player.AI)
            throw new InvalidOperationException("It's not the AI player's turn.");
        else if(state.Result != GameResult.Ongoing)
            throw new InvalidOperationException("The game has already ended.");

        for(int r = 0; r < state.Board.Length; r++)
        {
            for(int c = 0; c < state.Board[r].Length; c++)
            {
                if(state.Board[r][c] == Player.None)
                {
                    state.Board[r][c] = Player.AI;
                    state.CurrentPlayer = Player.Human;
                    return state;
                }
            }
        }
        
        throw new InvalidOperationException("No valid moves left for AI");
    }
}