//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui;

/// <summary>
/// a 2D rectangle structure
/// </summary>
public struct Rectangle : IEquatable<Rectangle> {
    
    private int _lowerX;
    private int _lowerY;
    private int _higherX;
    private int _higherY;

    /// <summary>
    /// lower x bound of the rectangle
    /// </summary>
    public int LowerX { 
        get => _lowerX; 
        set {
            if (value > HigherX) {
                (HigherX, _lowerX) = (value, HigherX);
            }
            else {
                _lowerX = value;
            }
        }
    }

    /// <summary>
    /// lower y bound of the rectangle
    /// </summary>
    public int LowerY {
        get => _lowerY;
        set {
            if (value > HigherY) {
                (HigherY, _lowerY) = (value, HigherY);
            }
            else {
                _lowerY = value;
            }
        }
    }

    /// <summary>
    /// higher x bound of the rectangle
    /// </summary>
    public int HigherX {
        get => _higherX;
        set {
            if (value < _lowerX) {
                (_lowerX, _higherX) = (value, _lowerX);
            }
            else {
                _higherX = value;
            }
        }
    }

    /// <summary>
    /// higher y bound of the rectangle
    /// </summary>
    public int HigherY {
        get => _higherY; 
        set {
            if (value < _lowerY) {
                (_lowerY, _higherY) = (value, _lowerY);
            }
            else {
                _higherY = value;
            }
        }
    }
    
    public Size Size => new(Width, Height);
    
    public Point Lower => new(_lowerX, _lowerY);
    public Point Higher => new(_higherX, _higherY);

    public Rectangle(int lowerX, int lowerY, int higherX, int higherY) {
        _lowerX = Math.Min(lowerX, higherX);
        _lowerY = Math.Min(lowerY, higherY);
        _higherX = Math.Max(lowerX, higherX);
        _higherY = Math.Max(lowerY, higherY);
    }

    public Rectangle(Point p, Size s) {
        _lowerX = p.X;
        _lowerY = p.Y;
        _higherX = p.X + s.Width - 1;
        _higherY = p.Y + s.Height - 1;
    }
    
    /// <summary>
    /// returns the width of the rectangle
    /// </summary>
    public int Width => HigherX - LowerX + 1;
    
    /// <summary>
    /// returns the height of the rectangle
    /// </summary>
    public int Height => HigherY - LowerY + 1;

    /// <summary>
    /// determines whether a point is inside the rectangle
    /// </summary>
    /// <param name="x">x coordinate of the point</param>
    /// <param name="y">y coordinate of the point</param>
    public bool Contains(int x, int y) => x >= _lowerX && x <= _higherX && y >= _lowerY && y <= _higherY;
    
    /// <summary>
    /// determines whether this rectangle overlaps with another one
    /// </summary>
    /// <param name="r">the other rectangle</param>
    /// <returns>true if they overlap, false otherwise</returns>
    public bool Overlaps(Rectangle r) =>
        r.LowerX < HigherX && r.HigherX > LowerX && r.LowerY < HigherY && r.HigherY > LowerY;

    public override string ToString() => $"[{LowerX}, {LowerY}]:[{HigherX}, {HigherY}] {Size.ToString()}";
    
    public (Point Location, Size Size) Decompose() => (Lower, Size);
    
    public static implicit operator (Point Location, Size Size)(Rectangle r) => r.Decompose();
    public static implicit operator Rectangle((Point Location, Size Size) s) => new(s.Location, s.Size);
    
    public static Rectangle operator +(Rectangle r, Point p) 
        => new(r.LowerX + p.X, r.LowerY + p.Y, r.HigherX + p.X, r.HigherY + p.Y);
    
    public static Rectangle operator -(Rectangle r, Size s) 
        => new(r._lowerX, r._lowerY, r._higherX - s.Width, r._higherY - s.Height);

    public bool Equals(Rectangle other) {
        return _lowerX == other._lowerX && _lowerY == other._lowerY && _higherX == other._higherX && _higherY == other._higherY;
    }

    public override bool Equals(object? obj) {
        return obj is Rectangle other && Equals(other);
    }

    public override int GetHashCode() {
        return HashCode.Combine(_lowerX, _lowerY, _higherX, _higherY);
    }

    public static bool operator ==(Rectangle left, Rectangle right) {
        return left.Equals(right);
    }

    public static bool operator !=(Rectangle left, Rectangle right) {
        return !left.Equals(right);
    }

    public void Deconstruct(out Point location, out Size size) => (location, size) = (Lower, Size);
    
    public static Rectangle Zero => new(0, 0, 0, 0);
}