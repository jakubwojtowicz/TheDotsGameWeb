using DotsWebApi.Model;

namespace DotsWebApi.Services.AI;

public interface IAIStrategy
{
    Move GetNextMove(GameState state);
}