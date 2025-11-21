using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services;
using DotsWebApi.Exceptions;
using DotsWebApi.Repositories;
using DotsWebApi.DTO;
using DotsWebApi.Services.AI;
using Xunit;
using Moq;

namespace DotsServerTests;

public class GameServiceTests
{
    private readonly Mock<IGameRules> _gameRules = new();
    private readonly Mock<IAIStrategy> _aiStrategy = new();
    private readonly Mock<IMoveApplier> _moveApplier = new();
    private readonly Mock<IGameRepository> _gameRepository = new();

    [Fact]
    public void CreateGame_ReturnsGameIdAndStoresGameState()
    {
        var service = new GameService(_gameRules.Object, _aiStrategy.Object, _moveApplier.Object, _gameRepository.Object);

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
        var service = new GameService(_gameRules.Object, _aiStrategy.Object, _moveApplier.Object, _gameRepository.Object);

        var notExistingId = "1234";

        _gameRepository.Setup(r => r.Get(notExistingId))
            .Returns(() => null);

        Assert.Throws<GameNotFoundException>(() => service.GetGameState(notExistingId));
    }
}

