using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services;

public interface IEnclosureDetector
{
    public List<(int r, int c)> GetEnclosedFields(GameState state, Player player);
}

public class EnclosureDetector: IEnclosureDetector
{
    public List<(int r, int c)> GetEnclosedFields(GameState state, Player player)
    {
        //Use BFS to find all enclosed fields by a player

        (int, int)[] directions = new (int, int)[]
        {
            (-1, 0), (1, 0),
            (0, -1), (0, 1)
        };

        int rows = state.Board.Length;
        int cols = state.Board[0].Length;
        var visited = new bool[rows, cols];
        var q = new Queue<(int r,int c)>();
        var captured = new List<(int r, int c)>();

        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
        {
            if (state.Board[r][c].Player == player || state.Board[r][c].Enclosed == true || visited[r,c]) continue;
            q.Clear();
            q.Enqueue((r,c));
            visited[r,c] = true;
            bool enclosed = true;
            var clusterCaptures = new List<(int r, int c)>();
            while (q.Count > 0)
            {
                var (cr, cc) = q.Dequeue();
                clusterCaptures.Add((cr, cc));
                if (cr == 0 || cr == rows-1 || cc == 0 || cc == cols-1) {enclosed = false;}
                foreach (var (dr, dc) in directions)
                {
                    int nr = cr + dr, nc = cc + dc;
                    if (nr < 0 || nr >= rows || nc < 0 || nc >= cols) continue;
                    if (visited[nr, nc]) continue;
                    if (state.Board[nr][nc].Player != player)
                    {
                        visited[nr, nc] = true;
                        q.Enqueue((nr, nc));
                    }
                }
            }
            if (enclosed)
            {
                captured.AddRange(clusterCaptures);
            } 
        }
        return captured;
    }
}