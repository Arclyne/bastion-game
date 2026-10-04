using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Bastion.Client.Rendering;

public sealed class BoardAssetSet
{
    // Cells per side of the board the classic and four player modes use.
    public const int ClassicBoardSize = 9;

    private const string BoardAssetName = "Models/ClassicBoard9x9";
    private const string PawnAssetName = "Models/ClassicPawn";
    private const string WallAssetName = "Models/WoodWall";

    // The anchor is named after the cell at its lower left corner, the same way
    // the server stores a wall.
    private const string AnchorPrefix = "Anchor_";
    private const char FirstColumnLetter = 'a';
    private const int FirstRowNumber = 1;

    // The asset stores base colours in linear space and the effect writes to an
    // sRGB buffer, so the terracotta would arrive near black.
    private const float GammaExponent = 1.0f / 2.2f;

    private BoardAssetSet(Model board, Model pawn, Model wall)
    {
        ConvertMaterialsToGamma(board);
        ConvertMaterialsToGamma(pawn);
        ConvertMaterialsToGamma(wall);
        Board = board;
        Pawn = pawn;
        Wall = wall;
        Frame = MeasureFrame(board);
    }

    public Model Board { get; }

    public Model Pawn { get; }

    public Model Wall { get; }

    public BoardFrame Frame { get; }

    public static BoardAssetSet Load(ContentManager content)
    {
        ArgumentNullException.ThrowIfNull(content);

        return new BoardAssetSet(
            content.Load<Model>(BoardAssetName),
            content.Load<Model>(PawnAssetName),
            content.Load<Model>(WallAssetName));
    }

    // Every tile of the board shares one material, and therefore one effect
    // instance: converting per mesh would raise the same colour 81 times and
    // wash it out to white.
    private static void ConvertMaterialsToGamma(Model model)
    {
        var convertedEffects = new HashSet<BasicEffect>();

        foreach (ModelMesh mesh in model.Meshes)
        {
            foreach (Effect pass in mesh.Effects)
            {
                if (pass is BasicEffect effect && convertedEffects.Add(effect))
                {
                    effect.DiffuseColor = ConvertColourToGamma(effect.DiffuseColor);
                }
            }
        }
    }

    private static Vector3 ConvertColourToGamma(Vector3 linear)
    {
        return new Vector3(
            MathF.Pow(linear.X, GammaExponent),
            MathF.Pow(linear.Y, GammaExponent),
            MathF.Pow(linear.Z, GammaExponent));
    }

    public static string GetAnchorName(BoardSlot groove)
    {
        char column = (char)(FirstColumnLetter + groove.Column);
        return string.Concat(AnchorPrefix, column, (groove.Row + FirstRowNumber).ToString());
    }

    private static BoardFrame MeasureFrame(Model board)
    {
        Vector3 origin = GetAnchorPosition(board, new BoardSlot(0, 0));
        Vector3 nextColumn = GetAnchorPosition(board, new BoardSlot(1, 0));
        Vector3 nextRow = GetAnchorPosition(board, new BoardSlot(0, 1));

        return new BoardFrame(origin, nextColumn - origin, nextRow - origin);
    }

    private static Vector3 GetAnchorPosition(Model board, BoardSlot groove)
    {
        string name = GetAnchorName(groove);
        if (!board.Bones.TryGetValue(name, out ModelBone? bone))
        {
            throw new InvalidOperationException($"The board model has no anchor node. Anchor={name}");
        }

        var transforms = new Matrix[board.Bones.Count];
        board.CopyAbsoluteBoneTransformsTo(transforms);

        return transforms[bone.Index].Translation;
    }
}
