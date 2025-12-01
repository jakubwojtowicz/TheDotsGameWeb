using DotsWebApi.Model;
using DotsWebApi.Model.Enums;
using DotsWebApi.Services;
using DotsWebApi.Services.GameEngine;
using DotsWebApiTests.Helpers;

namespace DotsServerTests.Tests.Services;

public class EnclosureDetectorTests
{
    private readonly EnclosureDetector _enclosureDetector;

    public EnclosureDetectorTests()
    {
        _enclosureDetector = new EnclosureDetector();
    }

    [Fact]
    public void GetEnclosedFields_OneEnclosedDot_ReturnOneField()
    {
        var state = BoardFactory.Create(
            "H H N",
            "H A H",
            "N H N"
        );

        var resultHuman = _enclosureDetector.GetEnclosedFields(state, Player.Human);
        var resultAI = _enclosureDetector.GetEnclosedFields(state, Player.AI);

        Assert.Contains((1, 1), resultHuman);
        Assert.Single(resultHuman);
        Assert.Empty(resultAI);
    }

    [Fact]
    public void GetEnclosedFields_NotEnclosedDot_ReturnEmptyList()
    {
        var state = BoardFactory.Create(
            "H H N",
            "H A A",
            "N H N"
        );

        var resultHuman = _enclosureDetector.GetEnclosedFields(state, Player.Human);
        var resultAI = _enclosureDetector.GetEnclosedFields(state, Player.AI);

        Assert.Empty(resultHuman);
        Assert.Empty(resultAI);
    }

    [Fact]
    public void GetEnclosedFields_EnclosedMultipleDotsHuman_ReturnFields()
    {
        var state = BoardFactory.Create(
            "H H H N",
            "H A H H",
            "H A A H",
            "N H H N"
        );

        var resultAI = _enclosureDetector.GetEnclosedFields(state, Player.AI);
        var resultHuman = _enclosureDetector.GetEnclosedFields(state, Player.Human);

        Assert.Contains((1, 1), resultHuman);
        Assert.Contains((2, 1), resultHuman);
        Assert.Contains((2, 2), resultHuman);
        Assert.Equal(3, resultHuman.Count);
        Assert.Empty(resultAI);
    }

    [Fact]
    public void GetEnclosedFields_EnclosedMultipleDotsAI_ReturnFields()
    {
        var state = BoardFactory.Create(
            "H A A H",
            "A H H A",
            "A H H A",
            "H A A H"
        );

        var resultAI = _enclosureDetector.GetEnclosedFields(state, Player.AI);
        var resultHuman = _enclosureDetector.GetEnclosedFields(state, Player.Human);

        Assert.Contains((1, 1), resultAI);
        Assert.Contains((1, 2), resultAI);
        Assert.Contains((2, 1), resultAI);
        Assert.Contains((2, 2), resultAI);
        Assert.Equal(4, resultAI.Count);
        Assert.Empty(resultHuman);
    }

    [Fact]
    public void GetEnclosedFields_EmptyBoard_ReturnEmptyList()
    {
        var state = new GameState(4, Player.Human);

        var resultHuman = _enclosureDetector.GetEnclosedFields(state, Player.Human);
        var resultAI = _enclosureDetector.GetEnclosedFields(state, Player.AI);

        Assert.Empty(resultHuman);  
        Assert.Empty(resultAI);  
    }

    [Fact]
    public void GetEnclosedFields_FullBoardWithoutEnclosed_ReturnEmptyList()
    {
        var state = BoardFactory.Create(
            "H A H A",
            "H A H A",
            "H A H A",
            "H A H A"
        );

        var resultHuman = _enclosureDetector.GetEnclosedFields(state, Player.Human);
        var resultAI = _enclosureDetector.GetEnclosedFields(state, Player.AI);

        Assert.Empty(resultAI);
        Assert.Empty(resultHuman);  
    }

    [Fact]
    public void GetEnclosedFields_EnclosedOnBorder_ReturnEmptyList()
    {
        var state = BoardFactory.Create(
            "N N A",
            "N A H",
            "N N A"
        );

        var resultHuman = _enclosureDetector.GetEnclosedFields(state, Player.Human);
        var resultAI = _enclosureDetector.GetEnclosedFields(state, Player.AI);

        Assert.Empty(resultHuman);
        Assert.Empty(resultAI);
    }

    [Fact]
    public void GetEnclosedFields_TerritoryExpansion_ReturnNewFields()
    {
        var state = BoardFactory.Create(
            "A A N N N N N N",
            "A H A A A N N N",
            "A H A H H A N N",
            "N A A A H A N N",
            "N N N N A N N N",
            "N N N N N N N N",
            "N N N N N N N N",
            "N N N N N N N N"
        );

        state.Board[1][1].EnclosedBy = Player.AI;
        state.Board[2][1].EnclosedBy = Player.AI;

        var resultHuman = _enclosureDetector.GetEnclosedFields(state, Player.Human);
        var resultAI = _enclosureDetector.GetEnclosedFields(state, Player.AI);

        Assert.Empty(resultHuman);
        Assert.Equal(3, resultAI.Count);
        Assert.Contains((2,3), resultAI);
        Assert.Contains((2,4), resultAI);
        Assert.Contains((3,4), resultAI);
    }

    [Fact]
    public void GetEnclosedFields_EmptyEnclosed_ReturnEmptyFields()
    {
        var state = BoardFactory.Create(
            "N N N N N N N N",
            "N N A A N N N N",
            "N N A N A N N N",
            "N A N N N A N N",
            "N N A A N N A N",
            "N N N N A A N N",
            "N N N N N N N N",
            "N N N N N N N N"
        );

        var resultAI = _enclosureDetector.GetEnclosedFields(state, Player.AI);

        Assert.Equal(6, resultAI.Count);
        Assert.Contains((3,3), resultAI);
    }

    [Fact]
    public void GetEnclosedFields_EmptyAndOppononentFieldsEnclosed_ReturnBothFieldTypes()
    {
        var state = BoardFactory.Create(
            "N N N N N N N N",
            "N N A A N N N N",
            "N N A N A N N N",
            "N A N H N A N N",
            "N N A A H N A N",
            "N N N N A A N N",
            "N N N N N N N N",
            "N N N N N N N N"
        );

        var resultAI = _enclosureDetector.GetEnclosedFields(state, Player.AI);

        Assert.Equal(6, resultAI.Count);
        Assert.Contains((3,3), resultAI);
        Assert.Contains((3,4), resultAI);
    }

    [Fact]
    public void GetEnclosedFields_AlmostEnclosed_ReturnEmptyList()
    {
        var state = BoardFactory.Create(
            "A A A A A A A A N A",
            "A N N N N N N N N A",
            "A N N N N N N N N A",
            "A N N N H N N N N A",
            "A N N N N H N N N A",
            "A N H H N N H N N A",
            "A N N H H H H N N A",
            "A N N N N N N N N A",
            "A N N N N N N N N A",
            "A A A A A A A A A A"
        );

        var resultAI = _enclosureDetector.GetEnclosedFields(state, Player.AI);

        Assert.Empty(resultAI);
    }

    [Fact]
    public void GetEnclosedFields_MultipleLoops_ReturnCapturedFields()
    {
        var state = BoardFactory.Create(
            "N N N N N N N N N N",
            "N N N H H N N N N N",
            "N N H N N H N N N N",
            "N N H A H H N N N N",
            "N N N H A N H N N N",
            "N N N N H A H N N N",
            "N N N N H N H N N N",
            "N N N N H H N N H N",
            "N N N N N N N H A H",
            "N N N N N N N N H N"
        );

        var resultHuman = _enclosureDetector.GetEnclosedFields(state, Player.Human);

        Assert.Equal(8, resultHuman.Count);
        Assert.Contains((8,8), resultHuman);
        Assert.Contains((3,3), resultHuman);
        Assert.Contains((4,4), resultHuman);
        Assert.Contains((5,5), resultHuman);
        Assert.Contains((2,3), resultHuman);
    }

    [Fact]
    public void GetEnclosedFields_NestedLoops_ReturnCapturedFields()
    {
        var state = BoardFactory.Create(
            "N N N N N N N N N N",
            "N N N N N N N N N N",
            "N N N N N N N N N N",
            "N N H H H H H H N N",
            "N N H N A N N H N N",
            "N N H A H A N H N N",
            "N N H N A N N H N N",
            "N N H H H H H H N N",
            "N N N N N N N N N N",
            "N N N N N N N N N N"
        );

        var resultHuman = _enclosureDetector.GetEnclosedFields(state, Player.Human);
        var resultAI = _enclosureDetector.GetEnclosedFields(state, Player.AI);

        Assert.Single(resultAI);
        Assert.Equal(11, resultHuman.Count);
        Assert.Contains((5,4), resultAI);
        Assert.Contains((5,5), resultHuman);
        Assert.Contains((5,3), resultHuman);
        Assert.Contains((4,3), resultHuman);
    }
}