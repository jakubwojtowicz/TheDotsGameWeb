using DotsWebApi.DTO;
using DotsWebApi.Exceptions;
using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
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
    public GameService(IGameRules gameRules)
    {
        _gameRules = gameRules;
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

        ValidateTurn(state, Player.Human);
        _gameRules.ValidateMove(state, move);
        UpdateGameState(state, move);

        return state;
    }

    public GameState MakeAIMove(string gameId)
    {
        if (!_games.TryGetValue(gameId, out var state))
            throw new GameNotFoundException();

        ValidateTurn(state, Player.AI);

        for(int r = 0; r < state.Board.Length; r++)
        {
            for(int c = 0; c < state.Board[r].Length; c++)
            {
                if(state.Board[r][c] == Player.None)
                {
                    var move = new Move { Player = Player.AI, X = r, Y = c };
                    UpdateGameState(state, move);
                    return state;
                }
            }
        }
        
        throw new InvalidOperationException("No valid moves left for AI");
    }

    private void UpdateGameState(GameState state, Move move)
    {
        state.Board[move.X][move.Y] = move.Player;
        state.LastMove = move;

        var opponent = move.Player == Player.Human ? Player.AI : Player.Human;

        var moveResult = _gameRules.GetMoveResult(state, move.Player, opponent);
     
        state.LastMoveResult = moveResult;

        if(move.Player == Player.Human)
        {
            state.HumanScore += moveResult.Score;
        }
        else if(move.Player == Player.AI)
        {
            state.AiScore += moveResult.Score;
        }

        foreach(var (r,c) in moveResult.Captured)
        {
            state.Board[r][c] = move.Player;
        }

        state.IsGameOver = _gameRules.CheckGameOver(state);
        state.Winner = _gameRules.GetWinner(state);
        state.CurrentPlayer = _gameRules.SwitchPlayer(state);
    }

    private void ValidateTurn(GameState state, Player player)
    {
        if(state.IsGameOver)
            throw new InvalidOperationException("The game has already ended.");
        else if(state.CurrentPlayer != player)
            throw new InvalidOperationException($"It's not the {player} player's turn.");
    }
}