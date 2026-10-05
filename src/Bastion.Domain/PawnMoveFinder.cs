using System;
using System.Collections.Generic;

namespace Bastion.Domain;

// Where a pawn may go, as CU-20 RN-02, RN-03 and RN-04 decide it.
public sealed class PawnMoveFinder
{
    private readonly Board _board;

    public PawnMoveFinder(Board board)
    {
        ArgumentNullException.ThrowIfNull(board);

        _board = board;
    }

    public IReadOnlyList<BoardPosition> GetMoves(BoardPosition pawn)
    {
        var moves = new List<BoardPosition>();

        foreach (BoardStep step in BoardStep.StraightSteps)
        {
            AddMovesTowards(moves, pawn, step);
        }

        return moves;
    }

    private void AddMovesTowards(List<BoardPosition> moves, BoardPosition pawn, BoardStep step)
    {
        BoardPosition neighbour = pawn.Step(step);
        if (!CanTravel(pawn, neighbour))
        {
            return;
        }

        if (!_board.HasPawn(neighbour))
        {
            moves.Add(neighbour);
            return;
        }

        AddJumpsOver(moves, neighbour, step);
    }

    // The edge of the board is read like a wall here: CU-20 does not name it,
    // and there is nowhere to land.
    private void AddJumpsOver(List<BoardPosition> moves, BoardPosition rival, BoardStep step)
    {
        BoardPosition behind = rival.Step(step);
        if (CanLand(rival, behind))
        {
            moves.Add(behind);
            return;
        }

        foreach (BoardStep side in GetSides(step))
        {
            BoardPosition diagonal = rival.Step(side);
            if (CanLand(rival, diagonal))
            {
                moves.Add(diagonal);
            }
        }
    }

    // The two steps square to the one being travelled.
    private static BoardStep[] GetSides(BoardStep step)
    {
        return
        [
            new BoardStep(step.Rows, step.Columns),
            new BoardStep(-step.Rows, -step.Columns),
        ];
    }

    private bool CanTravel(BoardPosition from, BoardPosition to)
    {
        return _board.IsInside(to) && !_board.IsWallBetween(from, to);
    }

    // RN-04: a pawn never shares a cell with another one.
    private bool CanLand(BoardPosition from, BoardPosition to)
    {
        return CanTravel(from, to) && !_board.HasPawn(to);
    }
}
