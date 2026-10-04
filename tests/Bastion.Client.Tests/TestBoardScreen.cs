using System.Linq;
using Xunit;
using Bastion.Client.Rendering;
using Bastion.Client.Screens.Board;
using Bastion.Client.Tests.Navigation;

namespace Bastion.Client.Tests;

// The scene is built without a renderer, which is what happens when the models
// cannot be loaded: it must still stand up and hold what it would draw.
public sealed class TestBoardScreen
{
    private const int BoardSize = 9;
    private const int MiddleColumn = BoardSize / 2;
    private const int LastRow = BoardSize - 1;
    private const int CrossingCount = BoardSize - 1;
    private const int PlayerCount = 2;
    private const int OrientationCount = 2;

    private static MatchView CreateView()
    {
        return new BoardScreen(new RecordingNavigator(), null).View;
    }

    [Fact]
    public void View_ANewScene_HasOnePawnForEachPlayer()
    {
        int pawns = CreateView().Pawns.Count;

        Assert.Equal(PlayerCount, pawns);
    }

    [Fact]
    public void View_ANewScene_PutsThePawnsOnOppositeBackRows()
    {
        MatchView view = CreateView();

        int[] rows = view.Pawns.Select(pawn => pawn.Cell.Row).OrderBy(row => row).ToArray();

        Assert.Equal([0, LastRow], rows);
    }

    [Fact]
    public void View_ANewScene_PutsBothPawnsOnTheMiddleColumn()
    {
        MatchView view = CreateView();

        bool isEveryPawnOnTheMiddleColumn = view.Pawns.All(pawn => pawn.Cell.Column == MiddleColumn);

        Assert.True(isEveryPawnOnTheMiddleColumn);
    }

    [Fact]
    public void View_ANewScene_ShowsOneWallOfEachOrientation()
    {
        MatchView view = CreateView();

        int orientations = view.Walls.Select(wall => wall.Orientation).Distinct().Count();

        Assert.Equal(OrientationCount, orientations);
    }

    // A wall covers two cells from its crossing, so a crossing on the last column
    // or row would hang off the board.
    [Fact]
    public void View_ANewScene_KeepsEveryWallOnACrossingOfTheBoard()
    {
        MatchView view = CreateView();

        bool isEveryWallOnACrossing = view.Walls.All(IsOnACrossing);

        Assert.True(isEveryWallOnACrossing);
    }

    private static bool IsOnACrossing(WallMarker wall)
    {
        return wall.Groove.Column >= 0 && wall.Groove.Column < CrossingCount
            && wall.Groove.Row >= 0 && wall.Groove.Row < CrossingCount;
    }
}
