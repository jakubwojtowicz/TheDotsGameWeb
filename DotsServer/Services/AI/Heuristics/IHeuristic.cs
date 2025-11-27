using DotsWebApi.Model;

namespace DotsWebApi.Services.AI.Heuristics;

public interface IHeuristic
{
    public int Evaluate(GameState state);
}