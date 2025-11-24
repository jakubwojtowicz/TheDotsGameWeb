using DotsWebApi.DTO;
using DotsWebApi.Controllers;
using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DotsServerTests.Controllers;

public class GameControllerTests
{
    private readonly Mock<IGameService> _service;
    private readonly GameController _controller;

    public GameControllerTests()
    {
        _service = new Mock<IGameService>();
        _controller = new GameController(_service.Object);
    }

    [Fact]
    public void CreateGame_ValidBoardSize_ReturnsCreatedResult()
    {
        int boardSize = 5;
        string gameId = "123";
        var state = new GameState(5, Player.Human);

        _service.Setup(s => s.CreateGame(boardSize)).Returns(gameId);
        _service.Setup(s => s.GetGameState(gameId)).Returns(state);

        var result = _controller.CreateGame(boardSize);

        var created = Assert.IsType<CreatedResult>(result.Result);
        Assert.Equal($"api/game/{gameId}", created.Location);

        var returnedState = Assert.IsType<GameState>(created.Value);
        Assert.Equal(state, returnedState);

        _service.Verify(s => s.CreateGame(boardSize), Times.Once);
        _service.Verify(s => s.GetGameState(gameId), Times.Once);
    }

    [Fact]
    public void GetById_ExistingGame_ReturnsOk()
    {
        string id = "123";
        var state = new GameState(5, Player.Human);

        _service.Setup(s => s.GetGameState(id)).Returns(state);

        var result = _controller.GetById(id);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returnedState = Assert.IsType<GameState>(ok.Value);
        Assert.Equal(state, returnedState);

        _service.Verify(s => s.GetGameState(id), Times.Once);
    }

    [Fact]
    public void MakeMove_ValidMove_ReturnsOk()
    {
        string id = "123";
        var moveDto = new MoveDto { X = 1, Y = 2 };
        var expected = new GameState(5, Player.AI);

        _service.Setup(s => s.MakeMove(id, moveDto)).Returns(expected);

        var result = _controller.MakeMove(id, moveDto);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returnedState = Assert.IsType<GameState>(ok.Value);
        Assert.Equal(expected, returnedState);

        _service.Verify(s => s.MakeMove(id, moveDto), Times.Once);
    }

    [Fact]
    public void MakeAIMove_ValidGame_ReturnsOk()
    {
        string id = "123";
        var expected = new GameState(5, Player.AI);

        _service.Setup(s => s.MakeAIMove(id)).Returns(expected);

        var result = _controller.MakeAIMove(id);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returnedState = Assert.IsType<GameState>(ok.Value);
        Assert.Equal(expected, returnedState);

        _service.Verify(s => s.MakeAIMove(id), Times.Once);
    }
}
