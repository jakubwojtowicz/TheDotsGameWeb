using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Model.Helpers;

namespace DotsWebApi.Services.GameEngine;

public interface IGameEngine
{ 
    public GameState ApplyMove(GameState prevState, Move move);
    public UndoMoveData ApplyMoveWithUndo(GameState prevState, Move move);
    public void UndoMove(GameState state, UndoMoveData undo);
    public MoveValidation ValidateMove(GameState state, Move move);
}

public class GameEngine : IGameEngine
{
    private readonly IEnclosureDetector _enclosureDetector;
    private readonly IGameResultProvider _gameResultProvider;
    public GameEngine(IEnclosureDetector enclosureDetector, IGameResultProvider gameResultProvider)
    {
        _enclosureDetector = enclosureDetector;
        _gameResultProvider = gameResultProvider;
    }

    public GameState ApplyMove(GameState prevState, Move move)
    {
        var newState = prevState.Clone();
        
        newState.Board[move.X][move.Y].Player = move.Player;
        newState.CurrentPlayer = move.Player == Player.Human ? Player.AI : Player.Human;
        newState.LastMove = move;

        var enclosed = _enclosureDetector.GetEnclosedFields(newState, move.Player);

        if(enclosed.Count > 0){
            foreach (var (r, c) in enclosed)
            {
                if(newState.Board[r][c].Player != Player.None)
                    newState.Scores[move.Player] += 1;
                newState.Board[r][c].EnclosedBy = move.Player;
            }
        }

        newState.IsGameOver = _gameResultProvider.IsGameOver(newState);

        if (newState.IsGameOver){
            newState.CurrentPlayer = Player.None;
            newState.Winner = _gameResultProvider.GetWinner(newState);
        }

        return newState;
    }

    public UndoMoveData ApplyMoveWithUndo(GameState state, Move move)
    {
        var undoMoveData = new UndoMoveData
        {
            PreviousCurrentPlayer = state.CurrentPlayer,
            PreviousIsGameOver = state.IsGameOver,
            PreviousWinner = state.Winner,
            PreviousScores = new Dictionary<Player, int>(state.Scores)
        };
        
        var targetField = state.Board[move.X][move.Y];
        undoMoveData.ChangedFields.Add((move.X, move.Y, targetField.Player, targetField.EnclosedBy));
        state.Board[move.X][move.Y].Player = move.Player;
        state.CurrentPlayer = move.Player == Player.Human ? Player.AI : Player.Human;
        state.LastMove = move;

        var enclosed = _enclosureDetector.GetEnclosedFields(state, move.Player);

        if(enclosed.Count > 0){
            foreach (var (r, c) in enclosed)
            {
                if(state.Board[r][c].Player != Player.None)
                    state.Scores[move.Player] += 1;
                var changedField = state.Board[r][c];
                undoMoveData.ChangedFields.Add((r, c, changedField.Player, changedField.EnclosedBy));
                state.Board[r][c].EnclosedBy = move.Player;
            }
        }

        state.IsGameOver = _gameResultProvider.IsGameOver(state);

        if (state.IsGameOver){
            state.CurrentPlayer = Player.None;
            state.Winner = _gameResultProvider.GetWinner(state);
        }

        return undoMoveData;
    }

    public void UndoMove(GameState state, UndoMoveData undo)
    {
        foreach (var (r, c, prevPlayer, prevEnclosedBy) in undo.ChangedFields)
        {
            state.Board[r][c].Player = prevPlayer;
            state.Board[r][c].EnclosedBy = prevEnclosedBy;
        }

        state.CurrentPlayer = undo.PreviousCurrentPlayer;
        state.IsGameOver = undo.PreviousIsGameOver;
        state.Winner = undo.PreviousWinner;
        state.Scores[Player.Human] = undo.PreviousScores[Player.Human];
        state.Scores[Player.AI] = undo.PreviousScores[Player.AI];
    }

    public MoveValidation ValidateMove(GameState state, Move move)
    {
        var validation = new MoveValidation();

        validation.IsValid = true;

        if (state.IsGameOver)
        {
            validation.IsValid = false;
            validation.Message = "The game has already ended.";   
        }
        else if(state.CurrentPlayer != move.Player)
        {
            validation.IsValid = false;
            validation.Message = $"It's not the {move.Player} player's turn.";
        }
        else if(move.X < 0 || move.X >= state.Board.Length || move.Y < 0 || move.Y >= state.Board.Length)
        {
            validation.IsValid = false;
            validation.Message = "Move is out of the board's bounds.";
        }
        else if(state.Board[move.X][move.Y].Player != Player.None)
        {
            validation.IsValid = false;
            validation.Message = "The cell is already occupied.";
        }
        else if(state.Board[move.X][move.Y].EnclosedBy != Player.None)
        {
            validation.IsValid = false;
            validation.Message = "The cell is enclosed. You are not allowed to place your dot in enclosed territory.";
        }
            
        return validation;
    }
}