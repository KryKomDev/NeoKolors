// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui;

public readonly struct Point : IEquatable<Point> {
    public int X { get; init; }
    public int Y { get; init; }
    
    public Point(int x, int y) {
        X = x;
        Y = y;
    }
    
    public static Point operator +(Point p1, Point p2) => new(p1.X + p2.X, p1.Y + p2.Y);
    public static Point operator -(Point p1, Point p2) => new(p1.X - p2.X, p1.Y - p2.Y);
    
    public override string ToString() => $"[{X},{Y}]";
    
    public static implicit operator (int X, int Y)(Point p) => (p.X, p.Y);
    public static implicit operator Point((int X, int Y) p) => new(p.X, p.Y);
    public static implicit operator Point2D(Point p) => new(p.X, p.Y);
    
    public static Point Zero => new(0, 0);
    public static Point One => new(1, 1);
    public static Point Two => new(2, 2);
    
    public static Point Min(Point p1, Point p2) => new(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
    public static Point Max(Point p1, Point p2) => new(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
    
    public static Size Rect(Point p1, Point p2) => new(p2.X - p1.X + 1, p2.Y - p1.Y + 1);

    public void Deconstruct(out int x, out int y) {
        x = X;
        y = Y;
    }

    public bool Equals(Point other) => X == other.X && Y == other.Y;

    public override bool Equals(object? obj) => obj is Point other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(X, Y);

    public static bool operator ==(Point left, Point right) => left.Equals(right);

    public static bool operator !=(Point left, Point right) => !left.Equals(right);
}