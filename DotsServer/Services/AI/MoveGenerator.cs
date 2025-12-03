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
        (int, int)[] directions = new (int, int)[]
        {
            (-1,-1), (-1, 0), (-1, 1),
            (0, -1), (0, 1),
            (1, -1), (1, 0), (1, 1), 
        };

        int rows = state.Board.Length;
        int cols = state.Board[0].Length;
        var list = new List<Move>();

        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
        {
            if (state.Board[r][c].Player != Player.None &&
                state.Board[r][c].Player != state.CurrentPlayer && 
                state.Board[r][c].EnclosedBy == Player.None)
            {
                foreach(var(dr,dc) in directions)
                {
                    int nr = r + dr, nc = c + dc;
                    if (nr < 0 || nr >= rows || nc < 0 || nc >= cols) continue;
                    if (state.Board[nr][nc].Player == Player.None && 
                        state.Board[nr][nc].EnclosedBy == Player.None)
                    {
                        list.Add(new Move { X = nr, Y = nc, Player = state.CurrentPlayer});
                    }
                }
            }
        }

        return list;
    }
}