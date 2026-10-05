using Microsoft.Xna.Framework;
using Xunit;
using Bastion.Client.Rendering;

namespace Bastion.Client.Tests;

// The frame mimics the board asset: one unit per cell, columns along X, rows
// towards negative Z, which leaves the board on the XZ plane with Y as its
// normal. The centre of a nine cell board then sits at the origin.
public sealed class TestOrbitCamera
{
    private const int BoardSize = 9;
    private const float LongDrag = 1000.0f;
    private const float FullTurn = 360.0f;
    private const float QuarterTurn = FullTurn / 4.0f;
    private const float SomeYaw = 73.0f;
    private const float SomePitch = 51.0f;
    private const int OneNotch = 1;
    private const int CoarsePrecision = 2;
    private const int Precision = 4;
    private const int ManyNotches = 200;
    private const float ClosestSpans = 0.9f;
    private const float FarthestSpans = 2.4f;
    private const float CellsPerNotch = 0.9f;
    private const float CentimetresPerCell = 100.0f;
    private const float FirstGrooveOffset = 3.5f;

    private static OrbitCamera CreateCamera()
    {
        var frame = new BoardFrame(new Vector3(-3.5f, 0.0f, 3.5f), Vector3.UnitX, -Vector3.UnitZ);

        return new OrbitCamera(frame, BoardSize);
    }

    private static float GetDistanceToTheCentre(OrbitCamera camera)
    {
        return camera.Position.Length();
    }

    private static float GetHeightOverTheBoard(OrbitCamera camera)
    {
        return Vector3.Dot(camera.Position, Vector3.Up);
    }

    [Fact]
    public void Turn_DraggedFarUpwards_StopsLevelWithTheBoard()
    {
        OrbitCamera camera = CreateCamera();

        camera.Turn(0.0f, LongDrag);

        Assert.Equal(0.0, GetHeightOverTheBoard(camera), Precision);
    }

    [Fact]
    public void Turn_DraggedFarDownwards_StopsLevelWithTheBoard()
    {
        OrbitCamera camera = CreateCamera();

        camera.Turn(0.0f, -LongDrag);

        Assert.Equal(0.0, GetHeightOverTheBoard(camera), Precision);
    }

    // The two ends of the arc are half a turn apart, so the camera ends up on
    // opposite sides of the board.
    [Fact]
    public void Turn_FromOneEndOfTheArcToTheOther_CrossesToTheOppositeSide()
    {
        OrbitCamera camera = CreateCamera();
        camera.Turn(0.0f, -LongDrag);
        Vector3 start = camera.Position;

        camera.Turn(0.0f, LongDrag);

        Assert.Equal(0.0, Vector3.Distance(-start, camera.Position), Precision);
    }

    [Fact]
    public void Turn_HalfWayUpTheArc_LooksStraightDownAtTheCentre()
    {
        OrbitCamera camera = CreateCamera();
        camera.Turn(0.0f, -LongDrag);

        camera.Turn(0.0f, QuarterTurn);

        Assert.Equal(0.0, new Vector2(camera.Position.X, camera.Position.Z).Length(), Precision);
    }

    [Fact]
    public void Turn_DraggedSidewaysAFullTurn_ComesBackToTheSamePlace()
    {
        OrbitCamera camera = CreateCamera();
        Vector3 start = camera.Position;

        camera.Turn(FullTurn, 0.0f);

        Assert.Equal(0.0, Vector3.Distance(start, camera.Position), Precision);
    }

    // Sideways there is no end to stop at: the player can keep going round.
    [Fact]
    public void Turn_DraggedSidewaysBeyondAFullTurn_KeepsGoingRound()
    {
        OrbitCamera camera = CreateCamera();
        Vector3 quarter = GetPositionAfterTurning(QuarterTurn);

        camera.Turn(FullTurn + QuarterTurn, 0.0f);

        Assert.Equal(0.0, Vector3.Distance(quarter, camera.Position), Precision);
    }

    // The board is nine cells wide, so the camera never gets closer than 8.1 nor
    // further than 21.6, and rests at 13.05.
    [Fact]
    public void Zoom_WheeledInFarBeyondTheLimit_StopsAtTheClosestDistance()
    {
        OrbitCamera camera = CreateCamera();

        camera.Zoom(ManyNotches);

        Assert.Equal(BoardSize * ClosestSpans, GetDistanceToTheCentre(camera), Precision);
    }

    [Fact]
    public void Zoom_WheeledOutFarBeyondTheLimit_StopsAtTheFarthestDistance()
    {
        OrbitCamera camera = CreateCamera();

        camera.Zoom(-ManyNotches);

        Assert.Equal(BoardSize * FarthestSpans, GetDistanceToTheCentre(camera), Precision);
    }

    [Fact]
    public void Zoom_OneNotchIn_MovesCloserByAFractionOfACell()
    {
        OrbitCamera camera = CreateCamera();
        float start = GetDistanceToTheCentre(camera);

        camera.Zoom(OneNotch);

        Assert.Equal(start - CellsPerNotch, GetDistanceToTheCentre(camera), Precision);
    }

    // The double click of the design goes through Reset, so what it restores is
    // the angle and the distance at once.
    [Fact]
    public void Reset_AfterTurningAndZooming_ComesBackToTheStartingView()
    {
        OrbitCamera camera = CreateCamera();
        Vector3 start = camera.Position;
        camera.Turn(SomeYaw, SomePitch);
        camera.Zoom(ManyNotches);

        camera.Reset();

        Assert.Equal(0.0, Vector3.Distance(start, camera.Position), Precision);
    }

    [Fact]
    public void View_AtAnyAngleOfTheArc_KeepsLookingAtTheCentreOfTheBoard()
    {
        OrbitCamera camera = CreateCamera();
        camera.Turn(SomeYaw, SomePitch);

        var centreOnScreen = Vector3.Transform(Vector3.Zero, camera.View);

        Assert.Equal(0.0, new Vector2(centreOnScreen.X, centreOnScreen.Y).Length(), Precision);
    }

    // One notch has to move the same fraction of a cell whatever unit the model
    // was exported in, so the wheel is never left useless by a change of scale.
    [Fact]
    public void Zoom_OneNotchOnABoardInCentimetres_MovesCloserByTheSameFractionOfACell()
    {
        var frame = new BoardFrame(
            new Vector3(-FirstGrooveOffset, 0.0f, FirstGrooveOffset) * CentimetresPerCell,
            Vector3.UnitX * CentimetresPerCell,
            -Vector3.UnitZ * CentimetresPerCell);
        var camera = new OrbitCamera(frame, BoardSize);
        float start = GetDistanceToTheCentre(camera);

        camera.Zoom(OneNotch);

        Assert.Equal(
            start - (CellsPerNotch * CentimetresPerCell),
            GetDistanceToTheCentre(camera),
            CoarsePrecision);
    }

    private static Vector3 GetPositionAfterTurning(float yawDegrees)
    {
        OrbitCamera camera = CreateCamera();
        camera.Turn(yawDegrees, 0.0f);

        return camera.Position;
    }
}
