// NeoKolors
// Copyright (c) 2025 KryKom

#define SYSTEM_PRIVATE_CORELIB

using System.Diagnostics;
using System.Diagnostics.Contracts;
using Metriks;

namespace NeoKolors.Tui;

[DebuggerStepThrough]
public readonly struct Size : IEquatable<Size> {
    public int Width { get; init; }
    public int Height { get; init; }
    
    public Size(int width, int height) {
        Width = width < 0 ? 0 : width;
        Height = height < 0 ? 0 : height;
    }

    public override string ToString() {
        return "({0}, {1})".Format(Width, Height);
    }

    public bool Equals(Size other) => Width == other.Width && Height == other.Height;
    public override bool Equals(object? obj) => obj is Size other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Width, Height);
    
    [DebuggerStepThrough]
    public static bool operator ==(Size left, Size right) => left.Equals(right);
    
    [DebuggerStepThrough]
    public static bool operator !=(Size left, Size right) => !left.Equals(right);
    
    [DebuggerStepThrough]
    public static Size operator +(Size left, Size right) => new(left.Width + right.Width, left.Height + right.Height);
    
    [DebuggerStepThrough]
    public static Rectangle operator +(Size size, Point p) => new(p, size);
    
    [DebuggerStepThrough]
    public static implicit operator Size(Size2D s) => new(s.X, s.Y);
    
    [DebuggerStepThrough]
    public static implicit operator Size2D(Size s) => new(s.Width, s.Height);
    
    [DebuggerStepThrough]
    public static implicit operator Size(Rectangle r) => r.Size;

    [DebuggerStepThrough]
    public static implicit operator Rectangle(Size r) => new(new Point(0, 0), new Size(r.Width, r.Height));
    
    [Pure]
    [JBPure]
    public Size Clamp(Size min, Size max) => 
        new(Math.Clamp(Width, min.Width, max.Width), Math.Clamp(Height, min.Height, max.Height));
    
    [Pure]
    public static Size Max(Size left, Size right) 
        => new(Math.Max(left.Width, right.Width), Math.Max(left.Height, right.Height));
    
    [Pure]
    public static Size Min(Size left, Size right) 
        => new(Math.Min(left.Width, right.Width), Math.Min(left.Height, right.Height));
    
    public static Size Zero => new(0, 0);
    public static Size One => new(1, 1);
    public static Size Two => new(2, 2);
}