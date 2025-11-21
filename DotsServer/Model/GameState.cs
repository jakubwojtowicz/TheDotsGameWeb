using DotsWebApi.Model.Enums;
namespace DotsWebApi.Model;

public class GameState
{
    public Player[][] Board { get; }
    public Player CurrentPlayer { get; set; }
    public bool IsGameOver { get; set; } = false;
    public Player Winner { get; set; } = Player.None;
    public Dictionary<Player, int> Scores => new()
    {
        { Player.Human, 0 },
        { Player.AI, 0 }
    };
    public Move? LastMove { get; set; }
    public MoveResult? LastMoveResult { get; set; }
    public GameState(int boardSize, Player startingPlayer)
    {
        if(boardSize <= 0 || boardSize > 100)
            throw new InvalidOperationException("Board size must be between 1 and 100.");
        Board = new Player[boardSize][];
        for (int r = 0; r < boardSize; r++)
            Board[r] = new Player[boardSize];
        CurrentPlayer = startingPlayer;
    }
}