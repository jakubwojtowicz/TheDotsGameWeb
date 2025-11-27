using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Repositories;
using Xunit;

namespace DotsServerTests.Tests.Repositories;

public class InMemoryGameRepositoryTests
{
    private readonly InMemoryGameRepository _repository;

    public InMemoryGameRepositoryTests()
    {
        _repository = new InMemoryGameRepository();
    }

    [Fact]
    public void Add_Game_CanBeRetrieved()
    {
        var gameId = "game1";
        var state = new GameState(3, Player.Human);

        _repository.Add(gameId, state);
        var retrieved = _repository.Get(gameId);

        Assert.NotNull(retrieved);
        Assert.Equal(state, retrieved);
    }

    [Fact]
    public void Get_NonExistingGame_ReturnsNull()
    {
        var retrieved = _repository.Get("unknown");

        Assert.Null(retrieved);
    }

    [Fact]
    public void Exists_ReturnsTrueForExistingGame()
    {
        var gameId = "game2";
        var state = new GameState(3, Player.Human);
        _repository.Add(gameId, state);

        var exists = _repository.Exists(gameId);

        Assert.True(exists);
    }

    [Fact]
    public void Exists_ReturnsFalseForNonExistingGame()
    {
        var exists = _repository.Exists("unknown");

        Assert.False(exists);
    }

    [Fact]
    public void Update_ExistingGame_ChangesState()
    {
        var gameId = "game3";
        var original = new GameState(3, Player.Human);
        _repository.Add(gameId, original);

        var updated = original.Clone();
        updated.CurrentPlayer = Player.AI;

        _repository.Update(gameId, updated);
        var retrieved = _repository.Get(gameId);

        Assert.NotNull(retrieved);
        Assert.Equal(Player.AI, retrieved.CurrentPlayer);
    }

    [Fact]
    public void Update_NonExistingGame_AddsGame()
    {
        var gameId = "newgame";
        var state = new GameState(3, Player.Human);

        _repository.Update(gameId, state);
        var retrieved = _repository.Get(gameId);

        Assert.NotNull(retrieved);
        Assert.Equal(state, retrieved);
    }
}
