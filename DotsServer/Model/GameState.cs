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
        {
            Board[r] = new Field[boardSize];

            for (int c = 0; c < boardSize; c++)
            {
                Board[r][c] = new Field();
            }
        }

        CurrentPlayer = startingPlayer;
    }
    public GameState Clone()
    {
        int size = Board.Length;

        var clone = new GameState(size, CurrentPlayer)
        {
            IsGameOver = this.IsGameOver,
            Winner = this.Winner,
            LastMove = this.LastMove
        };

        for (int r = 0; r < size; r++)
        {
            for (int c = 0; c < size; c++)
            {
                clone.Board[r][c] = new Field
                {
                    Player = this.Board[r][c].Player,
                    EnclosedBy = this.Board[r][c].EnclosedBy
                };
            }
        }

        clone.Scores[Player.Human] = this.Scores[Player.Human];
        clone.Scores[Player.AI] = this.Scores[Player.AI];

        return clone;
    }
}