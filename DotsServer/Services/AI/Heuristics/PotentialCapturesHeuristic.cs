using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;

namespace DotsWebApi.Services.AI.Heuristics;

public class PotentialCapturesHeuristic : IHeuristic
{
    private readonly int weight = 30;

    public int Evaluate(GameState state)
    {
        throw new NotImplementedException();
    }
}