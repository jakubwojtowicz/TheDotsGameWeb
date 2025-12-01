using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services;
using DotsWebApi.Services.GameEngine;
using DotsWebApiTests.Helpers;

namespace DotsServerTests.Tests.Services;

public class GameResultProviderTests
{
    private readonly GameResultProvider _provider;

    public GameResultProviderTests()
    {
        _provider = new GameResultProvider();
    }

    [Fact]
    public void IsGameOver_EmptyBoard_ReturnsTrue()
    {
        var state = new GameState(5, Player.Human);

        var result = _provider.IsGameOver(state);

        Assert.False(result);
    }

    [Fact]
    public void IsGameOver_FullBoard_ReturnsTrue()
    {
        var state = BoardFactory.Create(
            "H H A",
            "H A A",
            "A A H"
        );

        var result = _provider.IsGameOver(state);

        Assert.True(result);
    }

    [Fact]
    public void IsGameOver_PartialBoard_ReturnsFalse()
    {
        var state = BoardFactory.Create(
            "H H A",
            "H E A",
            "A A E"
        );

        var result = _provider.IsGameOver(state);

        Assert.False(result);
    }

    [Fact]
    public void IsGameOver_PartialBoardWithEmptyEnclosedByHuman_ReturnsTrue()
    {
        var state = BoardFactory.Create(
            "H H A",
            "H E H",
            "A H A"
        );

        state.Board[1][1].EnclosedBy = Player.Human;

        var result = _provider.IsGameOver(state);

        Assert.True(result);
    }

    [Fact]
    public void GetWinner_HumanWins_ReturnsHuman()
    {
        var state = new GameState(2, Player.Human);
        state.Scores[Player.Human] = 3;
        state.Scores[Player.AI] = 1;
        state.IsGameOver = true;

        var winner = _provider.GetWinner(state);

        Assert.Equal(Player.Human, winner);
    }

    [Fact]
    public void GetWinner_AIWins_ReturnsAI()
    {
        var state = new GameState(2, Player.Human);
        state.Scores[Player.Human] = 1;
        state.Scores[Player.AI] = 4;
        state.IsGameOver = true;

        var winner = _provider.GetWinner(state);

        Assert.Equal(Player.AI, winner);
    }

    [Fact]
    public void GetWinner_Tie_ReturnsNone()
    {
        var state = new GameState(2, Player.Human);
        state.Scores[Player.Human] = 2;
        state.Scores[Player.AI] = 2;
        state.IsGameOver = true;

        var winner = _provider.GetWinner(state);

        Assert.Equal(Player.None, winner);
    }

    [Fact]
    public void GetWinner_GameNotOver_ReturnsNone()
    {
        var state = new GameState(2, Player.Human);
        state.Scores[Player.Human] = 3;
        state.Scores[Player.AI] = 1;
        state.IsGameOver = false;

        var winner = _provider.GetWinner(state);

        Assert.Equal(Player.None, winner);
    }
    
}