using System;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;

namespace Bastion.Client.Rendering;

// The mouse is the only control the design gives the camera (D4): drag, wheel
// and a double click that returns to the starting angle.
public sealed class OrbitCamera
{
    private const float StartingYawDegrees = 45.0f;
    private const float StartingPitchDegrees = 38.0f;
    private const float MinimumPitchDegrees = 12.0f;
    private const float MaximumPitchDegrees = 80.0f;
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

    private readonly Vector3 _target;
    private readonly Vector3 _up;
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

    // The board sizes are odd, so the middle cell is the centre to look at.
    public OrbitCamera(BoardFrame frame, int boardSize)
    {
        ArgumentNullException.ThrowIfNull(frame);

        int middle = boardSize / 2;
        float span = boardSize * frame.CellSize;

        _target = frame.GetCellPosition(new BoardSlot(middle, middle));
        _up = frame.Up;
        _minimumDistance = span * ClosestSpans;
        _maximumDistance = span * FarthestSpans;
        _restingDistance = span * RestingSpans;
        _nearPlane = span * NearSpans;
        _farPlane = span * FarSpans;
        _distance = _restingDistance;
    }

    public Matrix View => Matrix.CreateLookAt(GetPosition(), _target, _up);

    public Matrix GetProjection(float aspectRatio)
    {
        return Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(FieldOfViewDegrees),
            aspectRatio,
            _nearPlane,
            _farPlane);
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

        Turn(input);
        Zoom(input);
        _lastMousePosition = input.MousePosition;
        _isPreviousButtonPressed = input.IsButtonPressed;
    }

    // Only while the button was already down: otherwise the first click jumps the
    // whole width of the screen.
    private void Turn(InputState input)
    {
        if (!input.IsButtonPressed || !_isPreviousButtonPressed)
        {
            ReadDoubleClick(input);
            return;
        }

        Point movement = input.MousePosition - _lastMousePosition;
        _yawDegrees -= movement.X * DegreesPerPixel;
        _pitchDegrees = MathHelper.Clamp(
            _pitchDegrees + (movement.Y * DegreesPerPixel),
            MinimumPitchDegrees,
            MaximumPitchDegrees);
    }

    private void ReadDoubleClick(InputState input)
    {
        if (!input.HasClicked)
        {
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

    private void Zoom(InputState input)
    {
        if (input.ScrollNotches == 0)
        {
            return;
        }

        float step = input.ScrollNotches * CellsPerWheelNotch;
        _distance = MathHelper.Clamp(_distance - step, _minimumDistance, _maximumDistance);
    }

    private Vector3 GetPosition()
    {
        float yaw = MathHelper.ToRadians(_yawDegrees);
        float pitch = MathHelper.ToRadians(_pitchDegrees);
        var rotation = Matrix.CreateFromYawPitchRoll(yaw, -pitch, 0.0f);
        var offset = Vector3.Transform(Vector3.Backward * _distance, rotation);

        return _target + offset;
    }
}
