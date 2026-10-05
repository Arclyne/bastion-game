using System;
using Microsoft.Xna.Framework;
using Bastion.Domain;

namespace Bastion.Client.Rendering;

// Both steps are measured from the anchor nodes of the asset, so an exporter
// that flips an axis or changes the unit cannot break this.
public sealed class BoardFrame
{
    private const float HalfCell = 0.5f;

    private readonly Vector3 _firstGroove;
    private readonly Vector3 _columnStep;
    private readonly Vector3 _rowStep;

    public BoardFrame(Vector3 firstGroove, Vector3 columnStep, Vector3 rowStep)
    {
        _firstGroove = firstGroove;
        _columnStep = columnStep;
        _rowStep = rowStep;
    }

    public float CellSize => _columnStep.Length();

    // Normal of the board plane: the way up, and the axis a wall turns around.
    public Vector3 Up => Vector3.Normalize(Vector3.Cross(_columnStep, _rowStep));

    // The two directions the board runs in. Together with Up they are the basis
    // the camera orbits in, so it never has to assume which axis is which.
    public Vector3 ColumnAxis => Vector3.Normalize(_columnStep);

    public Vector3 RowAxis => Vector3.Normalize(_rowStep);

    // A wall sits exactly on a groove crossing, which is what the anchors mark.
    public Vector3 GetGroovePosition(BoardPosition groove)
    {
        return _firstGroove + (_columnStep * groove.Column) + (_rowStep * groove.Row);
    }

    // The steps are not unit vectors, so each coordinate is the point projected
    // onto its own step.
    public BoardPosition GetCrossingAt(Vector3 point)
    {
        Vector3 offset = point - _firstGroove;
        float column = Vector3.Dot(offset, _columnStep) / _columnStep.LengthSquared();
        float row = Vector3.Dot(offset, _rowStep) / _rowStep.LengthSquared();

        return new BoardPosition((int)MathF.Round(column), (int)MathF.Round(row));
    }

    // Extrapolating also covers the last column and row, which have no crossing.
    public Vector3 GetCellPosition(BoardPosition cell)
    {
        return GetGroovePosition(cell) - ((_columnStep + _rowStep) * HalfCell);
    }
}
