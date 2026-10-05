using System;
using System.Collections.Generic;

namespace Bastion.Domain;

// Nothing is stored between calls, so every answer is given against the walls
// standing on the board at that moment.
public sealed class PathFinder
{
    private readonly Board _board;

    public PathFinder(Board board)
    {
        ArgumentNullException.ThrowIfNull(board);

        _board = board;
    }

    public IReadOnlyList<BoardPosition>? FindPath(Pawn pawn)
    {
        return FindPath(pawn, null);
    }

    // The same question with a wall that is not on the board yet, which is what
    // CU-21 step 13 asks before letting a wall go down.
    public IReadOnlyList<BoardPosition>? FindPath(Pawn pawn, Wall? candidate)
    {
        return new PathSearch(_board, pawn, candidate).Run();
    }
}
