using DotsWebApi.Model.Enums;
namespace DotsWebApi.Model;

public class GameState
{
    public Player[][] Board { get; }
    public Player CurrentPlayer { get; set; }
    public GameEvaluation GameEvaluation { get; }
    public GameState(int boardSize, Player startingPlayer)
    {
        Board = new Player[boardSize][];
        for (int r = 0; r < boardSize; r++)
            Board[r] = new Player[boardSize];
        GameEvaluation = new GameEvaluation();
        CurrentPlayer = startingPlayer;
    }
}