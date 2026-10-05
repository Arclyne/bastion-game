using System;
using System.Collections.Generic;

namespace Bastion.Domain;

// CU-21 RN-05: a wall never leaves a player without a way to its goal side, and
// the check covers every player, not only the one whose way the wall crosses.
//
// The shortest way kept for each player stays the shortest only because a wall
// never shortens a way and is never taken back (RN-07). A way found around a wall
// merely being considered is kept apart: it is a detour until that wall goes down.
public sealed class EnclosureValidator
{
    private readonly Board _board;
    private readonly PathFinder _pathFinder;

    // Kept per player rather than per goal side: two players are free to share a
    // side, and one slot between them would leave each throwing the other's way
    // away and searching every time.
    private readonly Dictionary<int, IReadOnlyList<BoardPosition>> _shortestPaths = [];
    private readonly Dictionary<int, IReadOnlyList<BoardPosition>> _pathsAroundTheCandidate = [];

    private Wall? _candidateInHand;

    public EnclosureValidator(Board board)
    {
        ArgumentNullException.ThrowIfNull(board);

        _board = board;
        _pathFinder = new PathFinder(board);
    }

    // The goal of the player the wall would shut in, which is what names that
    // player (CU-21 FA-11), or null when every player keeps a way through.
    // It answers RN-05 alone: whether the wall fits on its crossing at all is
    // RN-03 and RN-04, and is expected to have been settled first.
    public BoardSide? FindShutInPlayer(Wall candidate)
    {
        ReadTheCandidate(candidate);

        for (int playerIndex = 0; playerIndex < _board.Pawns.Count; playerIndex++)
        {
            if (!IsPathKept(playerIndex, candidate))
            {
                return _board.Pawns[playerIndex].Goal;
            }
        }

        return null;
    }

    // The shortest way that player has to its goal side on the board as it
    // stands, which is what CU-21 RN-09 shows, or null when it has none. Walls
    // that are only being considered never reach it.
    public IReadOnlyList<BoardPosition>? FindShortestPath(int playerIndex)
    {
        return FindAndKeepPath(_shortestPaths, playerIndex, null);
    }

    // A wall let go of joins the board, which turns the ways found around it into
    // the shortest ways there are; a wall carried elsewhere leaves them worth
    // nothing. Noticing either late costs a search, never a wrong answer.
    private void ReadTheCandidate(Wall candidate)
    {
        if (_candidateInHand is Wall held && _board.HasWall(held))
        {
            PromoteThePathsAroundTheCandidate();
        }

        if (_candidateInHand != candidate)
        {
            _pathsAroundTheCandidate.Clear();
        }

        _candidateInHand = candidate;
    }

    private void PromoteThePathsAroundTheCandidate()
    {
        foreach (KeyValuePair<int, IReadOnlyList<BoardPosition>> around in _pathsAroundTheCandidate)
        {
            _shortestPaths[around.Key] = around.Value;
        }

        _pathsAroundTheCandidate.Clear();
        _candidateInHand = null;
    }

    private bool IsPathKept(int playerIndex, Wall candidate)
    {
        IReadOnlyList<BoardPosition>? shortestCells = FindShortestPath(playerIndex);
        if (shortestCells is null)
        {
            return false;
        }

        if (IsOpen(shortestCells, _board.Pawns[playerIndex], candidate))
        {
            return true;
        }

        return FindAndKeepPath(_pathsAroundTheCandidate, playerIndex, candidate) is not null;
    }

    // Both stores are read the same way, so reading one and searching when it
    // cannot answer lives in a single place.
    private IReadOnlyList<BoardPosition>? FindAndKeepPath(
        Dictionary<int, IReadOnlyList<BoardPosition>> keptPaths,
        int playerIndex,
        Wall? candidate)
    {
        Pawn pawn = _board.Pawns[playerIndex];
        if (keptPaths.TryGetValue(playerIndex, out IReadOnlyList<BoardPosition>? knownCells)
            && IsOpen(knownCells, pawn, candidate))
        {
            return knownCells;
        }

        IReadOnlyList<BoardPosition>? foundCells = _pathFinder.FindPath(pawn, candidate);
        if (foundCells is null)
        {
            keptPaths.Remove(playerIndex);

            return null;
        }

        keptPaths[playerIndex] = foundCells;

        return foundCells;
    }

    // Asked of the way as a whole, which is what makes a pawn that has moved on
    // and a wall placed since then fall out on their own.
    private bool IsOpen(IReadOnlyList<BoardPosition> pathCells, Pawn pawn, Wall? candidate)
    {
        if (pathCells[0] != pawn.Cell || !_board.IsOnSide(pathCells[^1], pawn.Goal))
        {
            return false;
        }

        for (int stepIndex = 1; stepIndex < pathCells.Count; stepIndex++)
        {
            if (_board.IsWallBetween(pathCells[stepIndex - 1], pathCells[stepIndex], candidate))
            {
                return false;
            }
        }

        return true;
    }
}
