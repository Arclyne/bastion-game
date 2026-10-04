using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bastion.Client.Rendering;

// Draws straight into the back buffer, before the sprite batch opens, so the
// world keeps its depth buffer and the interface is painted on top of it.
public sealed class BoardRenderer
{
    private const float AmbientLevel = 0.30f;
    private const float KeyLevel = 0.65f;
    private const float FillLevel = 0.20f;
    private const float SpecularLevel = 0.06f;

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

    public void Draw(MatchView view)
    {
        ArgumentNullException.ThrowIfNull(view);

        PrepareDevice();
        _projection = _camera.GetProjection(_device.Viewport.AspectRatio);

        // The board keeps the colours of its own materials, so it passes no tint.
        DrawModel(_assets.Board, Matrix.Identity, null);
        DrawPawns(view);
        DrawWalls(view);
    }

    // The sprite batch of the previous frame left the device in 2D mode.
    private void PrepareDevice()
    {
        _device.DepthStencilState = DepthStencilState.Default;
        _device.BlendState = BlendState.Opaque;
        _device.RasterizerState = RasterizerState.CullCounterClockwise;
    }

    private void DrawPawns(MatchView view)
    {
        foreach (PawnMarker pawn in view.Pawns)
        {
            var world = Matrix.CreateTranslation(_assets.Frame.GetCellPosition(pawn.Cell));
            DrawModel(_assets.Pawn, world, pawn.Tint);
        }
    }

    private void DrawWalls(MatchView view)
    {
        foreach (WallMarker wall in view.Walls)
        {
            DrawModel(_assets.Wall, GetWallWorld(wall), wall.Tint);
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
