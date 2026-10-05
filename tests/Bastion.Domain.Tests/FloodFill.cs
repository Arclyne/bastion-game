using System;
using System.Collections.Generic;
using Bastion.Domain;

namespace Bastion.Domain.Tests;

// A plain flood fill that asks the board about one step at a time and counts the
// cells it crossed. It exists so the path finder can be checked against a wholly
// different way of searching.
//
// What it does not check is the geometry: it asks the same Board.IsWallBetween
// and Board.IsOnSide that the search asks, so a wrong answer there would be
// inherited by both and go unseen. TestBoard pins that geometry instead.
internal sealed class FloodFill
{
    // There is no way through at all, which no real count of cells can be.
    public const int NoWay = -1;

    private readonly Board _board;
    private readonly Wall? _candidate;
    private readonly Dictionary<BoardPosition, int> _distances = [];
    private readonly Queue<BoardPosition> _pendingCells = new Queue<BoardPosition>();

    public FloodFill(Board board, Wall? candidate)
    {
        _board = board;
        _candidate = candidate;
    }

    // How many cells a shortest way covers, counting both ends, or -1 when the
    // goal side cannot be reached at all.
    public int MeasureWayTo(Pawn pawn)
    {
        if (!_board.IsInside(pawn.Cell))
        {
            return NoWay;
        }

        Reach(pawn.Cell, 1);

        while (_pendingCells.Count > 0)
        {
            BoardPosition cell = _pendingCells.Dequeue();
            if (_board.IsOnSide(cell, pawn.Goal))
            {
                return _distances[cell];
            }

            Spread(cell);
        }

        return NoWay;
    }

    private void Spread(BoardPosition cell)
    {
        foreach (BoardStep step in BoardStep.StraightSteps)
        {
            BoardPosition next = cell.Step(step);
            if (IsOpen(cell, next) && !_distances.ContainsKey(next))
            {
                Reach(next, _distances[cell] + 1);
            }
        }
    }

    private bool IsOpen(BoardPosition from, BoardPosition to)
    {
        return _board.IsInside(to)
            && !_board.IsWallBetween(from, to)
            && (_candidate is null || !_candidate.Value.IsBetween(from, to));
    }

    private void Reach(BoardPosition cell, int distance)
    {
        _distances[cell] = distance;
        _pendingCells.Enqueue(cell);
    }
}
