using DotsWebApi.Model;

namespace DotsWebApi.Repositories;

public class InMemoryGameRepository : IGameRepository
{
    private readonly Dictionary<string, GameState> _games = new();
    public void Add(string gameId, GameState state) => _games[gameId] = state;
    public GameState? Get(string gameId) => _games.TryGetValue(gameId, out var state) ? state : null;
    public bool Exists(string gameId) => _games.ContainsKey(gameId);
    public void Update(string gameId, GameState state) => _games[gameId] = state;
}