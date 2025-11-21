using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services.AI;

public class RandomMoveAiStrategy : IAIStrategy
{
    public Move GetNextMove(GameState state)
    {
        if (state == null)
            throw new ArgumentNullException(nameof(state));
        var rand = new Random();
        int boardSize = state.Board.Length;
        List<(int r, int c)> availableMoves = new();
        for (int r = 0; r < boardSize; r++)
        {
            for (int c = 0; c < boardSize; c++)
            {
                if (state.Board[r][c] == Player.None)
                    availableMoves.Add((r, c));
            }
        }
        if (availableMoves.Count == 0)
            throw new InvalidOperationException("No available moves for AI.");
        var (row, col) = availableMoves[rand.Next(availableMoves.Count)];
        return new Move { Player = Player.AI, X = row, Y = col };
    }
}