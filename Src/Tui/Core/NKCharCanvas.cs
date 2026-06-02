// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using SkiaSharp;

namespace NeoKolors.Tui.Core;

public class NKCharCanvas : ICharCanvas {
    
    protected readonly List2D<CellInfo>     _data;
    protected readonly List<SixelImageInfo> _images;

    protected readonly bool _resize;
    
    public int Width   => _data.XSize;
    public int Height  => _data.YSize;
    public Size2D Size => _data.Size;
    
    public CellInfo this[int x, int y] { get => _data[x, y]; set => _data[x, y] = value; }
    
    public NKCharCanvas(int width, int height, bool resize = false) {
        _data = new List2D<CellInfo>(width, height);
        _data.Resize(width, height);
        _data.Fill(CellInfo.GetDefault);
        _images = [];
        _resize = resize;
    }

    public NKCharCanvas() {
        _data = new List2D<CellInfo>();
        _data.Resize(NKConsole.WindowSize.X, NKConsole.WindowSize.Y);
        _data.Fill(CellInfo.GetDefault);
        _images = [];
        _resize = false;
    }
    
    public void Place(ICharCanvas canvas, Point2D offset = default) {
        _data.Place(canvas.GetCellsArray(), (t, p) => t.ZIndex <= p.ZIndex, offset, _resize);
    }

    public void Place(NKCharCanvas canvas, Point2D offset = default) {
        _data.Place(canvas._data, (t, p) => t.ZIndex <= p.ZIndex, offset, _resize);
    }
    
    public void Place(CellInfo[,] cells, Point2D offset = default) {
        _data.Place(cells, (t, p) => t.ZIndex <= p.ZIndex, offset, _resize);
    }

    public void Place(char?[,] chars, Point2D offset = default, int zIndex = 0) {
        var sx = chars.GetLength(0) + offset.X;
        var sy = chars.GetLength(1) + offset.Y;
        
        if (_resize && (sx > _data.XSize || sy > _data.YSize))
            _data.Expand(Math.Max(sx, _data.XSize), Math.Max(sy, _data.YSize), CellInfo.GetDefault);
        
        for (int x = offset.X; x < Math.Min(chars.Len0 + offset.X, _data.XSize); x++) {
            for (int y = offset.Y; y < Math.Min(chars.Len1 + offset.Y, _data.YSize); y++) {
                var cellInfo = _data[x, y];
                
                if (cellInfo.ZIndex <= zIndex)
                    cellInfo.Char = chars[x - offset.X, y - offset.Y];
            }
        }
    }

    public void Place(char[,] chars, Point2D offset = default, int zIndex = 0) {
        var sx = chars.GetLen0() + offset.X;
        var sy = chars.GetLen1() + offset.Y;
        
        if (_resize && (sx > _data.XSize || sy > _data.YSize))
            _data.Expand(Math.Max(sx, _data.XSize), Math.Max(sy, _data.YSize), CellInfo.GetDefault);
        
        for (int x = offset.X; x < Math.Min(chars.Len0 + offset.X, _data.XSize); x++) {
            for (int y = offset.Y; y < Math.Min(chars.Len1 + offset.Y, _data.YSize); y++) {
                var cellInfo = _data[x, y];
                
                if (cellInfo.ZIndex <= zIndex)
                    cellInfo.Char = chars[x - offset.X, y - offset.Y];
            }
        }
    }

    public void Place(string str, Point2D offset = default, int zIndex = 0) {
        if (offset.Y != ..^Height)
            return;

        int xi = 0;
        
        for (int x = Math.Clamp(offset.X, 0, Width); x < Math.Clamp(offset.X + str.Length, 0, Width); x++) {
            if (_data[x, offset.Y].ZIndex <= zIndex) {
                _data[x, offset.Y].Char = str[xi];
            }

            xi++;
        }
    }

    public void Place(AnsiString str, Point2D offset = default, int zIndex = 0) {
        if (offset.Y != ..^Height)
            return;

        int xi = 0;
        var chars = str.ToArray();
        
        for (int x = Math.Clamp(offset.X, 0, Width); x < Math.Clamp(offset.X + str.Length, 0, Width); x++) {
            var cellInfo = _data[x, offset.Y];
            if (cellInfo.ZIndex <= zIndex) {
                cellInfo.Char  = chars[xi].Char;
                cellInfo.Style = cellInfo.Style.OverrideWith(chars[xi].Style);
            }

            xi++;
        }
    }
    
    public void Place(AnsiChar c, Point2D offset = default, int zIndex = 0) {
        if (offset.X != ..^Width || offset.Y != ..^Height) 
            return;
        
        var cellInfo = _data[offset.X, offset.Y];

        if (cellInfo.ZIndex > zIndex)
            return;
        
        cellInfo.Char  = c.Char;
        cellInfo.Style = cellInfo.Style.OverrideWith(c.Style);
    }

    public void Restyle(NKStyle[,] styles, Point2D offset = default, int zIndex = 0) {
        for (int x = offset.X; x < Math.Min(styles.Len0 + offset.X, _data.XSize); x++) {
            for (int y = offset.Y; y < Math.Min(styles.Len1 + offset.Y, _data.YSize); y++) {
                var cellInfo = _data[x, y];
                
                if (cellInfo.ZIndex <= zIndex)
                    cellInfo.Style = styles[x - offset.X, y - offset.Y];
            }
        }
    }
    
    public void PlaceSixel(SKBitmap image, Point2D offset, Size2D size, Size2D charSize, int zIndex = 0) {
        _images.Add(new SixelImageInfo(image, size, charSize, offset, zIndex));

        // calculate visible region
        var startX = Math.Max(0, offset.X);
        var startY = Math.Max(0, offset.Y);
        var endX   = Math.Min(_data.XSize, offset.X + charSize.X);
        var endY   = Math.Min(_data.YSize, offset.Y + charSize.Y);

        var width  = endX - startX;
        var height = endY - startY;

        if (width > 0 && height > 0)
            _data.Fill(() => CellInfo.GetNull(zIndex), new Point2D(startX, startY), new Size2D(width, height));
    }

    public ISixelImageInfo[] GetSixels() {
        return _images.Select(i => (ISixelImageInfo)i).ToArray();
    }

    public void Resize(int width, int height) {
        _data.Resize(width, height, CellInfo.GetDefault);
    }

    public void Clean() {
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                var cell = _data[x, y];
                if (cell != null) {
                    cell.Char = ' ';
                    cell.Style = NKStyle.Default;
                    cell.ZIndex = int.MinValue;
                }
            }
        }
    }
    
    public void Clear() {
        _data.Clear();
    }
    
    public CellInfo[,] GetCellsArray() {
        return _data.ToArray();
    }
}