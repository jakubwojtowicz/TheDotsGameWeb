using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services;

public interface IMoveApplier
{
    void ApplyMove(GameState board, Move move);
    public void CaptureDots(GameState state, MoveResult moveResult);
}

public class MoveApplier : IMoveApplier
{
    public void ApplyMove(GameState state, Move move)
    {
        state.Board[move.X][move.Y] = move.Player;
        state.CurrentPlayer = move.Player == Player.Human ? Player.AI : Player.Human;
        state.LastMove = move;
    }

    public void CaptureDots(GameState state, MoveResult moveResult)
    {
        foreach (var (r, c) in moveResult.Captured)
        {
            state.Board[r][c] = moveResult.Player;
        }
        state.Scores[moveResult.Player] += moveResult.Score;
    }
}