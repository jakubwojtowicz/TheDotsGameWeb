using DotsWebApi.Model;         
using DotsWebApi.Model.Enums;    
using DotsWebApi.Services;
using Moq;
using Xunit;

namespace DotsServerTests.Services;

public class GameStateProcessorTests
{
    private readonly Mock<IMoveResolver> _moveResolver = new();
    private readonly Mock<IGameResultProvider> _resultProvider = new();
    private readonly GameStateProcessor _stateProcessor;

    public GameStateProcessorTests()
    {
        _stateProcessor = new GameStateProcessor(_moveResolver.Object, _resultProvider.Object);
    }

    [Fact]
    public void GetNextState_HumanMove_ReturnsNewState()
    {
        _moveResolver.Setup(r => r.GetMoveResult(
            It.IsAny<GameState>(), 
            It.IsAny<Player>(),
            It.IsAny<Player>()))
            .Returns(
                new MoveResult {
                    Score = 0, 
                    Player = Player.Human});

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

        var newState = _stateProcessor.GetNextState(state, move);

        Assert.Equal(Player.AI, newState.CurrentPlayer);
        Assert.Equal(move, newState.LastMove);
        Assert.False(newState.IsGameOver);
        Assert.Equal(Player.Human, newState.Board[0][1]);
    }

    [Fact]
    public void GetNextState_AIMove_ReturnsNewState()
    {
        _moveResolver.Setup(r => r.GetMoveResult(
            It.IsAny<GameState>(), 
            It.IsAny<Player>(),
            It.IsAny<Player>()))
            .Returns(
                new MoveResult {
                    Score = 0, 
                    Player = Player.AI});

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

        var newState = _stateProcessor.GetNextState(state, move);

        Assert.Equal(Player.Human, newState.CurrentPlayer);
        Assert.Equal(move, newState.LastMove);
        Assert.False(newState.IsGameOver);
        Assert.Equal(Player.AI, newState.Board[0][1]);
    }

    [Fact]
    public void GetNextState_LastMove_SetsGameEndedState()
    {
        _moveResolver.Setup(r => r.GetMoveResult(
            It.IsAny<GameState>(), 
            It.IsAny<Player>(),
            It.IsAny<Player>()))
            .Returns(
                new MoveResult {
                    Score = 0, 
                    Player = Player.Human});

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

        var newState = _stateProcessor.GetNextState(state, move);

        Assert.Equal(Player.None, newState.CurrentPlayer);
        Assert.True(newState.IsGameOver);
        Assert.Equal(Player.Human, newState.Winner);
    }

    [Fact]
    public void GetNextState_MoveWithPoint_ApplyPointsAndUpdatesBoard()
    {
        _moveResolver.Setup(r => r.GetMoveResult(
            It.IsAny<GameState>(), 
            It.IsAny<Player>(),
            It.IsAny<Player>()))
            .Returns(
                new MoveResult {
                    Score = 1, 
                    Player = Player.Human,
                    Captured = new List<(int r, int c)>{(1,1)}});

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
            X = 1,
            Y = 0,
            Player = Player.Human
        };

        var newState = _stateProcessor.GetNextState(state, move);

        Assert.Equal(Player.Human, newState.Board[1][1]);
        Assert.Equal(1, newState.Scores[Player.Human]);
    }
}
