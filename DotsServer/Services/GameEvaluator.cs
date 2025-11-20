using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services;

public interface IGameEvaluator
{
    GameEvaluation EvaluateGame(GameState state);
}

public class GameEvaluator : IGameEvaluator
{
    public GameEvaluation EvaluateGame(GameState state)
    {
        var humanScore = CalculateScore(state, Player.Human, Player.AI);
        var aiScore = CalculateScore(state, Player.AI, Player.Human);
        var isGameOver = CheckGameOver(state);
        var winner = isGameOver
            ? (humanScore > aiScore ? Player.Human : (aiScore > humanScore ? Player.AI : Player.None))
            : Player.None;

        return new GameEvaluation()
        {
            IsGameOver = isGameOver,
            Winner = winner,    
            HumanScore = humanScore,
            AiScore = aiScore
        };
    }

    public bool CheckGameOver(GameState state)
    {
        for(int r = 0; r < state.Board.Length; r++)
        {
            for(int c = 0; c < state.Board[r].Length; c++)
            {
                // If any cell is empty, the game is not over
                if(state.Board[r][c] == Player.None)
                    return false;
            }
        }
        // If no empty cells, the game is over
        return true;
    }

    private static readonly (int, int)[] Directions = new (int, int)[]
    {
        (-1, 0), (1, 0),
        (0, -1), (0, 1)
    };

    public int CalculateScore(GameState state, Player player, Player opponent)
    {
        int rows = state.Board.Length;
        int cols = state.Board[0].Length;
        var visited = new bool[rows, cols];
        var q = new Queue<(int r,int c)>();
        int score = 0;

        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
        {
            if (state.Board[r][c] != opponent || visited[r,c]) continue;
            q.Clear();
            q.Enqueue((r,c));
            visited[r,c] = true;
            bool enclosed = true;
            int clusterCount = 0;
            while (q.Count > 0)
            {
                var (cr, cc) = q.Dequeue();
                if(state.Board[cr][cc] == opponent) clusterCount++;
                if (cr == 0 || cr == rows-1 || cc == 0 || cc == cols-1) {enclosed = false;}
                foreach (var (dr, dc) in Directions)
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
            if (enclosed) score += clusterCount;
        }
        return score;
    }
}