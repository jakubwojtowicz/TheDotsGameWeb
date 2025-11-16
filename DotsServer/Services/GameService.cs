using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

public interface IGameService
{
    string CreateGame(int boardSize = 50);
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
            throw new ArgumentException("Invalid game ID");

        return _games[gameId];
    }

    public GameState MakeMove(string gameId, Move move)
    {
        if(!_games.ContainsKey(gameId))
            throw new ArgumentException("Invalid game ID");

        var state = _games[gameId];

        if(state.Board[move.X][move.Y] != Player.None)
            throw new InvalidOperationException("Invalid move");

        state.Board[move.X][move.Y] = Player.Human;

        state.CurrentPlayer = Player.AI;

        return state;
    }

    public GameState MakeAIMove(string gameId)
    {
        if(!_games.ContainsKey(gameId))
            throw new ArgumentException("Invalid game ID");

        var state = _games[gameId];

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