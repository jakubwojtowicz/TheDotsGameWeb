using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services;

namespace DotsServerTests.Services;

public class MoveResolverTests
{
    [Fact]
    public void GetMoveResult_OneEnclosedDot_ReturnOnePoint()
    {
        var state = new GameState(3, Player.Human);

        state.Board[0] = new[] { Player.Human, Player.Human, Player.None };
        state.Board[1] = new[] { Player.Human, Player.AI,    Player.Human };
        state.Board[2] = new[] { Player.None,  Player.Human, Player.None };

        var player = Player.Human;
        var opponent = Player.AI;
        var rules = new MoveResolver();

        var result = rules.GetMoveResult(state, player, opponent);

        Assert.Equal(1, result.Score);
        Assert.Contains((1, 1), result.Captured);
    }

    [Fact]
    public void GetMoveResult_NotEnclosedDot_ReturnZeroPoints()
    {
        var state = new GameState(3, Player.Human);

        state.Board[0] = new[] { Player.Human, Player.Human, Player.None };
        state.Board[1] = new[] { Player.Human, Player.AI,    Player.AI };
        state.Board[2] = new[] { Player.None,  Player.Human, Player.None };

        var player = Player.Human;
        var opponent = Player.AI;
        var rules = new MoveResolver();

        var result = rules.GetMoveResult(state, player, opponent);

        Assert.Equal(0, result.Score);
        Assert.Empty(result.Captured);
    }

    [Fact]
    public void GetMoveResult_EnclosedMultipleDots_ReturnPoints()
    {
        var state = new GameState(4, Player.Human);

        state.Board[0] = new[] { Player.Human, Player.Human, Player.Human, Player.None };
        state.Board[1] = new[] { Player.Human, Player.AI,    Player.Human, Player.Human };
        state.Board[2] = new[] { Player.Human, Player.AI,    Player.AI,    Player.Human };
        state.Board[3] = new[] { Player.None,  Player.Human, Player.Human, Player.None };

        var player = Player.Human;
        var opponent = Player.AI;
        var rules = new MoveResolver();

        var result = rules.GetMoveResult(state, player, opponent);

        Assert.Equal(3, result.Score);
        Assert.Contains((1, 1), result.Captured);
        Assert.Contains((2, 1), result.Captured);
        Assert.Contains((2, 2), result.Captured);
    }

    [Fact]
    public void GetMoveResult_EmptyBoard_ReturnZeroPoints()
    {
        var state = new GameState(4, Player.Human);

        var player = Player.Human;
        var opponent = Player.AI;
        var rules = new MoveResolver();

        var result = rules.GetMoveResult(state, player, opponent);

        Assert.Equal(0, result.Score);
        Assert.Empty(result.Captured);  
    }

    [Fact]
    public void GetMoveResult_FullBoardWithoutEnclosed_ReturnZeroPoints()
    {
        var state = new GameState(4, Player.Human);

        state.Board[0] = new[] { Player.Human, Player.AI, Player.Human, Player.AI };
        state.Board[1] = new[] { Player.Human, Player.AI, Player.Human, Player.AI };
        state.Board[2] = new[] { Player.Human, Player.AI, Player.Human, Player.AI };
        state.Board[3] = new[] { Player.Human, Player.AI, Player.Human, Player.AI };

        var player = Player.Human;
        var opponent = Player.AI;
        var rules = new MoveResolver();

        var result = rules.GetMoveResult(state, player, opponent);

        Assert.Equal(0, result.Score);
        Assert.Empty(result.Captured);  
    }

    [Fact]
    public void GetMoveResult_EnclosedOnBorder_ReturnZeroPoints()
    {
        var state = new GameState(3, Player.Human);

        state.Board[0] = new[] { Player.None, Player.None, Player.AI};
        state.Board[1] = new[] { Player.None, Player.AI, Player.Human};
        state.Board[2] = new[] { Player.None, Player.None, Player.AI};

        var player = Player.AI;
        var opponent = Player.Human;
        var rules = new MoveResolver();

        var result = rules.GetMoveResult(state, player, opponent);

        Assert.Equal(0, result.Score);
        Assert.Empty(result.Captured);  
    }
    
}