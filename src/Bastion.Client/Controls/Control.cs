using System;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public abstract class Control
{
    private Rectangle _bounds;
    private bool _isVisible = true;

    public Rectangle Bounds
    {
        get { return _bounds; }
        init { _bounds = value; }
    }

    public bool IsEnabled { get; set; } = true;

    public bool IsVisible
    {
        get { return _isVisible; }
        init { _isVisible = value; }
    }

    public bool IsHovered { get; protected set; }

    public bool IsFocused { get; set; }

    public string? Warning { get; set; }

    public bool HasWarning => !string.IsNullOrEmpty(Warning);

    public void Show()
    {
        _isVisible = true;
    }

    public void Hide()
    {
        _isVisible = false;
    }

    public void MoveTo(Rectangle bounds)
    {
        _bounds = bounds;
    }

    public virtual void Update(InputState input)
    {
        ArgumentNullException.ThrowIfNull(input);

        IsHovered = IsEnabled && IsVisible && Bounds.Contains(input.MousePosition);
    }

    public abstract void Draw(Canvas canvas);
}
