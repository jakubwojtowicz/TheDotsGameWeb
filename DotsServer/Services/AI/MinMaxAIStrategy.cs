using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services.AI;
using DotsWebApi.Services.StateProcessors;
using Microsoft.AspNetCore.Components.Forms;

namespace DotsWebApi.Services.AI;

public class MinMaxAIStrategy : IAIStrategy
{
    private readonly IGameStateProcessor _gameStateProcessor;
    private readonly IMoveGenerator _moveGenerator;
    private readonly IStateEvaluator _stateEvaluator;

    public MinMaxAIStrategy(IGameStateProcessor gameStateProcessor, IMoveGenerator moveGenerator, IStateEvaluator stateEvaluator)
    {
        _gameStateProcessor = gameStateProcessor;
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
            var stateAfterAI = _gameStateProcessor.GetNextState(state, move);

            var moveScore = MinMax(stateAfterAI, 10);

            if (moveScore > bestScore)
            {
                bestScore = moveScore;
                bestMove = move;
            }
        }

        return bestMove;
    }

    private int MinMax(GameState state, int depth)
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
                    _gameStateProcessor.GetNextState(state, move),
                    depth - 1
                );

                maxEval = Math.Max(eval, maxEval);
            }
            return maxEval;
        }
        else if(state.CurrentPlayer == Player.Human)
        {
            int minEval = int.MaxValue;
            var moves = _moveGenerator.GenerateMoves(state);

            foreach (var move in moves)
            {
                int eval = MinMax(
                    _gameStateProcessor.GetNextState(state, move),
                    depth - 1
                );

                minEval = Math.Min(minEval, eval);
            }

            return minEval;
        }
        return 0;
    }
}
