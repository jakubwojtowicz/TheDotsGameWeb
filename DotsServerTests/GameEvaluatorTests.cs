namespace DotsServerTests;
using DotsWebApi.Model;         
using DotsWebApi.Model.Enums;    
using DotsWebApi.Services;       
using Xunit;

public class GaveEvaluatorTests
{

    [Fact]
    public void CalculateScore_EnclosedDot_ReturnPoint()
    {
        var state = new GameState(3, Player.Human);

        state.Board[0] = new[] { Player.Human, Player.Human, Player.None };
        state.Board[1] = new[] { Player.Human, Player.AI,    Player.Human };
        state.Board[2] = new[] { Player.None,  Player.Human, Player.None };

        var player = Player.Human;
        var opponent = Player.AI;
        var evaluator = new GameEvaluator();

        var score = evaluator.CalculateScore(state, player, opponent);

        Assert.Equal(1, score);
    }

    [Fact]
    public void CalculateScore_NotEnclosedDot_ReturnZeroPoints()
    {
        var state = new GameState(3, Player.Human);

        state.Board[0] = new[] { Player.Human, Player.Human, Player.None };
        state.Board[1] = new[] { Player.Human, Player.AI,    Player.AI };
        state.Board[2] = new[] { Player.None,  Player.Human, Player.None };

        var player = Player.Human;
        var opponent = Player.AI;
        var evaluator = new GameEvaluator();

        var score = evaluator.CalculateScore(state, player, opponent);

        Assert.Equal(0, score);
    }

    [Fact]
    public void CalculateScore_EnclosedMultipleDots_ReturnPoints()
    {
        var state = new GameState(4, Player.Human);

        state.Board[0] = new[] { Player.Human, Player.Human, Player.Human, Player.None };
        state.Board[1] = new[] { Player.Human, Player.AI,    Player.Human, Player.Human };
        state.Board[2] = new[] { Player.Human, Player.AI,    Player.AI,    Player.Human };
        state.Board[3] = new[] { Player.None,  Player.Human, Player.Human, Player.None };

        var player = Player.Human;
        var opponent = Player.AI;
        var evaluator = new GameEvaluator();

        var score = evaluator.CalculateScore(state, player, opponent);

        Assert.Equal(3, score);
    }

    [Fact]
    public void CalculateScore_EmptyBoard_ReturnZeroPoints()
    {
        var state = new GameState(4, Player.Human);

        var player = Player.Human;
        var opponent = Player.AI;
        var evaluator = new GameEvaluator();

        var score = evaluator.CalculateScore(state, player, opponent);

        Assert.Equal(0, score);
    }

    [Fact]
    public void CalculateScore_FullBoardWithoutEnclosed_ReturnZeroPoints()
    {
        var state = new GameState(4, Player.Human);

        state.Board[0] = new[] { Player.Human, Player.AI, Player.Human, Player.AI };
        state.Board[1] = new[] { Player.Human, Player.AI, Player.Human, Player.AI };
        state.Board[2] = new[] { Player.Human, Player.AI, Player.Human, Player.AI };
        state.Board[3] = new[] { Player.Human, Player.AI, Player.Human, Player.AI };

        var player = Player.Human;
        var opponent = Player.AI;
        var evaluator = new GameEvaluator();

        var score = evaluator.CalculateScore(state, player, opponent);

        Assert.Equal(0, score);
    }

    [Fact]
    public void CalculateScore_DoubleClosure_ReturnPointsForBothPlayers()
    {
        var state = new GameState(7, Player.Human);

        state.Board[0] = new[] { Player.None, Player.None, Player.None, Player.None, Player.None, Player.AI, Player.None };
        state.Board[1] = new[] { Player.None, Player.None, Player.Human, Player.None, Player.AI, Player.Human, Player.AI };
        state.Board[2] = new[] { Player.None, Player.Human, Player.AI, Player.Human, Player.None, Player.AI, Player.None };
        state.Board[3] = new[] { Player.Human, Player.AI, Player.Human, Player.AI, Player.Human, Player.None, Player.None };
        state.Board[4] = new[] { Player.None, Player.Human, Player.AI, Player.Human, Player.None, Player.None, Player.None };
        state.Board[5] = new[] { Player.None, Player.None, Player.Human, Player.None, Player.None, Player.None, Player.None };
        state.Board[6] = new[] { Player.None, Player.None, Player.None, Player.None, Player.None, Player.None, Player.None };

        var evaluator = new GameEvaluator();

        var scorePlayer = evaluator.CalculateScore(state, Player.Human, Player.AI);

        Assert.Equal(4, scorePlayer);

        var scoreOpponent = evaluator.CalculateScore(state, Player.AI, Player.Human);

        Assert.Equal(2, scoreOpponent);
    }
}
