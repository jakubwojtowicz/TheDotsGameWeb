using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services;
using DotsWebApi.Exceptions;
using DotsWebApi.Repositories;
using DotsWebApi.DTO;
using DotsWebApi.Services.AI;
using Xunit;
using Moq;

namespace DotsServerTests.Services;

public class GameServiceTests
{
    private readonly Mock<IGameStateProcessor> _gameStateProcessor = new();
    private readonly Mock<IAIStrategy> _aiStrategy = new();
    private readonly Mock<IGameRepository> _gameRepository = new();
    private readonly Mock<IMoveValidator> _moveValidator = new();

    [Fact]
    public void CreateGame_ReturnsGameIdAndStoresGameState()
    {
        var service = new GameService(_gameStateProcessor.Object, 
            _aiStrategy.Object,
            _gameRepository.Object,
            _moveValidator.Object);

        var gameId = service.CreateGame(3, Player.Human);

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(new GameState(3, Player.Human));

        var state = service.GetGameState(gameId);

        Assert.NotNull(gameId);
        Assert.Equal(3, state.Board.Length);
        Assert.Equal(Player.Human, state.CurrentPlayer);
    }

    [Fact]
    public void GetGameState_InvalidGameId_ThrowsException()
    {
        var service = new GameService(_gameStateProcessor.Object, 
            _aiStrategy.Object,
            _gameRepository.Object,
            _moveValidator.Object);

        var notExistingId = "1234";

        _gameRepository.Setup(r => r.Get(notExistingId))
            .Returns(() => null);

        Assert.Throws<GameNotFoundException>(() => service.GetGameState(notExistingId));
    }

    [Fact]
    public void GetGameState_ValidGameId_ReturnGameState()
    {
        var service = new GameService(_gameStateProcessor.Object, 
            _aiStrategy.Object,
            _gameRepository.Object,
            _moveValidator.Object);

        var existingId = "1234";

        _gameRepository.Setup(r => r.Get(existingId))
            .Returns(new GameState(3, Player.Human));

        var state = service.GetGameState(existingId);

        Assert.Equal(3, state.Board.Length);
        Assert.Equal(Player.Human, state.CurrentPlayer);
    }

    [Fact]
    public void MakeMove_ValidMove_ReturnsUpdatedState()
    {
        var gameId = "game123";
        var initialState = new GameState(3, Player.Human);

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(initialState);

        _moveValidator.Setup(r => r.GetMoveValidation(
            It.IsAny<GameState>(), 
            It.IsAny<Move>()))
            .Returns(new MoveValidation { IsValid = true });

        var newState = initialState.Clone();
        newState.Board[0][0] = Player.Human;
        newState.CurrentPlayer = Player.AI;
        newState.LastMove = new Move { Player = Player.Human, X = 0, Y = 0 };
        newState.LastMoveResult = new MoveResult { Score = 0 };

        _gameStateProcessor.Setup(r => r.GetNextState(initialState, 
            It.Is<Move>(m => m.X == 0 && m.Y == 0)))
            .Returns(newState);

        _gameRepository.Setup(r => r.Update(gameId, newState));

        var service = new GameService(_gameStateProcessor.Object, 
            _aiStrategy.Object,
            _gameRepository.Object,
            _moveValidator.Object);

        var dto = new MoveDto { X = 0, Y = 0 };

        var returnedState = service.MakeMove(gameId, dto);

        Assert.Equal(Player.Human, returnedState.Board[0][0]);
        Assert.Equal(Player.AI, returnedState.CurrentPlayer);
        Assert.NotNull(returnedState.LastMove);
        Assert.NotNull(returnedState.LastMoveResult);

        _gameRepository.Verify(r => r.Update(gameId, newState), Times.Once);
    }

    [Fact]
    public void MakeMove_InvalidMove_ThrowsException()
    {
        var gameId = "game123";
        var initialState = new GameState(3, Player.Human);

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(initialState);

        _moveValidator.Setup(r => r.GetMoveValidation(
            It.IsAny<GameState>(), 
            It.IsAny<Move>()))
            .Returns(new MoveValidation { IsValid = false, Message = "Invalid move." });

        var service = new GameService(_gameStateProcessor.Object, 
            _aiStrategy.Object,
            _gameRepository.Object,
            _moveValidator.Object);

        var dto = new MoveDto { X = 0, Y = 0 };

        Assert.Throws<InvalidMoveException>(() => service.MakeMove(gameId, dto));
    }

    [Fact]
    public void MakeMove_InvalidGameId_ThrowsException()
    {
        var gameId = "game123";

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(() => null);

        var service = new GameService(_gameStateProcessor.Object, 
            _aiStrategy.Object,
            _gameRepository.Object,
            _moveValidator.Object);

        var dto = new MoveDto { X = 0, Y = 0 };

        Assert.Throws<GameNotFoundException>(() => service.MakeMove(gameId, dto));
    }

    [Fact]
    public void MakeAIMove_InvalidGameId_ThrowsException()
    {
        var gameId = "game123";

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(() => null);

        var service = new GameService(_gameStateProcessor.Object, 
            _aiStrategy.Object,
            _gameRepository.Object,
            _moveValidator.Object);

        Assert.Throws<GameNotFoundException>(() => service.MakeAIMove(gameId));
    }

    [Fact]
    public void MakeAIMove_GameOver_ThrowsException()
    {
        var gameId = "game123";

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(new GameState(3, Player.Human){IsGameOver = true});

        var service = new GameService(_gameStateProcessor.Object, 
            _aiStrategy.Object,
            _gameRepository.Object,
            _moveValidator.Object);

        Assert.Throws<InvalidOperationException>(() => service.MakeAIMove(gameId));
    }

    [Fact]
    public void MakeAIMove_HumanTurn_ThrowsException()
    {
        var gameId = "game123";

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(new GameState(3, Player.Human){IsGameOver = false, CurrentPlayer = Player.Human});

        var service = new GameService(_gameStateProcessor.Object, 
            _aiStrategy.Object,
            _gameRepository.Object,
            _moveValidator.Object);

        Assert.Throws<InvalidOperationException>(() => service.MakeAIMove(gameId));
    }

    [Fact]
    public void MakeAIMove_ValidGame_ReturnsNewState()
    {
        var gameId = "game123";

        var initState = new GameState(3, Player.AI);

        var newState = initState.Clone();
        newState.Board[0][0] = Player.AI;
        newState.CurrentPlayer = Player.Human;
        newState.LastMove = new Move { Player = Player.AI, X = 0, Y = 0 };
        newState.LastMoveResult = new MoveResult { Score = 0 };

        var aiMove = new Move { X = 0, Y = 0, Player = Player.AI };

        _aiStrategy.Setup(r => r.GetNextMove(initState))
            .Returns(aiMove);

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(initState);

        _gameStateProcessor.Setup(r => r.GetNextState(initState, aiMove))
            .Returns(newState);

        var service = new GameService(_gameStateProcessor.Object, 
            _aiStrategy.Object,
            _gameRepository.Object,
            _moveValidator.Object);

        var returnedState = service.MakeAIMove(gameId);

        Assert.Equal(Player.AI, returnedState.Board[0][0]);
        Assert.Equal(Player.Human, returnedState.CurrentPlayer);
        Assert.NotNull(returnedState.LastMove);
        Assert.NotNull(returnedState.LastMoveResult);

        _gameRepository.Verify(r => r.Update(gameId, newState), Times.Once);
    }
}

