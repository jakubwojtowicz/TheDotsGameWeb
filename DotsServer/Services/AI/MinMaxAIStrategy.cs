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
    private readonly ILogger<MinMaxAIStrategy> _logger;

    public MinMaxAIStrategy(IGameEngine gameEngine, IMoveGenerator moveGenerator, IStateEvaluator stateEvaluator, ILogger<MinMaxAIStrategy> logger)
    {
        _gameEngine = gameEngine;
        _moveGenerator = moveGenerator;
        _stateEvaluator = stateEvaluator;
        _logger = logger;
    }

    public Move GetNextMove(GameState state)
    {
        var start_time = DateTime.Now;

        int bestScore = int.MinValue;
        Move bestMove = new Move();
        object lockObj = new object();

        var moves = _moveGenerator.GenerateMoves(state);

        Parallel.ForEach(moves, move =>
        {
            var stateAfterAI = _gameEngine.ApplyMove(state, move);

            int moveScore = MinMax(stateAfterAI, 3, int.MinValue, int.MaxValue);

            lock (lockObj)
            {
                _logger.LogInformation($"Move: {move.X}, {move.Y} score: {moveScore}");

                if (moveScore > bestScore)
                {
                    bestScore = moveScore;
                    bestMove = move;
                }
            }
        });

        var elapsed_time = (DateTime.Now - start_time).TotalSeconds;

        _logger.LogInformation($"MiniMax calculated best move in {elapsed_time} seconds.");

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
                var undo = _gameEngine.ApplyMoveWithUndo(state, move);

                int eval = MinMax(
                    state,
                    depth - 1,
                    alpha,
                    beta
                );

                _gameEngine.UndoMove(state, undo);

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
                var undo = _gameEngine.ApplyMoveWithUndo(state, move);

                int eval = MinMax(
                    state,
                    depth - 1,
                    alpha,
                    beta
                );

                _gameEngine.UndoMove(state, undo);

                minEval = Math.Min(minEval, eval);
                beta = Math.Min(beta, eval);

                if (beta <= alpha)
                    break;
            }

            return minEval;
        }
    }
}
