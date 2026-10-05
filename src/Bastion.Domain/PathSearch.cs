using System;
using System.Collections.Generic;

namespace Bastion.Domain;

// One A* run over the cells of the board, then thrown away. The walls are read
// into a grid of openings first so the search never looks at a wall again, and
// the heuristic never overstates the distance left, which is what makes the way
// it returns the shortest one that CU-21 RN-09 shows the player.
internal sealed class PathSearch
{
    private const int NotReached = int.MaxValue;

    private readonly Board _board;
    private readonly Pawn _pawn;
    private readonly Wall? _candidate;
    private readonly bool[,] _openAlongRows;
    private readonly bool[,] _openAlongColumns;
    private readonly int[,] _costs;
    private readonly BoardPosition?[,] _previousCells;
    private readonly PriorityQueue<BoardPosition, int> _pendingCells = new PriorityQueue<BoardPosition, int>();

    // The candidate is a wall that is only being considered, so it is not on the
    // board and has to be read on top of it.
    public PathSearch(Board board, Pawn pawn, Wall? candidate)
    {
        _board = board;
        _pawn = pawn;
        _candidate = candidate;
        _openAlongRows = new bool[board.Size, board.Size];
        _openAlongColumns = new bool[board.Size, board.Size];
        _costs = new int[board.Size, board.Size];
        _previousCells = new BoardPosition?[board.Size, board.Size];

        PrepareTheGrid();
    }

    public IReadOnlyList<BoardPosition>? Run()
    {
        if (!_board.IsInside(_pawn.Cell))
        {
            return null;
        }

        Reach(_pawn.Cell, 0);

        while (_pendingCells.TryDequeue(out BoardPosition cell, out _))
        {
            if (_board.IsOnSide(cell, _pawn.Goal))
            {
                return BuildPathTo(cell);
            }

            Expand(cell);
        }

        return null;
    }

    // The grid holds both halves of what the search walks: which steps are open,
    // and how dearly each cell has been reached so far.
    private void PrepareTheGrid()
    {
        for (int column = 0; column < _board.Size; column++)
        {
            for (int row = 0; row < _board.Size; row++)
            {
                PrepareTheGridAt(new BoardPosition(column, row));
            }
        }
    }

    // Only the two steps that run away from the cell are read. The other two
    // belong to the neighbours and are read when their turn comes, so every
    // opening is written down exactly once.
    private void PrepareTheGridAt(BoardPosition cell)
    {
        BoardPosition right = cell.Step(new BoardStep(1, 0));
        BoardPosition above = cell.Step(new BoardStep(0, 1));

        _costs[cell.Column, cell.Row] = NotReached;
        _openAlongRows[cell.Column, cell.Row] = IsStepOpen(cell, right);
        _openAlongColumns[cell.Column, cell.Row] = IsStepOpen(cell, above);
    }

    private bool IsStepOpen(BoardPosition from, BoardPosition to)
    {
        return _board.IsInside(to) && !_board.IsWallBetween(from, to, _candidate);
    }

    private bool IsOpenBetween(BoardPosition from, BoardPosition to)
    {
        if (from.Row == to.Row)
        {
            return _openAlongRows[Math.Min(from.Column, to.Column), from.Row];
        }

        return _openAlongColumns[from.Column, Math.Min(from.Row, to.Row)];
    }

    private void Expand(BoardPosition cell)
    {
        foreach (BoardStep step in BoardStep.StraightSteps)
        {
            Advance(cell, step);
        }
    }

    private void Advance(BoardPosition cell, BoardStep step)
    {
        BoardPosition next = cell.Step(step);
        if (!_board.IsInside(next) || !IsOpenBetween(cell, next))
        {
            return;
        }

        int cost = _costs[cell.Column, cell.Row] + 1;
        if (cost >= _costs[next.Column, next.Row])
        {
            return;
        }

        _previousCells[next.Column, next.Row] = cell;
        Reach(next, cost);
    }

    // Ordered by the steps already taken plus the steps still left at best, which
    // is what keeps the search heading for the goal side.
    private void Reach(BoardPosition cell, int cost)
    {
        _costs[cell.Column, cell.Row] = cost;
        _pendingCells.Enqueue(cell, cost + _board.GetStepsToSide(cell, _pawn.Goal));
    }

    private IReadOnlyList<BoardPosition> BuildPathTo(BoardPosition goal)
    {
        var pathCells = new List<BoardPosition>();
        BoardPosition? cell = goal;

        while (cell is not null)
        {
            pathCells.Add(cell.Value);
            cell = _previousCells[cell.Value.Column, cell.Value.Row];
        }

        pathCells.Reverse();

        return pathCells;
    }
}
