using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services;
using DotsWebApi.Exceptions;
using DotsWebApi.Repositories;
using DotsWebApi.DTO;
using DotsWebApi.Services.AI;
using Xunit;
using Moq;
using DotsWebApi.Services.GameEngine;
using System.Threading.Tasks;

namespace DotsServerTests.Tests.Services;

public class GameServiceTests
{
    private readonly Mock<IGameEngine> _gameEngine = new();
    private readonly Mock<IAIStrategy> _aiStrategy = new();
    private readonly Mock<IGameRepository> _gameRepository = new();

    private readonly GameService _gameService;

    public GameServiceTests()
    {
        _gameService = new GameService(_gameEngine.Object,
            _aiStrategy.Object, 
            _gameRepository.Object);
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
    public async Task MakeMove_ValidMove_ReturnsUpdatedStateAsync()
    {
        var gameId = "game123";
        var initialState = new GameState(3, Player.Human);

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(initialState);

        _gameEngine.Setup(r => r.ValidateMove(
            It.IsAny<GameState>(), 
            It.IsAny<Move>()))
            .Returns(new MoveValidation { IsValid = true });

        var newState = initialState.Clone();
        newState.Board[0][0].Player = Player.Human;
        newState.CurrentPlayer = Player.AI;
        newState.LastMove = new Move { Player = Player.Human, X = 0, Y = 0 };

        _gameEngine.Setup(r => r.ApplyMove(initialState, 
            It.Is<Move>(m => m.X == 0 && m.Y == 0)))
            .Returns(newState);

        _gameRepository.Setup(r => r.Update(gameId, newState));

        var dto = new MoveDto { X = 0, Y = 0 };

        var returnedState = await _gameService.MakeMoveAsync(gameId, dto);

        Assert.Equal(Player.Human, returnedState.Board[0][0].Player);
        Assert.Equal(Player.AI, returnedState.CurrentPlayer);
        Assert.NotNull(returnedState.LastMove);

        _gameRepository.Verify(r => r.Update(gameId, newState), Times.Once);
    }

    [Fact]
    public async Task MakeMove_InvalidMove_ThrowsException()
    {
        var gameId = "game123";
        var initialState = new GameState(3, Player.Human);

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(initialState);

        _gameEngine.Setup(r => r.ValidateMove(
            It.IsAny<GameState>(), 
            It.IsAny<Move>()))
            .Returns(new MoveValidation { IsValid = false, Message = "Invalid move." });

        var dto = new MoveDto { X = 0, Y = 0 };

        await Assert.ThrowsAsync<InvalidMoveException>(() => _gameService.MakeMoveAsync(gameId, dto));
    }

    [Fact]
    public async Task MakeMove_InvalidGameId_ThrowsException()
    {
        var gameId = "game123";

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(() => null);

        var dto = new MoveDto { X = 0, Y = 0 };

        await Assert.ThrowsAsync<GameNotFoundException>(() => _gameService.MakeMoveAsync(gameId, dto));
    }

    [Fact]
    public async Task MakeAIMove_InvalidGameId_ThrowsException()
    {
        var gameId = "game123";

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(() => null);

        await Assert.ThrowsAsync<GameNotFoundException>(() => _gameService.MakeAIMoveAsync(gameId));
    }

    [Fact]
    public async Task MakeAIMove_GameOver_ThrowsExceptionAsync()
    {
        var gameId = "game123";

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(new GameState(3, Player.Human){IsGameOver = true});

        await Assert.ThrowsAsync<InvalidOperationException>(() => _gameService.MakeAIMoveAsync(gameId));
    }

    [Fact]
    public async Task MakeAIMove_HumanTurn_ThrowsException()
    {
        var gameId = "game123";

        _gameRepository.Setup(r => r.Get(gameId))
            .Returns(new GameState(3, Player.Human){IsGameOver = false, CurrentPlayer = Player.Human});

        await Assert.ThrowsAsync<InvalidOperationException>(() => _gameService.MakeAIMoveAsync(gameId));
    }

    [Fact]
    public async Task MakeAIMove_ValidGame_ReturnsNewStateAsync()
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

        _gameEngine.Setup(r => r.ApplyMove(initState, aiMove))
            .Returns(newState);

        var returnedState = await _gameService.MakeAIMoveAsync(gameId);

        Assert.Equal(Player.AI, returnedState.Board[0][0].Player);
        Assert.Equal(Player.Human, returnedState.CurrentPlayer);
        Assert.NotNull(returnedState.LastMove);

        _gameRepository.Verify(r => r.Update(gameId, newState), Times.Once);
    }
}

