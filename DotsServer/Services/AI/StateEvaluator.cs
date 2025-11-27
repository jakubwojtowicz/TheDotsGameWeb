using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services.AI.Heuristics;

namespace DotsWebApi.Services.AI;

public interface IStateEvaluator
{
    public int Evaluate(GameState state);
}

public class StateEvaluator : IStateEvaluator
{
    private readonly List<IHeuristic> _heuristics;

    public StateEvaluator(IEnumerable<IHeuristic> heuristics)
    {
        _heuristics = heuristics.ToList();
    }
    public int Evaluate(GameState state)
    {
        int score = 0;
        foreach(var h in _heuristics)
            score += h.Evaluate(state);
        
        return score;
    }
}