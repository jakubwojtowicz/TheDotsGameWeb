using DotsWebApi.Model.Enums;
namespace DotsWebApi.Model;

public class GameState
{
    public Field[][] Board { get; }
    public Player CurrentPlayer { get; set; }
    public bool IsGameOver { get; set; } = false;
    public Player Winner { get; set; } = Player.None;
    public Dictionary<Player, int> Scores { get; } = new()
    {
        { Player.Human, 0 },
        { Player.AI, 0 }
    };
    public Move? LastMove { get; set; }
    public GameState(int boardSize, Player startingPlayer)
    {
        Board = new Field[boardSize][];
        for (int r = 0; r < boardSize; r++)
            Board[r] = new Field[boardSize];
        CurrentPlayer = startingPlayer;
    }
    public GameState Clone()
    {
        var size = Board.Length;
        var clone = new GameState(size, CurrentPlayer)
        {
            IsGameOver = this.IsGameOver,
            Winner = this.Winner,
            LastMove = this.LastMove
        };

        for (int r = 0; r < size; r++)
            Array.Copy(this.Board[r], clone.Board[r], size);

        clone.Scores[Player.Human] = this.Scores[Player.Human];
        clone.Scores[Player.AI] = this.Scores[Player.AI];

        return clone;
    }
}