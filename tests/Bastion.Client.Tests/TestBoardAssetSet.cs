using Xunit;
using Bastion.Client.Rendering;
using Bastion.Domain;

namespace Bastion.Client.Tests;

public sealed class TestBoardAssetSet
{
    [Fact]
    public void GetAnchorName_FirstCrossing_IsTheLowerLeftCell()
    {
        string name = BoardAssetSet.GetAnchorName(new BoardPosition(0, 0));

        Assert.Equal("Anchor_a1", name);
    }

    // The groove is named after the cell at its lower left corner, the same way
    // the server stores it, so the two never have to be translated.
    [Fact]
    public void GetAnchorName_LastCrossingOfTheClassicBoard_IsTheEighthColumnAndRow()
    {
        string name = BoardAssetSet.GetAnchorName(new BoardPosition(7, 7));

        Assert.Equal("Anchor_h8", name);
    }
}
