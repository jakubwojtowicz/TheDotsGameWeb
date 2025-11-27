using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services;
using DotsWebApi.Exceptions;
using DotsWebApi.Repositories;
using DotsWebApi.DTO;
using DotsWebApi.Services.AI;
using Xunit;
using Moq;

namespace DotsServerTests.Tests.Services;

public class GameServiceTests
{
    private readonly Mock<IGameStateProcessor> _gameStateProcessor = new();
    private readonly Mock<IAIStrategy> _aiStrategy = new();
    private readonly Mock<IGameRepository> _gameRepository = new();
    private readonly Mock<IMoveValidator> _moveValidator = new();

    private readonly GameService _gameService;

    public GameServiceTests()
    {
        _gameService = new GameService(_gameStateProcessor.Object,
            _aiStrategy.Object, 
            _gameRepository.Object, 
            _moveValidator.Object);
    }

    [Fact]
    public void CreateGame_ReturnsGameIdAndStoresGameState()
    {
        var gameId = _gameService.CreateGame(3, Player.Human);

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(new GameState(3, Player.Human));

        var state = _gameService.GetGameState(gameId);

        Assert.NotNull(gameId);
        Assert.Equal(3, state.Board.Length);
        Assert.Equal(Player.Human, state.CurrentPlayer);
    }

    [Fact]
    public void GetGameState_InvalidGameId_ThrowsException()
    {
        var notExistingId = "1234";

        _gameRepository.Setup(r => r.Get(notExistingId))
            .Returns(() => null);

        Assert.Throws<GameNotFoundException>(() => _gameService.GetGameState(notExistingId));
    }

    [Fact]
    public void GetGameState_ValidGameId_ReturnGameState()
    {
        var existingId = "1234";

        _gameRepository.Setup(r => r.Get(existingId))
            .Returns(new GameState(3, Player.Human));

        var state = _gameService.GetGameState(existingId);

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
        newState.Board[0][0].Player = Player.Human;
        newState.CurrentPlayer = Player.AI;
        newState.LastMove = new Move { Player = Player.Human, X = 0, Y = 0 };

        _gameStateProcessor.Setup(r => r.GetNextState(initialState, 
            It.Is<Move>(m => m.X == 0 && m.Y == 0)))
            .Returns(newState);

        _gameRepository.Setup(r => r.Update(gameId, newState));

        var dto = new MoveDto { X = 0, Y = 0 };

        var returnedState = _gameService.MakeMove(gameId, dto);

        Assert.Equal(Player.Human, returnedState.Board[0][0].Player);
        Assert.Equal(Player.AI, returnedState.CurrentPlayer);
        Assert.NotNull(returnedState.LastMove);

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

        var dto = new MoveDto { X = 0, Y = 0 };

        Assert.Throws<InvalidMoveException>(() => _gameService.MakeMove(gameId, dto));
    }

    [Fact]
    public void MakeMove_InvalidGameId_ThrowsException()
    {
        var gameId = "game123";

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(() => null);

        var dto = new MoveDto { X = 0, Y = 0 };

        Assert.Throws<GameNotFoundException>(() => _gameService.MakeMove(gameId, dto));
    }

    [Fact]
    public void MakeAIMove_InvalidGameId_ThrowsException()
    {
        var gameId = "game123";

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(() => null);

        Assert.Throws<GameNotFoundException>(() => _gameService.MakeAIMove(gameId));
    }

    [Fact]
    public void MakeAIMove_GameOver_ThrowsException()
    {
        var gameId = "game123";

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(new GameState(3, Player.Human){IsGameOver = true});

        Assert.Throws<InvalidOperationException>(() => _gameService.MakeAIMove(gameId));
    }

    [Fact]
    public void MakeAIMove_HumanTurn_ThrowsException()
    {
        var gameId = "game123";

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(new GameState(3, Player.Human){IsGameOver = false, CurrentPlayer = Player.Human});

        Assert.Throws<InvalidOperationException>(() => _gameService.MakeAIMove(gameId));
    }

    [Fact]
    public void MakeAIMove_ValidGame_ReturnsNewState()
    {
        var gameId = "game123";

        var initState = new GameState(3, Player.AI);

        var newState = initState.Clone();
        newState.Board[0][0].Player = Player.AI;
        newState.CurrentPlayer = Player.Human;
        newState.LastMove = new Move { Player = Player.AI, X = 0, Y = 0 };

        var aiMove = new Move { X = 0, Y = 0, Player = Player.AI };

        _aiStrategy.Setup(r => r.GetNextMove(initState))
            .Returns(aiMove);

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(initState);

        _gameStateProcessor.Setup(r => r.GetNextState(initState, aiMove))
            .Returns(newState);

        var returnedState = _gameService.MakeAIMove(gameId);

        Assert.Equal(Player.AI, returnedState.Board[0][0].Player);
        Assert.Equal(Player.Human, returnedState.CurrentPlayer);
        Assert.NotNull(returnedState.LastMove);

        _gameRepository.Verify(r => r.Update(gameId, newState), Times.Once);
    }
}

