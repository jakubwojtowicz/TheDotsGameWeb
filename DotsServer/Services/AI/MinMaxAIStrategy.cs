using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services.AI;
using DotsWebApi.Services.GameEngine;
using Microsoft.AspNetCore.Components.Forms;

namespace DotsWebApi.Services.AI;

public class MinMaxAIStrategy : IAIStrategy
{
    private readonly IGameEngine _gameEngine;
    private readonly IMoveGenerator _moveGenerator;
    private readonly IStateEvaluator _stateEvaluator;

    public MinMaxAIStrategy(IGameEngine gameEngine, IMoveGenerator moveGenerator, IStateEvaluator stateEvaluator)
    {
        _gameEngine = gameEngine;
        _moveGenerator = moveGenerator;
        _stateEvaluator = stateEvaluator;
    }

    public Move GetNextMove(GameState state)
    {
        int bestScore = int.MinValue;
        Move bestMove = new Move();

        var moves = _moveGenerator.GenerateMoves(state);

        foreach (var move in moves)
        {
            var stateAfterAI = _gameEngine.ApplyMove(state, move);

            var moveScore = MinMax(stateAfterAI, 3, int.MinValue, int.MaxValue);

            if (moveScore >= bestScore)
            {
                bestScore = moveScore;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private int MinMax(GameState state, int depth, int alpha, int beta)
    {
        if (depth == 0 || state.IsGameOver)
            return _stateEvaluator.Evaluate(state);

        if (state.CurrentPlayer == Player.AI)
        {
            int maxEval = int.MinValue;
            var moves = _moveGenerator.GenerateMoves(state);

            foreach (var move in moves)
            {
                int eval = MinMax(
                    _gameEngine.ApplyMove(state, move),
                    depth - 1,
                    alpha,
                    beta
                );

                maxEval = Math.Max(eval, maxEval);
                alpha = Math.Max(alpha, eval);

                if (beta <= alpha)
                    break;
            }

            return maxEval;
        }
        else 
        {
            int minEval = int.MaxValue;
            var moves = _moveGenerator.GenerateMoves(state);

            foreach (var move in moves)
            {
                int eval = MinMax(
                    _gameEngine.ApplyMove(state, move),
                    depth - 1,
                    alpha,
                    beta
                );

                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);

                if (beta <= alpha)
                    break;
            }

            return minEval;
        }
    }
}
