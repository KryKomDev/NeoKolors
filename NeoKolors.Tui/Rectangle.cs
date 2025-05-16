//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui;

/// <summary>
/// a 2D rectangle structure
/// </summary>
public struct Rectangle {
    
    private int lowerX;
    private int lowerY;
    private int higherX;
    private int higherY;

    /// <summary>
    /// lower x bound of the rectangle
    /// </summary>
    public int LowerX { 
        get => lowerX; 
        set {
            if (value > HigherX) {
                (HigherX, lowerX) = (value, HigherX);
            }
            else {
                lowerX = value;
            }
        }
    }

    /// <summary>
    /// lower y bound of the rectangle
    /// </summary>
    public int LowerY {
        get => lowerY;
        set {
            if (value > HigherY) {
                (HigherY, lowerY) = (value, HigherY);
            }
            else {
                lowerY = value;
            }
        }
    }

    /// <summary>
    /// higher x bound of the rectangle
    /// </summary>
    public int HigherX {
        get => higherX;
        set {
            if (value < lowerX) {
                (lowerX, higherX) = (value, lowerX);
            }
            else {
                higherX = value;
            }
        }
    }

    /// <summary>
    /// higher y bound of the rectangle
    /// </summary>
    public int HigherY {
        get => higherY; 
        set {
            if (value < lowerY) {
                (lowerY, higherY) = (value, lowerY);
            }
            else {
                higherY = value;
            }
        }
    }

    public Rectangle(int lowerX, int lowerY, int higherX, int higherY) {
        this.lowerX = Math.Min(lowerX, higherX);
        this.lowerY = Math.Min(lowerY, higherY);
        this.higherX = Math.Max(lowerX, higherX);
        this.higherY = Math.Max(lowerY, higherY);
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
    public bool Contains(int x, int y) => x >= lowerX && x <= higherX && y >= lowerY && y <= higherY;
    
    /// <summary>
    /// determines whether this rectangle overlaps with another one
    /// </summary>
    /// <param name="r">the other rectangle</param>
    /// <returns>true if they overlap, false otherwise</returns>
    public bool Overlaps(Rectangle r) =>
        r.LowerX < HigherX && r.HigherX > LowerX && r.LowerY < HigherY && r.HigherY > LowerY;

    public override string ToString() => $"[{LowerX}, {LowerY}]:[{HigherX}, {HigherY}]";
}