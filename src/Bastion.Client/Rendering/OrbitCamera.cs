using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;

namespace Bastion.Client.Rendering;

// The mouse is the only control the design gives the camera (D4).
public sealed class OrbitCamera
{
    private const float StartingYawDegrees = 45.0f;
    private const float StartingPitchDegrees = 38.0f;
    // Dragging up and down sweeps half a turn: 0 and 180 leave the camera in the
    // plane of the board, level with it, and 90 is right above the centre.
    private const float MinimumPitchDegrees = 0.0f;
    private const float MaximumPitchDegrees = 180.0f;
    private const float FullTurnDegrees = 360.0f;
    private const float DegreesPerPixel = 0.35f;
    private const float CellsPerWheelNotch = 0.9f;
    private const float DoubleClickSeconds = 0.35f;
    private const float ClosestSpans = 0.9f;
    private const float FarthestSpans = 2.4f;
    private const float RestingSpans = 1.45f;
    private const float FieldOfViewDegrees = 40.0f;

    // The clip planes follow the size of the board instead of fixed units: the
    // exported model may be in metres or in centimetres and both have to work.
    private const float NearSpans = 0.01f;
    private const float FarSpans = 10.0f;

    private readonly float _cellSize;
    private readonly Vector3 _target;
    private readonly Vector3 _normal;
    private readonly Vector3 _columnAxis;
    private readonly Vector3 _rowAxis;
    private readonly float _minimumDistance;
    private readonly float _maximumDistance;
    private readonly float _restingDistance;
    private readonly float _nearPlane;
    private readonly float _farPlane;

    private float _yawDegrees = StartingYawDegrees;
    private float _pitchDegrees = StartingPitchDegrees;
    private float _distance;
    private float _lastClickSeconds = float.NegativeInfinity;
    private Point _lastMousePosition;
    private bool _isPreviousButtonPressed;
    private bool _hasTurnedWhilePressed;

    // The board sizes are odd, so the middle cell is the centre to look at.
    public OrbitCamera(BoardFrame frame, int boardSize)
    {
        ArgumentNullException.ThrowIfNull(frame);

        int middle = boardSize / 2;
        float span = boardSize * frame.CellSize;

        _cellSize = frame.CellSize;

        _target = frame.GetCellPosition(new BoardSlot(middle, middle));
        _normal = frame.Up;
        _columnAxis = frame.ColumnAxis;
        _rowAxis = frame.RowAxis;
        _minimumDistance = span * ClosestSpans;
        _maximumDistance = span * FarthestSpans;
        _restingDistance = span * RestingSpans;
        _nearPlane = span * NearSpans;
        _farPlane = span * FarSpans;
        _distance = _restingDistance;
    }

    public Matrix View => Matrix.CreateLookAt(Position, _target, GetUpDirection());

    public Vector3 Position
    {
        get
        {
            float pitch = MathHelper.ToRadians(_pitchDegrees);
            Vector3 offset = (GetHorizontalDirection() * MathF.Cos(pitch)) + (_normal * MathF.Sin(pitch));

            return _target + (offset * _distance);
        }
    }

    public Matrix GetProjection(float aspectRatio)
    {
        return Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(FieldOfViewDegrees),
            aspectRatio,
            _nearPlane,
            _farPlane);
    }

    // Sideways there is no end to reach, so the angle is wrapped to keep it from
    // growing without bound.
    public void Turn(float yawDegrees, float pitchDegrees)
    {
        _yawDegrees = WrapDegrees(_yawDegrees + yawDegrees);
        _pitchDegrees = MathHelper.Clamp(
            _pitchDegrees + pitchDegrees,
            MinimumPitchDegrees,
            MaximumPitchDegrees);
    }

    // The step is measured in cells, so a model exported in another unit keeps the
    // same feel, and the ends stop the camera from turning the board inside out.
    public void Zoom(int notches)
    {
        float step = notches * CellsPerWheelNotch * _cellSize;
        _distance = MathHelper.Clamp(_distance - step, _minimumDistance, _maximumDistance);
    }

    public void Reset()
    {
        _yawDegrees = StartingYawDegrees;
        _pitchDegrees = StartingPitchDegrees;
        _distance = _restingDistance;
    }

    public void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        TurnWithDrag(input);
        ZoomWithWheel(input);
        _lastMousePosition = input.MousePosition;
        _isPreviousButtonPressed = input.IsButtonPressed;
    }

    // Only while the button was already down: otherwise the first click jumps the
    // whole width of the screen.
    private void TurnWithDrag(InputState input)
    {
        if (!input.IsButtonPressed || !_isPreviousButtonPressed)
        {
            ReadDoubleClick(input);
            return;
        }

        Point movement = input.MousePosition - _lastMousePosition;
        if (movement != Point.Zero)
        {
            _hasTurnedWhilePressed = true;
        }

        Turn(-movement.X * DegreesPerPixel, movement.Y * DegreesPerPixel);
    }

    private void ReadDoubleClick(InputState input)
    {
        if (!input.HasClicked)
        {
            return;
        }

        // The release that ends a drag is not a click: the player was turning the
        // board, and reading it as one would snap the view back mid gesture.
        if (_hasTurnedWhilePressed)
        {
            _hasTurnedWhilePressed = false;
            return;
        }

        // ElapsedSeconds carries the total running time, so it works as the
        // timestamp of the previous click.
        if (input.ElapsedSeconds - _lastClickSeconds <= DoubleClickSeconds)
        {
            Reset();
        }

        _lastClickSeconds = input.ElapsedSeconds;
    }

    private void ZoomWithWheel(InputState input)
    {
        if (input.ScrollNotches == 0)
        {
            return;
        }

        Zoom(input.ScrollNotches);
    }

    private Vector3 GetHorizontalDirection()
    {
        float yaw = MathHelper.ToRadians(_yawDegrees);

        return (_columnAxis * MathF.Cos(yaw)) + (_rowAxis * MathF.Sin(yaw));
    }

    // Perpendicular to the line of sight at every angle of the arc, which is what
    // keeps the view from degenerating when the camera reaches the top.
    private Vector3 GetUpDirection()
    {
        float pitch = MathHelper.ToRadians(_pitchDegrees);

        return (_normal * MathF.Cos(pitch)) - (GetHorizontalDirection() * MathF.Sin(pitch));
    }

    private static float WrapDegrees(float degrees)
    {
        float wrapped = degrees % FullTurnDegrees;

        return wrapped < 0.0f ? wrapped + FullTurnDegrees : wrapped;
    }
}
