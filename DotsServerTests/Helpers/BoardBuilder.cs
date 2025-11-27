using DotsWebApi.Model;
using DotsWebApi.Model.Enums;

namespace DotsWebApiTests.Helpers;

public static class BoardFactory
{
    public static GameState Create(params string[] rows)
    {
        int size = rows.Length;
        var state = new GameState(size, Player.Human);

        for (int r = 0; r < size; r++)
        {
            var cols = rows[r].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int c = 0; c < size; c++)
            {
                char symbol = cols[c][0];
                var field = state.Board[r][c];

                field.Player = symbol switch
                {
                    'H' => Player.Human,
                    'A' => Player.AI,
                    'E' => Player.None,
                    _   => Player.None
                };
            }
        }

        return state;
    }
}
