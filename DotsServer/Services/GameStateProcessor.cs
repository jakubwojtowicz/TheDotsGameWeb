using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services;

public interface IGameStateProcessor
{ 
    public GameState GetNextState(GameState prevState, Move move);
}

public class GameStateProcessor : IGameStateProcessor
{
    private readonly IMoveResolver _moveResolver;
    private readonly IGameResultProvider _gameResultProvider;
    public GameStateProcessor(IMoveResolver moveResolver, IGameResultProvider gameResultProvider)
    {
        _moveResolver = moveResolver;
        _gameResultProvider = gameResultProvider;
    }

    public GameState GetNextState(GameState prevState, Move move)
    {
        var newState = prevState.Clone();
        
        newState.Board[move.X][move.Y] = move.Player;
        newState.CurrentPlayer = move.Player == Player.Human ? Player.AI : Player.Human;
        newState.LastMove = move;

        var moveResult = _moveResolver.GetMoveResult(newState, move.Player, move.Player == Player.Human ? Player.AI : Player.Human);
        newState.LastMoveResult = moveResult;

        if(moveResult.Score > 0){
            foreach (var (r, c) in moveResult.Captured)
            {
                newState.Board[r][c] = moveResult.Player;
            }
            newState.Scores[moveResult.Player] += moveResult.Score;
        }

        newState.IsGameOver = _gameResultProvider.IsGameOver(newState);

        if (newState.IsGameOver){
            newState.CurrentPlayer = Player.None;
            newState.Winner = _gameResultProvider.GetWinner(newState);
        }

        return newState;
    }
}