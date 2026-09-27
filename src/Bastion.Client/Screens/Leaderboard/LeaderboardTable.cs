using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Bastion.Client.Controls;

namespace Bastion.Client.Screens.Leaderboard;

// The ranking as a table with translated column headers, the rows of the current page and the player's own row
// pinned below them (CU-35 main flow step 4).
public sealed class LeaderboardTable : Control
{
    public const int HeaderHeight = 28;
    public const int RowHeight = 28;
    public const int OwnRowGap = 10;

    private const int CellPadding = 10;
    private const int ColumnCount = 5;
    private const int StripeEvery = 2;

    // Left edge of each column as a fraction of the table width: position, player, elo, matches, wins.
    private static readonly float[] _columnStarts = [0f, 0.1f, 0.55f, 0.7f, 0.85f];

    public IReadOnlyList<string> Headers { get; set; } = [];

    public IReadOnlyList<LeaderboardRowView> Rows { get; set; } = [];

    public LeaderboardRowView? OwnRow { get; set; }

    public string OwnRowNote { get; set; } = string.Empty;

    public string StatusText { get; set; } = string.Empty;

    public static int ComputeHeight(int visibleRows)
    {
        return HeaderHeight + (visibleRows * RowHeight) + OwnRowGap + RowHeight;
    }

    public override void Draw(Canvas canvas)
    {
        ArgumentNullException.ThrowIfNull(canvas);

        if (!IsVisible)
        {
            return;
        }

        DrawCells(canvas, Headers, new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, HeaderHeight), true);
        if (StatusText.Length > 0)
        {
            DrawStatus(canvas);
        }
        else
        {
            DrawRows(canvas);
        }

        DrawOwnRow(canvas);
    }

    private void DrawRows(Canvas canvas)
    {
        for (int rowIndex = 0; rowIndex < Rows.Count; rowIndex++)
        {
            Rectangle area = GetRowArea(rowIndex);
            if (rowIndex % StripeEvery == 0)
            {
                canvas.Shapes.DrawRectangle(area, Theme.Field);
            }

            DrawCells(canvas, ToCells(Rows[rowIndex]), area, false);
        }
    }

    private void DrawStatus(Canvas canvas)
    {
        Rectangle area = GetRowArea(0);
        TextStyle style = TextStyleFactory.CreateBody(canvas.Fonts, Theme.TextMuted);
        canvas.Text.Draw(StatusText, new Vector2(area.X + CellPadding, area.Y), style);
    }

    private void DrawOwnRow(Canvas canvas)
    {
        var area = new Rectangle(Bounds.X, Bounds.Bottom - RowHeight, Bounds.Width, RowHeight);
        canvas.Shapes.DrawRectangle(area, Theme.FieldFocused);
        if (OwnRow is not null)
        {
            DrawCells(canvas, ToCells(OwnRow), area, true);
            return;
        }

        TextStyle style = TextStyleFactory.CreateBody(canvas.Fonts, Theme.TextDark);
        canvas.Text.Draw(OwnRowNote, new Vector2(area.X + CellPadding, area.Y), style);
    }

    private void DrawCells(Canvas canvas, IReadOnlyList<string> cells, Rectangle area, bool isBold)
    {
        TextStyle style = isBold
            ? TextStyleFactory.CreateBoldBody(canvas.Fonts, Theme.TextDark)
            : TextStyleFactory.CreateBody(canvas.Fonts, Theme.TextDark);
        for (int columnIndex = 0; columnIndex < cells.Count && columnIndex < ColumnCount; columnIndex++)
        {
            float left = area.X + (_columnStarts[columnIndex] * area.Width) + CellPadding;
            canvas.Text.Draw(cells[columnIndex], new Vector2(left, area.Y), style);
        }
    }

    private Rectangle GetRowArea(int rowIndex)
    {
        int top = Bounds.Y + HeaderHeight + (rowIndex * RowHeight);
        return new Rectangle(Bounds.X, top, Bounds.Width, RowHeight);
    }

    private static IReadOnlyList<string> ToCells(LeaderboardRowView row)
    {
        return [row.Position, row.Nickname, row.Elo, row.Matches, row.Wins];
    }
}
