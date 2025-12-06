using DotsWebApi.Model.Enums;

namespace DotsWebApi.Model.Helpers;

public class UndoMoveData
{
    public List<(int Row, int Col, Player PreviousPlayer, Player PreviousEnclosedBy)> ChangedFields { get; set; } = new();
    public Player PreviousCurrentPlayer { get; set; }
    public bool PreviousIsGameOver { get; set; }
    public Player PreviousWinner { get; set; }
    public Dictionary<Player, int> PreviousScores { get; set; } = new();
}
