namespace NeoKolors.ConsoleGraphics.TUI;

public struct Rectangle {
    private int lowerX;
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

    private int lowerY;
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

    private int higherX;
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

    private int higherY;
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
    
    public int Width => HigherX - LowerX;
    public int Height => HigherY - LowerY;

    public bool IsInside(int x, int y) => x >= lowerX && x <= higherX && y >= lowerY && y <= higherY;
}