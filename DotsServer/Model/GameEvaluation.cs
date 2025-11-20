using DotsWebApi.Model.Enums;

namespace DotsWebApi.Model;

public class GameEvaluation
{
    public bool IsGameOver { get; set; } = false;
    public Player? Winner { get; set; }
    public Dictionary<Player, int> Scores { get; set; } = new Dictionary<Player, int>();
}