using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services.AI.Heuristics;

public class ScoreHeuristic : IHeuristic
{
    private readonly int weight = 1000;
    public int Evaluate(GameState state)
    {
        return weight * (state.Scores[Player.AI] - state.Scores[Player.Human]);
    }
}