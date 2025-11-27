using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services.AI.Heuristics;

public class GameOverHeuristic : IHeuristic
{
    private readonly int weight = 1000000;
    public int Evaluate(GameState state)
    {
        if(state.Winner == Player.Human)
        {
            return -1 * weight;
        }
        else if(state.Winner == Player.AI)
        {
            return 1 * weight;
        }
        else
            return 0;
    }
}