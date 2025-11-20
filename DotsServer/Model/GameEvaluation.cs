using DotsWebApi.Model.Enums;

namespace DotsWebApi.Model;

public class GameEvaluation
{
    public bool IsGameOver { get; set; } = false;
    public Player? Winner { get; set; }
    public int AiScore { get; set; } = 0;
    public int HumanScore { get; set; } = 0;
}