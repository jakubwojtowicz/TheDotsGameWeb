using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services;

namespace DotsServerTests.Services;

public class MoveValidatorTests
{
    [Fact]
    public void GetMoveValidation_ValidMove_ReturnsValid()
    {
        var state = new GameState(3, Player.Human);

        var move = new Move
        {
            X = 1,
            Y = 1,
            Player = Player.Human,
        };

        var rules = new MoveValidator();
        var validation = rules.GetMoveValidation(state, move);

        Assert.True(validation.IsValid);
    }

    [Fact]
    public void GetMoveValidation_WrongPlayer_ReturnsInvalid()
    {
        var state = new GameState(3, Player.Human);

        var move = new Move
        {
            X = 1,
            Y = 1,
            Player = Player.AI,
        };

        var rules = new MoveValidator();
        var validation = rules.GetMoveValidation(state, move);

        Assert.False(validation.IsValid);
    }

    [Fact]
    public void GetMoveValidation_GameEnded_ReturnsInvalid()
    {
        var state = new GameState(3, Player.Human);
        state.IsGameOver = true;

        var move = new Move
        {
            X = 1,
            Y = 1,
            Player = Player.Human,
        };

        var rules = new MoveValidator();
        var validation = rules.GetMoveValidation(state, move);

        Assert.False(validation.IsValid);
    }

    [Fact]
    public void GetMoveValidation_OutOfBounds_ReturnsInvalid()
    {
        var state = new GameState(3, Player.Human);
        state.IsGameOver = false;

        var move = new Move
        {
            X = 50000,
            Y = -10,
            Player = Player.Human,
        };

        var rules = new MoveValidator();
        var validation = rules.GetMoveValidation(state, move);

        Assert.False(validation.IsValid);
    }

    [Fact]
    public void GetMoveValidation_CellOccupied_ReturnsInvalid()
    {
        var state = new GameState(3, Player.Human);

        state.Board[0] = new[] { Player.None, Player.AI, Player.None };

        var move = new Move
        {
            X = 0,
            Y = 1,
            Player = Player.Human,
        };

        var rules = new MoveValidator();
        var validation = rules.GetMoveValidation(state, move);

        Assert.False(validation.IsValid);
    }
}