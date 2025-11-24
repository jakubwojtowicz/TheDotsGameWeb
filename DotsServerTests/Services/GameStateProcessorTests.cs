using DotsWebApi.Model;         
using DotsWebApi.Model.Enums;    
using DotsWebApi.Services;       
using Xunit;

namespace DotsServerTests.Services;

public class GameStateProcessorTests
{
    [Fact]
    public void IsGameOver_FullBoard_ReturnsTrue()
    {
        var state = new GameState(2, Player.Human);
        state.Board[0] = new[] { Player.Human, Player.AI };
        state.Board[1] = new[] { Player.AI, Player.Human };

        var rules = new GameStateProcessor();
        var result = rules.IsGameOver(state);

        Assert.True(result);
    }

    [Fact]
    public void IsGameOver_PartialBoard_ReturnsFalse()
    {
        var state = new GameState(2, Player.Human);
        state.Board[0] = new[] { Player.Human, Player.None };
        state.Board[1] = new[] { Player.AI, Player.Human };

        var rules = new GameStateProcessor();
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

        var rules = new GameStateProcessor();
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

        var rules = new GameStateProcessor();
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

        var rules = new GameStateProcessor();
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

        var rules = new GameStateProcessor();
        var winner = rules.GetWinner(state);

        Assert.Equal(Player.None, winner);
    }

    [Fact]
    public void GetMoveResult_OneEnclosedDot_ReturnOnePoint()
    {
        var state = new GameState(3, Player.Human);

        state.Board[0] = new[] { Player.Human, Player.Human, Player.None };
        state.Board[1] = new[] { Player.Human, Player.AI,    Player.Human };
        state.Board[2] = new[] { Player.None,  Player.Human, Player.None };

        var player = Player.Human;
        var opponent = Player.AI;
        var rules = new GameStateProcessor();

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
        var rules = new GameStateProcessor();

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
        var rules = new GameStateProcessor();

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
        var rules = new GameStateProcessor();

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
        var rules = new GameStateProcessor();

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
        var rules = new GameStateProcessor();

        var result = rules.GetMoveResult(state, player, opponent);

        Assert.Equal(0, result.Score);
        Assert.Empty(result.Captured);  
    }

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

        var rules = new GameStateProcessor();
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

        var rules = new GameStateProcessor();
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

        var rules = new GameStateProcessor();
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

        var rules = new GameStateProcessor();
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

        var rules = new GameStateProcessor();
        var validation = rules.GetMoveValidation(state, move);

        Assert.False(validation.IsValid);
    }

    [Fact]
    public void ApplyMove_HumanMove_ReturnsNewState()
    {
        var state = new GameState(3, Player.Human);

        var move = new Move
        {
            X = 0,
            Y = 1,
            Player = Player.Human
        };

        var rules = new GameStateProcessor();

        var newState = rules.ApplyMove(state, move);

        Assert.Equal(Player.AI, newState.CurrentPlayer);
        Assert.Equal(move, newState.LastMove);
        Assert.False(newState.IsGameOver);
        Assert.Equal(Player.Human, newState.Board[0][1]);
    }

    [Fact]
    public void ApplyMove_AIMove_ReturnsNewState()
    {
        var state = new GameState(3, Player.AI);

        var move = new Move
        {
            X = 0,
            Y = 1,
            Player = Player.AI
        };

        var rules = new GameStateProcessor();

        var newState = rules.ApplyMove(state, move);

        Assert.Equal(Player.Human, newState.CurrentPlayer);
        Assert.Equal(move, newState.LastMove);
        Assert.False(newState.IsGameOver);
        Assert.Equal(Player.AI, newState.Board[0][1]);
    }

    [Fact]
    public void ApplyMove_LastMove_SetsGameEndedState()
    {
        var state = new GameState(2, Player.Human);
        state.Board[0] = new[] { Player.Human, Player.AI };
        state.Board[1] = new[] { Player.AI, Player.None };
        state.CurrentPlayer = Player.Human;

        var move = new Move
        {
            X = 1,
            Y = 1,
            Player = Player.Human
        };

        var rules = new GameStateProcessor();

        var newState = rules.ApplyMove(state, move);

        Assert.Equal(Player.None, newState.CurrentPlayer);
        Assert.True(newState.IsGameOver);
        Assert.Equal(Player.None, newState.Winner);
    }

    [Fact]
    public void ApplyMove_MoveWithPoint_ApplyPointsAndUpdatesBoard()
    {
        var state = new GameState(3, Player.Human);

        state.Board[0] = new[] { Player.None, Player.Human, Player.None };
        state.Board[1] = new[] { Player.None, Player.AI, Player.Human };
        state.Board[2] = new[] { Player.None, Player.Human, Player.None };

        state.CurrentPlayer = Player.Human;

        var move = new Move
        {
            X = 1,
            Y = 0,
            Player = Player.Human
        };

        var rules = new GameStateProcessor();

        var newState = rules.ApplyMove(state, move);

        Assert.Equal(Player.Human, newState.Board[1][1]);
        Assert.Equal(1, newState.Scores[Player.Human]);
    }
}
