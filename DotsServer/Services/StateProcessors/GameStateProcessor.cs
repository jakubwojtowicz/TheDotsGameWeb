using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services.StateProcessors;

public interface IGameStateProcessor
{ 
    public GameState GetNextState(GameState prevState, Move move);
}

public class GameStateProcessor : IGameStateProcessor
{
    private readonly IEnclosureDetector _enclosureDetector;
    private readonly IGameResultProvider _gameResultProvider;
    public GameStateProcessor(IEnclosureDetector enclosureDetector, IGameResultProvider gameResultProvider)
    {
        _enclosureDetector = enclosureDetector;
        _gameResultProvider = gameResultProvider;
    }

    public GameState GetNextState(GameState prevState, Move move)
    {
        var newState = prevState.Clone();
        
        newState.Board[move.X][move.Y].Player = move.Player;
        newState.CurrentPlayer = move.Player == Player.Human ? Player.AI : Player.Human;
        newState.LastMove = move;

        var enclosed = _enclosureDetector.GetEnclosedFields(newState, move.Player);

        if(enclosed.Count > 0){
            foreach (var (r, c) in enclosed)
            {
                if(newState.Board[r][c].Player != Player.None)
                    newState.Scores[move.Player] += 1;
                newState.Board[r][c].EnclosedBy = move.Player;
            }
        }

        newState.IsGameOver = _gameResultProvider.IsGameOver(newState);

        if (newState.IsGameOver){
            newState.CurrentPlayer = Player.None;
            newState.Winner = _gameResultProvider.GetWinner(newState);
        }

        return newState;
    }
}