using DotsWebApi.Model.Enums;

namespace DotsWebApi.Model;

public class MoveResult
{
    public Player Player { get; set; }
    public int Score { get; set; }
    public List<(int r, int c)> Captured { get; set; } = new();
}