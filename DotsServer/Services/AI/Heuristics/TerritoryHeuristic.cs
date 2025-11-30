using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services.AI.Heuristics;

public class TerritoryHeuristic : IHeuristic
{
    private int weight = 100;
    public int Evaluate(GameState state)
    {
        int humanTerritory = 0;
        int aiTerritory = 0;
        int rows = state.Board.Length;
        int cols = state.Board[0].Length;

        for (int r = 0; r < rows; r++)
        for (int c = 0; c < cols; c++)
        {
            if (state.Board[r][c].EnclosedBy == Player.AI)
            {
                aiTerritory ++;
            }
            else if(state.Board[r][c].EnclosedBy == Player.Human)
            {
                humanTerritory ++;
            }
        }

        return weight * (aiTerritory - humanTerritory);
    }
}