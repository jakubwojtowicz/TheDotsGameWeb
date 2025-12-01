using DotsWebApi.Model;         
using DotsWebApi.Model.Enums;    
using DotsWebApi.Services;
using DotsWebApi.Services.GameEngine;
using DotsWebApiTests.Helpers;
using Moq;
using Xunit;

namespace DotsServerTests.Tests.Services;

public class GameEngineTests
{
    private readonly Mock<IEnclosureDetector> _enclosureDetector = new();
    private readonly Mock<IGameResultProvider> _resultProvider = new();
    private readonly GameEngine _gameEngine;

    public GameEngineTests()
    {
        _gameEngine = new GameEngine(_enclosureDetector.Object, _resultProvider.Object);
    }

    [Fact]
    public void ApplyMove_HumanMove_ReturnsNewState()
    {
        _enclosureDetector.Setup(r => r.GetEnclosedFields(
            It.IsAny<GameState>(), 
            It.IsAny<Player>()))
            .Returns(
                new List<(int r, int c)>());

        _resultProvider.Setup(r => r.GetWinner(
            It.IsAny<GameState>()))
            .Returns(
                Player.None);
        
        _resultProvider.Setup(r => r.IsGameOver(
            It.IsAny<GameState>()))
            .Returns(
                () => false
            );

        var state = new GameState(3, Player.Human);

        var move = new Move
        {
            X = 0,
            Y = 1,
            Player = Player.Human
        };

        var newState = _gameEngine.ApplyMove(state, move);

        Assert.Equal(Player.AI, newState.CurrentPlayer);
        Assert.Equal(move, newState.LastMove);
        Assert.False(newState.IsGameOver);
        Assert.Equal(Player.Human, newState.Board[0][1].Player);
    }

    [Fact]
    public void ApplyMove_AIMove_ReturnsNewState()
    {
        _enclosureDetector.Setup(r => r.GetEnclosedFields(
            It.IsAny<GameState>(), 
            It.IsAny<Player>()))
            .Returns(
                new List<(int r, int c)>());

        _resultProvider.Setup(r => r.GetWinner(
            It.IsAny<GameState>()))
            .Returns(
                Player.None);
        
        _resultProvider.Setup(r => r.IsGameOver(
            It.IsAny<GameState>()))
            .Returns(
                () => false
            );

        var state = new GameState(3, Player.AI);

        var move = new Move
        {
            X = 0,
            Y = 1,
            Player = Player.AI
        };

        var newState = _gameEngine.ApplyMove(state, move);

        Assert.Equal(Player.Human, newState.CurrentPlayer);
        Assert.Equal(move, newState.LastMove);
        Assert.False(newState.IsGameOver);
        Assert.Equal(Player.AI, newState.Board[0][1].Player);
    }

    [Fact]
    public void ApplyMove_LastMove_SetsGameEndedState()
    {
        _enclosureDetector.Setup(r => r.GetEnclosedFields(
            It.IsAny<GameState>(), 
            It.IsAny<Player>()))
            .Returns(
                new List<(int r, int c)>());

        _resultProvider.Setup(r => r.GetWinner(
            It.IsAny<GameState>()))
            .Returns(
                Player.Human);
        
        _resultProvider.Setup(r => r.IsGameOver(
            It.IsAny<GameState>()))
            .Returns(
                () => true
            );

        var state = new GameState(2, Player.Human);

        var move = new Move
        {
            X = 1,
            Y = 1,
            Player = Player.Human
        };

        var newState = _gameEngine.ApplyMove(state, move);

        Assert.Equal(Player.None, newState.CurrentPlayer);
        Assert.True(newState.IsGameOver);
        Assert.Equal(Player.Human, newState.Winner);
    }

    [Fact]
    public void ApplyMove_MoveWithPoint_ApplyPointsAndUpdatesBoard()
    {
        _enclosureDetector.Setup(r => r.GetEnclosedFields(
            It.IsAny<GameState>(), 
            It.IsAny<Player>()))
            .Returns(
                new List<(int r, int c)>{(1,1)});

        _resultProvider.Setup(r => r.GetWinner(
            It.IsAny<GameState>()))
            .Returns(
                Player.None);
        
        _resultProvider.Setup(r => r.IsGameOver(
            It.IsAny<GameState>()))
            .Returns(
                () => false
            );

        var state = new GameState(3, Player.Human);

        state.Board[1][1].Player = Player.AI;

        var move = new Move
        {
            X = 1,
            Y = 0,
            Player = Player.Human
        };

        var newState = _gameEngine.ApplyMove(state, move);

        Assert.Equal(1, newState.Scores[Player.Human]);
        Assert.Equal(Player.Human, newState.Board[1][1].EnclosedBy);
    }

        [Fact]
    public void ValidateMove_ValidMove_ReturnsValid()
    {
        var state = new GameState(3, Player.Human);

        var move = new Move
        {
            X = 1,
            Y = 1,
            Player = Player.Human,
        };

        var validation = _gameEngine.ValidateMove(state, move);

        Assert.True(validation.IsValid);
    }

    [Fact]
    public void ValidateMove_WrongPlayer_ReturnsInvalid()
    {
        var state = new GameState(3, Player.Human);

        var move = new Move
        {
            X = 1,
            Y = 1,
            Player = Player.AI,
        };

        var validation = _gameEngine.ValidateMove(state, move);

        Assert.False(validation.IsValid);
    }

    [Fact]
    public void ValidateMove_GameEnded_ReturnsInvalid()
    {
        var state = new GameState(3, Player.Human);
        state.IsGameOver = true;

        var move = new Move
        {
            X = 1,
            Y = 1,
            Player = Player.Human,
        };

        var validation = _gameEngine.ValidateMove(state, move);

        Assert.False(validation.IsValid);
    }

    [Fact]
    public void ValidateMove_OutOfBounds_ReturnsInvalid()
    {
        var state = new GameState(3, Player.Human);
        state.IsGameOver = false;

        var move = new Move
        {
            X = 50000,
            Y = -10,
            Player = Player.Human,
        };

        var validation = _gameEngine.ValidateMove(state, move);

        Assert.False(validation.IsValid);
    }

    [Fact]
    public void ValidateMove_CellOccupied_ReturnsInvalid()
    {
        var state = BoardFactory.Create(
            "N A N",
            "N N N",
            "N N N"
        );

        var move = new Move
        {
            X = 0,
            Y = 1,
            Player = Player.Human,
        };

        var validation = _gameEngine.ValidateMove(state, move);

        Assert.False(validation.IsValid);
    }

    
    [Fact]
    public void ValidateMove_CellEnclosed_ReturnsInvalid()
    {
        var state = BoardFactory.Create(
            "E A H",
            "A E A",
            "H A H"
        );

        state.Board[1][1].EnclosedBy = Player.AI;

        var move = new Move
        {
            X = 1,
            Y = 1,
            Player = Player.Human,
        };

        var validation = _gameEngine.ValidateMove(state, move);

        Assert.False(validation.IsValid);
    }
}
