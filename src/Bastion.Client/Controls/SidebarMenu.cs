using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Bastion.Client.Controls;

public sealed class SidebarMenu : Control
{
    public const int ItemHeight = 48;
    private const int ItemGap = 4;
    private const int TextInset = 18;

    public List<SidebarItem> Items { get; } = [];

    public int SelectedIndex { get; set; }

    public event EventHandler<SelectionChangedEventArgs>? ItemChosen;

    public override void Update(InputState input)
    {
        base.Update(input);

        if (!IsEnabled || !IsVisible || !input.HasClicked)
        {
            return;
        }

        for (int itemIndex = 0; itemIndex < Items.Count; itemIndex++)
        {
            if (Items[itemIndex].IsEnabled
                && GetItemBounds(itemIndex).Contains(input.MousePosition)
                && itemIndex != SelectedIndex)
            {
                SelectedIndex = itemIndex;
                ItemChosen?.Invoke(this, new SelectionChangedEventArgs { SelectedIndex = itemIndex });
                return;
            }
        }
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        for (int itemIndex = 0; itemIndex < Items.Count; itemIndex++)
        {
            Rectangle area = GetItemBounds(itemIndex);
            bool isSelected = itemIndex == SelectedIndex;

            if (isSelected)
            {
                canvas.Shapes.DrawRoundedRectangle(area, Theme.FieldCornerRadius, Theme.TextDark);
            }

            Color color = GetItemColor(Items[itemIndex], isSelected);
            TextStyle style = TextStyleFactory.CreateBody(canvas.Fonts, color);
            float y = area.Y + ((area.Height - canvas.Text.GetLineHeight(style)) / 2f);
            canvas.Text.Draw(Items[itemIndex].Text, new Vector2(area.X + TextInset, MathF.Round(y)), style);
        }
    }

    private static Color GetItemColor(SidebarItem item, bool isSelected)
    {
        if (isSelected)
        {
            return Theme.TextLight;
        }

        return item.IsEnabled ? Theme.Label : Theme.Placeholder;
    }

    private Rectangle GetItemBounds(int index)
    {
        return new Rectangle(Bounds.X, Bounds.Y + (index * (ItemHeight + ItemGap)), Bounds.Width, ItemHeight);
    }
}
