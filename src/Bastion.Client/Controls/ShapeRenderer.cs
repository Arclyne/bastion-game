using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Bastion.Client.Controls;

public sealed class ShapeRenderer : IDisposable
{
    private const float LineThickness = 2f;

    private const float PixelCenter = 0.5f;

    private readonly GraphicsDevice _device;
    private readonly SpriteBatch _batch;
    private readonly Dictionary<RoundedRectangleShape, Texture2D> _textures = [];

    public ShapeRenderer(GraphicsDevice device, SpriteBatch batch)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(batch);

        _device = device;
        _batch = batch;

        Pixel = new Texture2D(device, 1, 1);
        Pixel.SetData(new[] { Color.White });
    }

    public Texture2D Pixel { get; }

    public void DrawRectangle(Rectangle bounds, Color color)
    {
        _batch.Draw(Pixel, bounds, color);
    }

    public void DrawLine(Point from, Point to, Color color)
    {
        var delta = new Vector2(to.X - from.X, to.Y - from.Y);
        float length = delta.Length();

        if (length < 1f)
        {
            return;
        }

        float angle = MathF.Atan2(delta.Y, delta.X);
        var scale = new Vector2(length, LineThickness);
        var origin = new Vector2(0f, PixelCenter);

        _batch.Draw(Pixel, new Vector2(from.X, from.Y), null, color, angle, origin, scale, SpriteEffects.None, 0f);
    }

    public void DrawRoundedRectangle(Rectangle bounds, int cornerRadius, Color color)
    {
        var shape = new RoundedRectangleShape
        {
            Width = bounds.Width,
            Height = bounds.Height,
            CornerRadius = cornerRadius
        };

        _batch.Draw(GetTexture(shape), new Vector2(bounds.X, bounds.Y), color);
    }

    public void DrawRoundedBorder(Rectangle bounds, BorderStyle style)
    {
        ArgumentNullException.ThrowIfNull(style);

        var shape = new RoundedRectangleShape
        {
            Width = bounds.Width,
            Height = bounds.Height,
            CornerRadius = style.CornerRadius,
            BorderThickness = style.Thickness
        };

        _batch.Draw(GetTexture(shape), new Vector2(bounds.X, bounds.Y), style.Color);
    }

    public void Dispose()
    {
        foreach (Texture2D texture in _textures.Values)
        {
            texture.Dispose();
        }

        _textures.Clear();
        Pixel.Dispose();
    }

    private Texture2D GetTexture(RoundedRectangleShape shape)
    {
        if (_textures.TryGetValue(shape, out Texture2D? cached))
        {
            return cached;
        }

        var texture = new Texture2D(_device, shape.Width, shape.Height);
        texture.SetData(BuildPixels(shape));
        _textures[shape] = texture;

        return texture;
    }

    private static Color[] BuildPixels(RoundedRectangleShape shape)
    {
        var pixels = new Color[shape.Width * shape.Height];

        for (int y = 0; y < shape.Height; y++)
        {
            for (int x = 0; x < shape.Width; x++)
            {
                float distance = GetSignedDistance(x, y, shape);
                pixels[(y * shape.Width) + x] = Color.White * GetCoverage(distance, shape.BorderThickness);
            }
        }

        return pixels;
    }

    private static float GetSignedDistance(int x, int y, RoundedRectangleShape shape)
    {
        float halfWidth = shape.Width / 2f;
        float halfHeight = shape.Height / 2f;
        float radius = MathHelper.Clamp(shape.CornerRadius, 0, MathF.Min(halfWidth, halfHeight));
        float offsetX = MathF.Abs(x + PixelCenter - halfWidth) - halfWidth + radius;
        float offsetY = MathF.Abs(y + PixelCenter - halfHeight) - halfHeight + radius;
        float clampedX = MathF.Max(offsetX, 0);
        float clampedY = MathF.Max(offsetY, 0);
        float outside = MathF.Sqrt((clampedX * clampedX) + (clampedY * clampedY));

        return outside + MathF.Min(MathF.Max(offsetX, offsetY), 0) - radius;
    }

    private static float GetCoverage(float distance, int borderThickness)
    {
        float coverage = Math.Clamp(PixelCenter - distance, 0f, 1f);

        if (borderThickness <= 0)
        {
            return coverage;
        }

        return coverage - Math.Clamp(PixelCenter - (distance + borderThickness), 0f, 1f);
    }
}
