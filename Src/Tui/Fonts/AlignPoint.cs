// NeoKolors
// Copyright (c) KryKom 2026

using Metriks;

namespace NeoKolors.Tui.Fonts;

public record struct AlignPoint {

    public char    Character { get; }
    public Point2D Position  { get; set; }
    
    internal void SetPosition(Point2D point) => Position = point;

    public AlignPoint(char character, Point2D position) {
        Character = character;
        Position  = position;
    }

    public override string ToString() => $"{Character.ToString()}{Position.ToString()}";
}