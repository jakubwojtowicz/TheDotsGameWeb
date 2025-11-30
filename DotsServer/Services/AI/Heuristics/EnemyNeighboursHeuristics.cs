using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services.AI.Heuristics;

public class EnemyNeighboursHeuristic: IHeuristic
{
    private readonly int attackWeight = 10;
    private readonly int defensekWeight = 8;

    public int Evaluate(GameState state)
    {
        int humanEnemyNeighbours = 0;
        int aiEnemyNeighbours = 0;
        (int, int)[] directions = new (int, int)[]
        {
            (-1, 0), (1, 0),
            (0, -1), (0, 1)
        };
        int rows = state.Board.Length;
        int cols = state.Board[0].Length;

        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
        {
            if(state.Board[r][c].Player != Player.None && state.Board[r][c].EnclosedBy == Player.None)
            {
                foreach (var (dr, dc) in directions)
                {
                    int nr = r + dr, nc = c + dc;
                    if (nr < 0 || nr >= rows || nc < 0 || nc >= cols) continue;
                    if(state.Board[r][c].Player == Player.Human && state.Board[nr][nc].Player == Player.AI) humanEnemyNeighbours ++;
                    else if(state.Board[r][c].Player == Player.AI && state.Board[nr][nc].Player == Player.Human) aiEnemyNeighbours ++;
                }
            }
        }

        return humanEnemyNeighbours * attackWeight - aiEnemyNeighbours * defensekWeight;
    }
}