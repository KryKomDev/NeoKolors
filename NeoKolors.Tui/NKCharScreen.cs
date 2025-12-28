// NeoKolors
// Copyright (c) 2025 KryKom

using System.Text;
using Metriks;
using SkiaSharp;

namespace NeoKolors.Tui;

public class NKCharScreen : NKCharCanvas, ICharScreen {
    
    public NKCharScreen(int width, int height) : base(width, height) { }
    public NKCharScreen(Size size) : base(size.Width, size.Height) { }
    
    public void Render() {
        var isBehind   = true;
        var lastFColor = NKColor.Default;
        var lastBColor = NKColor.Default;
        var lastStyle  = TextStyles.NONE;
        
        for (int y = 0; y < Height; y++) {
            Stdio.SetCursorPosition(0, y);
            var sb = new StringBuilder();
            
            for (int x = 0; x < Width; x++) {
                var cell = _data[x, y];
                var (c, nkStyle, _, _) = cell;
                
                if (!cell.Changed) {
                    Stdio.Write(sb.ToString());
                    sb.Clear();
                    isBehind = true;
                    continue;
                }

                cell.Changed = false;

                if (isBehind) 
                    Stdio.SetCursorPosition(x, y);
                
                isBehind = false;

                if (lastFColor != nkStyle.FColor) {
                    sb.Append(nkStyle.FColor.Text);
                    lastFColor = nkStyle.FColor;
                }

                if (lastBColor != nkStyle.BColor) {
                    sb.Append(nkStyle.BColor.Bckg);
                    lastBColor = nkStyle.BColor;
                }

                if (lastStyle != nkStyle.Styles) {
                    sb.Append(nkStyle.Styles);
                    lastStyle = nkStyle.Styles;
                }

                if (c is null || char.IsControl(c.Value))
                    sb.Append(' ');
                else
                    sb.Append(c);
            }

            if (!isBehind) {
                Stdio.Write(sb.ToString());
            }
            
            isBehind = true;
        }
    }
    
    // ============================= TESTING OVERRIDE METHODS ============================= // 
    
    #if NK_IMMEDIATE_RENDERING
    
    public new CellInfo this[int x, int y] {
        get => base[x, y];
        set {
            base[x, y] = value;
            Render();
        }
    }
    
    public new void Place(ICharCanvas canvas, Point2D offset = default) {
        base.Place(canvas, offset);
        Render();
    }
    
    public new void Place(CellInfo[,] cells, Point2D offset = default) {
        base.Place(cells, offset);
        Render();
    }
    
    public new void Place(char?[,] chars, Point2D offset = default, int zIndex = 0) {
        base.Place(chars, offset, zIndex);
        Render();
    }
    
    public new void PlaceString(string str, Point2D offset = default, int zIndex = 0) {
        base.PlaceString(str, offset, zIndex);
        Render();
    }
    
    public new void Restyle(NKStyle[,] styles, Point2D offset = default, int zIndex = 0) {
        base.Restyle(styles, offset, zIndex);
        Render();
    }
    
    public new void PlaceSixel(SKImage image, Point2D offset, Size2D size, int zIndex = 0) {
        base.PlaceSixel(image, offset, size, zIndex);
        Render();
    }
    
    public new void Resize(int width, int height) {
        base.Resize(width, height);
        Render();
    }
    
    public new void Clear() {
        base.Clear();
        Render();
    }
    
    #endif
}