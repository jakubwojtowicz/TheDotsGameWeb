using DotsWebApi.Model;

namespace DotsWebApi.Repositories;

public interface IGameRepository
{
    public void Add(string id, GameState state);
    public bool Exists(string id);
    public GameState? Get(string id);
    public void Update(string id, GameState state);
    
}