// NeoKolors
// Copyright (c) krystof 2026

using Metriks;

namespace NeoKolors.Tui.Core;

/// <summary>
///     Represents a borderless style for UI components. This style offers no visual border,
///     making the content occupy the entire region provided without extra padding or
///     decoration.
/// </summary>
public struct BorderlessStyle : IBorderStyle {
    public void Draw(Rect2D rect, in ICharCanvas canvas, int z) { }
    public Rect2D GetContentBox(Size2D size) => new(Point2D.Zero, size);
    public Size2D GetBorderBox(Size2D size) => size;
}