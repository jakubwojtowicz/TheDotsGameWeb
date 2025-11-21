namespace DotsServerTests;
using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services;
using Xunit;

public class MoveApplierTests
{
    [Fact]
    public void ApplyMove_UpdatesBoardAndCurrentPlayer()
    {
        var state = new GameState(2, Player.Human);
        var move = new Move { Player = Player.Human, X = 0, Y = 0 };

        var applier = new MoveApplier();
        applier.ApplyMove(state, move);

        Assert.Equal(Player.Human, state.Board[0][0]);
        Assert.Equal(Player.AI, state.CurrentPlayer);
        Assert.Equal(move, state.LastMove);
    }

    [Fact]
    public void CaptureDots_UpdatesBoardAndScores()
    {
        var state = new GameState(2, Player.Human);
        var moveResult = new MoveResult
        {
            Player = Player.Human,
            Captured = new List<(int r, int c)> { (0, 1), (1, 0) },
            Score = 2
        };

        var applier = new MoveApplier();
        applier.CaptureDots(state, moveResult);

        Assert.Equal(Player.Human, state.Board[0][1]);
        Assert.Equal(Player.Human, state.Board[1][0]);
        Assert.Equal(2, state.Scores[Player.Human]);
    }
}