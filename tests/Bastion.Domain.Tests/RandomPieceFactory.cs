using System;
using Bastion.Domain;

namespace Bastion.Domain.Tests;

// Pieces thrown about at random, for the tests that check an answer against
// another way of working it out. The seed is held here so a run that fails can
// be repeated, and the ranges come from the board itself rather than from
// numbers written out again in each test.
internal sealed class RandomPieceFactory
{
    private const int OrientationCount = 2;
    private const int SideCount = 4;
    private const int Heads = 0;

    private readonly Random _random;
    private readonly int _boardSize;

    public RandomPieceFactory(int seed, int boardSize)
    {
        _random = new Random(seed);
        _boardSize = boardSize;
    }

    public int Pick(int count)
    {
        return _random.Next(count);
    }

    public Wall CreateWall()
    {
        int crossings = _boardSize - Wall.Length + 1;
        var crossing = new BoardPosition(_random.Next(crossings), _random.Next(crossings));

        return new Wall(crossing, CreateOrientation());
    }

    // Half the answers are asked without a wall being considered at all, so both
    // ways of asking the path finder are covered.
    public Wall? CreateCandidate()
    {
        if (_random.Next(OrientationCount) == Heads)
        {
            return null;
        }

        return CreateWall();
    }

    public Pawn CreatePawn()
    {
        var cell = new BoardPosition(_random.Next(_boardSize), _random.Next(_boardSize));

        return new Pawn(cell, (BoardSide)_random.Next(SideCount));
    }

    private WallOrientation CreateOrientation()
    {
        if (_random.Next(OrientationCount) == Heads)
        {
            return WallOrientation.Horizontal;
        }

        return WallOrientation.Vertical;
    }
}
