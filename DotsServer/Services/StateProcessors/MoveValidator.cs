using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services.StateProcessors;

public interface IMoveValidator
{
    public MoveValidation GetMoveValidation(GameState state, Move move);
}

public class MoveValidator: IMoveValidator
{
    public MoveValidation GetMoveValidation(GameState state, Move move)
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