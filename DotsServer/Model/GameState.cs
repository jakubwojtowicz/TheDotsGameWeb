using DotsWebApi.Model.Enums;
namespace DotsWebApi.Model;

public class GameState
{
    public Player[][] Board { get; }

    public Player CurrentPlayer { get; set; } = Player.Human;
    public GameResult Result { get; set; } = GameResult.Ongoing;
    public int HumanScore { get; set; } = 0;
    public int AIScore { get; set; } = 0;

    public GameState(int boardSize)
    {
        Board = new Player[boardSize][];
        for (int r = 0; r < boardSize; r++)
            Board[r] = new Player[boardSize];
    }
}