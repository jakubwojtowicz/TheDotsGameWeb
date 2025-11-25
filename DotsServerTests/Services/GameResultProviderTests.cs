using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services;

namespace DotsServerTests.Services;

public class GameResultProviderTests
{
    [Fact]
    public void IsGameOver_FullBoard_ReturnsTrue()
    {
        var state = new GameState(2, Player.Human);
        state.Board[0] = new[] { Player.Human, Player.AI };
        state.Board[1] = new[] { Player.AI, Player.Human };

        var rules = new GameResultProvider();
        var result = rules.IsGameOver(state);

        Assert.True(result);
    }

    [Fact]
    public void IsGameOver_PartialBoard_ReturnsFalse()
    {
        var state = new GameState(2, Player.Human);
        state.Board[0] = new[] { Player.Human, Player.None };
        state.Board[1] = new[] { Player.AI, Player.Human };

        var rules = new GameResultProvider();
        var result = rules.IsGameOver(state);

        Assert.False(result);
    }

    [Fact]
    public void GetWinner_HumanWins_ReturnsHuman()
    {
        var state = new GameState(2, Player.Human);
        state.Scores[Player.Human] = 3;
        state.Scores[Player.AI] = 1;
        state.IsGameOver = true;

        var rules = new GameResultProvider();
        var winner = rules.GetWinner(state);

        Assert.Equal(Player.Human, winner);
    }

    [Fact]
    public void GetWinner_AIWins_ReturnsAI()
    {
        var state = new GameState(2, Player.Human);
        state.Scores[Player.Human] = 1;
        state.Scores[Player.AI] = 4;
        state.IsGameOver = true;

        var rules = new GameResultProvider();
        var winner = rules.GetWinner(state);

        Assert.Equal(Player.AI, winner);
    }

    [Fact]
    public void GetWinner_Tie_ReturnsNone()
    {
        var state = new GameState(2, Player.Human);
        state.Scores[Player.Human] = 2;
        state.Scores[Player.AI] = 2;
        state.IsGameOver = true;

        var rules = new GameResultProvider();
        var winner = rules.GetWinner(state);

        Assert.Equal(Player.None, winner);
    }

    [Fact]
    public void GetWinner_GameNotOver_ReturnsNone()
    {
        var state = new GameState(2, Player.Human);
        state.Scores[Player.Human] = 3;
        state.Scores[Player.AI] = 1;
        state.IsGameOver = false;

        var rules = new GameResultProvider();
        var winner = rules.GetWinner(state);

        Assert.Equal(Player.None, winner);
    }
    
}