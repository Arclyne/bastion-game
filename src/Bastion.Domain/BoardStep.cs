using System.Collections.Generic;

namespace Bastion.Domain;

public readonly record struct BoardStep(int Columns, int Rows)
{
    // The four ways a pawn travels (CU-20 RN-02), which are also the four ways a
    // path runs. Both the move finder and the path finder read them from here, so
    // there is one definition of what counts as a neighbour.
    public static IReadOnlyList<BoardStep> StraightSteps { get; } =
    [
        new BoardStep(0, 1),
        new BoardStep(1, 0),
        new BoardStep(0, -1),
        new BoardStep(-1, 0),
    ];
}
