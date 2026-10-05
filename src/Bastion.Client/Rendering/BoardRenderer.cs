using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Bastion.Domain;

namespace Bastion.Client.Rendering;

// Draws straight into the back buffer, before the sprite batch opens, so the
// world keeps its depth buffer and the interface is painted on top of it.
public sealed class BoardRenderer
{
    private const float AmbientLevel = 0.30f;
    private const float KeyLevel = 0.65f;
    private const float FillLevel = 0.20f;
    private const float SpecularLevel = 0.06f;
    private const float OpaqueAlpha = 1.0f;

    // Below this the ray runs along the board instead of meeting it.
    private const float LevelWithTheBoard = 0.0001f;

    // One crossing fewer per side than cells, since a wall needs two to lie on.
    private const int CrossingsPerSide = BoardAssetSet.ClassicBoardSize - 1;

    // The two ends of the ray, as the viewport measures depth.
    private const float NearDepth = 0.0f;
    private const float FarDepth = 1.0f;

    private static readonly Vector3 _keyLightDirection = Vector3.Normalize(new Vector3(-0.35f, -1.0f, -0.45f));
    private static readonly Vector3 _fillLightDirection = Vector3.Normalize(new Vector3(0.6f, -0.25f, 0.5f));

    private readonly GraphicsDevice _device;
    private readonly BoardAssetSet _assets;
    private readonly OrbitCamera _camera;
    private readonly Dictionary<Model, Matrix[]> _boneTransforms = [];

    private Matrix _projection;

    public BoardRenderer(GraphicsDevice device, BoardAssetSet assets, OrbitCamera camera)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(assets);
        ArgumentNullException.ThrowIfNull(camera);

        _device = device;
        _assets = assets;
        _camera = camera;
    }

    public OrbitCamera Camera => _camera;

    // A wall is laid on a crossing of grooves, not on a cell, so that is what the
    // pointer is matched against.
    public BoardPosition? FindCrossing(Point pointer)
    {
        Vector3? point = FindPointOnTheBoard(pointer);
        if (point is null)
        {
            return null;
        }

        BoardPosition crossing = _assets.Frame.GetCrossingAt(point.Value);

        return IsOnTheBoard(crossing, CrossingsPerSide) ? crossing : null;
    }

    // A pawn stands on a cell, so a move is aimed at one of those instead.
    public BoardPosition? FindCell(Point pointer)
    {
        Vector3? point = FindPointOnTheBoard(pointer);
        if (point is null)
        {
            return null;
        }

        BoardPosition cell = _assets.Frame.GetCellAt(point.Value);

        return IsOnTheBoard(cell, BoardAssetSet.ClassicBoardSize) ? cell : null;
    }

    private Vector3? FindPointOnTheBoard(Point pointer)
    {
        Viewport viewport = _device.Viewport;
        Matrix view = _camera.View;
        Matrix projection = _camera.GetProjection(viewport.AspectRatio);
        var screen = new Vector3(pointer.X, pointer.Y, NearDepth);

        Vector3 near = viewport.Unproject(screen, projection, view, Matrix.Identity);
        Vector3 far = viewport.Unproject(screen with { Z = FarDepth }, projection, view, Matrix.Identity);
        var direction = Vector3.Normalize(far - near);
        Vector3 normal = _assets.Frame.Up;
        float slope = Vector3.Dot(direction, normal);

        return MathF.Abs(slope) < LevelWithTheBoard
            ? null
            : MeasureHit(near, direction, slope);
    }

    private Vector3? MeasureHit(Vector3 near, Vector3 direction, float slope)
    {
        Vector3 onTheBoard = _assets.Frame.GetGroovePosition(default);
        float distance = Vector3.Dot(onTheBoard - near, _assets.Frame.Up) / slope;

        return distance < 0.0f ? null : near + (direction * distance);
    }

    private static bool IsOnTheBoard(BoardPosition position, int count)
    {
        return position.Column >= 0 && position.Column < count
            && position.Row >= 0 && position.Row < count;
    }

    public void Draw(MatchView view)
    {
        ArgumentNullException.ThrowIfNull(view);

        PrepareDevice();
        _projection = _camera.GetProjection(_device.Viewport.AspectRatio);

        // The board keeps the colours of its own materials, so it passes no tint.
        DrawModel(_assets.Board, Matrix.Identity, null);
        DrawPawns(view.Pawns);
        DrawWalls(view);
        DrawPreviews(view);
    }

    // Translucent pieces come last and only read the depth buffer: writing to it
    // would let them hide one another and the board behind them.
    private void DrawPreviews(MatchView view)
    {
        if (view.PawnPreviews.Count == 0 && view.WallPreview is null)
        {
            return;
        }

        _device.BlendState = BlendState.AlphaBlend;
        _device.DepthStencilState = DepthStencilState.DepthRead;

        DrawPawns(view.PawnPreviews);

        if (view.WallPreview is WallMarker wall)
        {
            DrawWall(wall);
        }
    }

    private void DrawPawn(PawnMarker pawn)
    {
        DrawModel(_assets.Pawn, Matrix.CreateTranslation(_assets.Frame.GetCellPosition(pawn.Cell)), pawn.Tint);
    }

    private void DrawWall(WallMarker wall)
    {
        DrawModel(_assets.Wall, GetWallWorld(wall), wall.Tint);
    }

    // The sprite batch of the previous frame left the device in 2D mode.
    private void PrepareDevice()
    {
        _device.DepthStencilState = DepthStencilState.Default;
        _device.BlendState = BlendState.Opaque;
        _device.RasterizerState = RasterizerState.CullCounterClockwise;
    }

    private void DrawPawns(IEnumerable<PawnMarker> pawns)
    {
        foreach (PawnMarker pawn in pawns)
        {
            DrawPawn(pawn);
        }
    }

    private void DrawWalls(MatchView view)
    {
        foreach (WallMarker wall in view.Walls)
        {
            DrawWall(wall);
        }
    }

    // The wall is modelled along the column axis, so the vertical one is a quarter
    // turn around the board normal.
    private Matrix GetWallWorld(WallMarker wall)
    {
        var placement = Matrix.CreateTranslation(_assets.Frame.GetGroovePosition(wall.Groove));
        if (wall.Orientation == WallOrientation.Horizontal)
        {
            return placement;
        }

        return Matrix.CreateFromAxisAngle(_assets.Frame.Up, MathHelper.PiOver2) * placement;
    }

    // The pawn and the wall ship white so the player colour can replace it; the
    // board has its own colours and passes no tint.
    private void DrawModel(Model model, Matrix world, Color? tint)
    {
        Matrix[] bones = GetBoneTransforms(model);

        foreach (ModelMesh mesh in model.Meshes)
        {
            ApplyEffects(mesh, bones[mesh.ParentBone.Index] * world, tint);
            mesh.Draw();
        }
    }

    private void ApplyEffects(ModelMesh mesh, Matrix world, Color? tint)
    {
        foreach (Effect pass in mesh.Effects)
        {
            if (pass is not BasicEffect effect)
            {
                continue;
            }

            effect.World = world;
            effect.View = _camera.View;
            effect.Projection = _projection;
            ConfigureLighting(effect);

            // The tint carries how see-through the piece is, which is what tells a
            // preview from a piece already on the board.
            effect.Alpha = tint?.ToVector4().W ?? OpaqueAlpha;

            if (tint.HasValue)
            {
                effect.DiffuseColor = tint.Value.ToVector3();
            }
        }
    }

    // The default lighting of the effect adds three lights that together pass 2.0
    // and burn the board white, so the rig is set out here.
    private static void ConfigureLighting(BasicEffect effect)
    {
        effect.LightingEnabled = true;
        effect.PreferPerPixelLighting = true;
        effect.AmbientLightColor = Vector3.One * AmbientLevel;
        effect.SpecularColor = Vector3.One * SpecularLevel;

        effect.DirectionalLight0.Enabled = true;
        effect.DirectionalLight0.Direction = _keyLightDirection;
        effect.DirectionalLight0.DiffuseColor = Vector3.One * KeyLevel;
        effect.DirectionalLight0.SpecularColor = Vector3.One * SpecularLevel;

        effect.DirectionalLight1.Enabled = true;
        effect.DirectionalLight1.Direction = _fillLightDirection;
        effect.DirectionalLight1.DiffuseColor = Vector3.One * FillLevel;
        effect.DirectionalLight1.SpecularColor = Vector3.Zero;

        effect.DirectionalLight2.Enabled = false;
    }

    // Each mesh sits where its bone puts it: drawing them all at the origin would
    // pile the whole board onto one cell.
    private Matrix[] GetBoneTransforms(Model model)
    {
        if (_boneTransforms.TryGetValue(model, out Matrix[]? cachedTransforms))
        {
            return cachedTransforms;
        }

        var transforms = new Matrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(transforms);
        _boneTransforms[model] = transforms;

        return transforms;
    }
}
