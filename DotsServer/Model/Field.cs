using DotsWebApi.Model.Enums;

namespace DotsWebApi.Model;

public class Field
{
    public Player Player { get; set; } = Player.None;
    public bool Enclosed { get; set; } = false;
}