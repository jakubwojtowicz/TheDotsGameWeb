using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services.AI;

public interface IMoveGenerator
{
    public List<Move> GenerateMoves(GameState state);
}

public class MoveGenerator : IMoveGenerator
{
    public List<Move> GenerateMoves(GameState state)
    {
        int rows = state.Board.Length;
        int cols = state.Board[0].Length;
        var list = new List<Move>();

        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
        {
            if (state.Board[r][c] == Player.None)
            {
                list.Add(new Move { X = r, Y = c, Player = state.CurrentPlayer});
            }
        }

        return list;
    }
}