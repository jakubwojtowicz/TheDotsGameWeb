using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services;

public interface IMoveResolver
{
    public MoveResult GetMoveResult(GameState state, Player player, Player opponent);
}

public class MoveResolver: IMoveResolver
{
    public MoveResult GetMoveResult(GameState state, Player player, Player opponent)
    {
        //Use BFS to find all enclosed opponent dots (O(n) time complexity)

        (int, int)[] directions = new (int, int)[]
        {
            (-1, 0), (1, 0),
            (0, -1), (0, 1)
        };

        int rows = state.Board.Length;
        int cols = state.Board[0].Length;
        var visited = new bool[rows, cols];
        var q = new Queue<(int r,int c)>();
        int score = 0;
        var captured = new List<(int r, int c)>();

        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
        {
            if (state.Board[r][c] != opponent || visited[r,c]) continue;
            q.Clear();
            q.Enqueue((r,c));
            visited[r,c] = true;
            bool enclosed = true;
            int clusterCount = 0;
            var clusterDots = new List<(int r, int c)>();
            while (q.Count > 0)
            {
                var (cr, cc) = q.Dequeue();
                if(state.Board[cr][cc] == opponent) {
                    clusterCount++;
                    clusterDots.Add((cr, cc));
                }
                if (cr == 0 || cr == rows-1 || cc == 0 || cc == cols-1) {enclosed = false;}
                foreach (var (dr, dc) in directions)
                {
                    int nr = cr + dr, nc = cc + dc;
                    if (nr < 0 || nr >= rows || nc < 0 || nc >= cols) continue;
                    if (visited[nr, nc]) continue;
                    if (state.Board[nr][nc] != player)
                    {
                        visited[nr, nc] = true;
                        q.Enqueue((nr, nc));
                    }
                }
            }
            if (enclosed)
            {
                score += clusterCount;
                captured.AddRange(clusterDots);
            } 
        }
        return new MoveResult
        {
            Player = player,
            Score = score,
            Captured = captured
        };
    }
}