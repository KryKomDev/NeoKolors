// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using SkiaSharp;

namespace NeoKolors.Tui.Rendering;

public interface ICharCanvas {
    
    public int Width   { get; }
    public int Height  { get; }
    public Size2D Size { get; }
    
    public CellInfo this[int x, int y] { get; set; }
    
    public void Place(ICharCanvas canvas, Point2D offset = default);
    public void Place(CellInfo[,] cells,  Point2D offset = default);
    public void Place(char?[,]    chars,  Point2D offset = default, int zIndex = 0);
    public void Place(string str, Point2D offset = default, int zIndex = 0);
    public void Place(AnsiString str, Point2D offset = default, int zIndex = 0);
    public void Place(AnsiChar c, Point2D offset = default, int zIndex = 0);
    
    public void Restyle(NKStyle[,] styles, Point2D offset = default, int zIndex = 0);
    
    public void PlaceSixel(SKBitmap image, Point2D offset, Size2D size, Size2D charSize, int zIndex = 0);
    public ISixelImageInfo[] GetSixels();
    
    public void Resize(int width, int height);
    public void Clean();
    public void Clear();
    
    public CellInfo[,] GetCellsArray();
}