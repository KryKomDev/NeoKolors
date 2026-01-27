// NeoKolors
// Copyright (c) 2025 KryKom

using System.Diagnostics;
using System.Diagnostics.Contracts;
using Metriks;

namespace NeoKolors.Tui;

public class SizeF {
    public float Width { get; init; }
    public float Height { get; init; }
    
    public SizeF(float width, float height) {
        Width = width < 0 ? 0 : width;
        Height = height < 0 ? 0 : height;
    }

    public override string ToString() {
        return "({0}, {1})".Format(Width, Height);
    }

    private const float TOLERANCE = 0.000001f;
    
    public bool Equals(SizeF other) => Math.Abs(Width - other.Width) < TOLERANCE && Math.Abs(Height - other.Height) < TOLERANCE;
    public override bool Equals(object? obj) => obj is SizeF other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(Width, Height);
    
    [DebuggerStepThrough]
    public static bool operator ==(SizeF left, SizeF right) => left.Equals(right);
    
    [DebuggerStepThrough]
    public static bool operator !=(SizeF left, SizeF right) => !left.Equals(right);
    
    [DebuggerStepThrough]
    public static SizeF operator +(SizeF left, SizeF right) => new(left.Width + right.Width, left.Height + right.Height);
    
    [DebuggerStepThrough]
    public static Rectangle operator +(SizeF size, Point p) => new(p, new Size((int)size.Width, (int)size.Height));
    
    [DebuggerStepThrough]
    public static implicit operator SizeF(Size2D s) => new(s.X, s.Y);
    
    [DebuggerStepThrough]
    public static implicit operator Size2D(SizeF s) => new((int)s.Width, (int)s.Height);
    
    [DebuggerStepThrough]
    public static implicit operator SizeF(Rectangle r) => new(r.Size.Width, r.Size.Height);

    [DebuggerStepThrough]
    public static implicit operator Rectangle(SizeF r) => new(new Point(0, 0), new Size((int)r.Width, (int)r.Height));
    
    [Pure]
    [JBPure]
    public SizeF Clamp(SizeF min, SizeF max) => 
        new(Math.Clamp(Width, min.Width, max.Width), Math.Clamp(Height, min.Height, max.Height));
    
    [Pure]
    public static SizeF Max(SizeF left, SizeF right) 
        => new(Math.Max(left.Width, right.Width), Math.Max(left.Height, right.Height));
    
    [Pure]
    public static SizeF Min(SizeF left, SizeF right) 
        => new(Math.Min(left.Width, right.Width), Math.Min(left.Height, right.Height));
    
    public static SizeF Zero => new(0, 0);
    public static SizeF One => new(1, 1);
    public static SizeF Two => new(2, 2);
}