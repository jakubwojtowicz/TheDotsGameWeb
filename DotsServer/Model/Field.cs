using DotsWebApi.Model.Enums;

namespace DotsWebApi.Model;

public class Field
{
    public Player Player { get; set; } = Player.None;
    public Player EnclosedBy { get; set; } = Player.None;
}