namespace Bastion.Domain;

// Counted from the lower left corner, where column 0 is the column "a".
public readonly record struct BoardPosition(int Column, int Row)
{
    // Where the step lands. Both the move finder and the path finder read a
    // neighbour through here, so what counts as one step is written down once.
    public BoardPosition Step(BoardStep step)
    {
        return new BoardPosition(Column + step.Columns, Row + step.Rows);
    }
}
