using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services;

public interface IGameEvaluator
{
    GameEvaluation EvaluateGame(GameState state);
}

public class GameEvaluator : IGameEvaluator
{
    public GameEvaluation EvaluateGame(GameState state)
    {
        return new GameEvaluation()
        {
            IsGameOver = false,
            Winner = Player.None,
            Scores = new Dictionary<Player, int>
            {
                { Player.Human, 0 },
                { Player.AI, 0 }
            }
        };
    }
}