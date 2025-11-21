using DotsWebApi.Model.Enums;

namespace DotsWebApi.Model;
public class Move
{
    public Player Player { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}