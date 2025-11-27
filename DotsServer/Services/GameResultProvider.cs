using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApi.Services;

public interface IGameResultProvider
{
    public bool IsGameOver(GameState state);
    public Player GetWinner(GameState state);
}

public class GameResultProvider : IGameResultProvider
{
    public bool IsGameOver(GameState state)
    {
        for(int r = 0; r < state.Board.Length; r++)
        {
            for(int c = 0; c < state.Board[r].Length; c++)
            {
                // If any cell is empty, the game is not over
                if(state.Board[r][c].Player == Player.None && state.Board[r][c].Enclosed == false)
                    return false;
            }
        }
        // If no empty cells, the game is over
        return true;
    }

    public Player GetWinner(GameState state)
    {
        if(state.IsGameOver == false)
            return Player.None;
        else if(state.Scores[Player.Human] > state.Scores[Player.AI ])
            return Player.Human;
        else if(state.Scores[Player.AI] > state.Scores[Player.Human])
            return Player.AI;
        else
            return Player.None; // Tie
    }
}