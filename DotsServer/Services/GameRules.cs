using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services;

public interface IGameRules
{
    public MoveValidation GetMoveValidation(GameState state, Move move);
    public bool IsGameOver(GameState state);
    public Player GetWinner(GameState state);   
    public MoveResult GetMoveResult(GameState state, Player player, Player opponent);
}

public class GameRules : IGameRules
{
    public bool IsGameOver(GameState state)
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

    public Player GetWinner(GameState state)
    {
        if(state.IsGameOver == false)
            return Player.None;
        else if(state.Scores[Player.Human] > state.Scores[Player.AI ])
            return Player.Human;
        else if(state.Scores[Player.AI] > state.Scores[Player.Human])
            return Player.AI;
        else
            return Player.None; // Tie
    }

    private static readonly (int, int)[] Directions = new (int, int)[]
    {
        (-1, 0), (1, 0),
        (0, -1), (0, 1)
    };

    public MoveResult GetMoveResult(GameState state, Player player, Player opponent)
    {
        //Use BFS to find all enclosed opponent dots

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

    public MoveValidation GetMoveValidation(GameState state, Move move)
    {
        var validation = new MoveValidation();

        validation.IsValid = true;

        if (state.IsGameOver)
        {
            validation.IsValid = false;
            validation.Message = "The game has already ended.";   
        }
        else if(state.CurrentPlayer != move.Player)
        {
            validation.IsValid = false;
            validation.Message = $"It's not the {move.Player} player's turn.";
        }
        else if(state.Board[move.X][move.Y] != Player.None)
        {
            validation.IsValid = false;
            validation.Message = "The cell is already occupied.";
        }
        else if(move.X < 0 || move.X >= state.Board.Length || move.Y < 0 || move.Y >= state.Board.Length)
        {
            validation.IsValid = false;
            validation.Message = "Move is out of board bounds.";
        }
            
        return validation;
    }
}