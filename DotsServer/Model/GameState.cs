using DotsWebApi.Model.Enums;
namespace DotsWebApi.Model;

public class GameState
{
    public Player[][] Board { get; }
    public Player CurrentPlayer { get; set; }
    public bool IsGameOver { get; set; } = false;
    public Player Winner { get; set; } = Player.None;
    public Dictionary<Player, int> Scores { get; } = new()
    {
        { Player.Human, 0 },
        { Player.AI, 0 }
    };
    public Move? LastMove { get; set; }
    public MoveResult? LastMoveResult { get; set; }
    public GameState(int boardSize, Player startingPlayer)
    {
        Board = new Player[boardSize][];
        for (int r = 0; r < boardSize; r++)
            Board[r] = new Player[boardSize];
        CurrentPlayer = startingPlayer;
    }
    public GameState Clone()
    {
        var size = Board.Length;
        var clone = new GameState(size, CurrentPlayer)
        {
            IsGameOver = this.IsGameOver,
            Winner = this.Winner,
            LastMove = this.LastMove,
            LastMoveResult = this.LastMoveResult
        };

        for (int r = 0; r < size; r++)
            Array.Copy(this.Board[r], clone.Board[r], size);

        clone.Scores[Player.Human] = this.Scores[Player.Human];
        clone.Scores[Player.AI] = this.Scores[Player.AI];

        return clone;
    }
}