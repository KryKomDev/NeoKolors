// NeoKolors
// Copyright (c) 2025 KryKom

using Metriks;
using SkiaSharp;

namespace NeoKolors.Tui;

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
        _data.Fill(() => new CellInfo(null, NKStyle.Default, true, 0));
        _images = [];
        _resize = resize;
    }

    public NKCharCanvas() {
        _data = new List2D<CellInfo>();
        _data.Resize(Stdio.WindowWidth, Stdio.WindowHeight);
        _data.Fill(() => new CellInfo(null, NKStyle.Default, true, 0));
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

    public void PlaceString(string str, Point2D offset = default, int zIndex = 0) {
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
    
    public void Restyle(NKStyle[,] styles, Point2D offset = default, int zIndex = 0) {
        for (int x = offset.X; x < Math.Min(styles.Len0 + offset.X, _data.XSize); x++) {
            for (int y = offset.Y; y < Math.Min(styles.Len1 + offset.Y, _data.YSize); y++) {
                var cellInfo = _data[x, y];
                
                if (cellInfo.ZIndex <= zIndex)
                    cellInfo.Style = styles[x - offset.X, y - offset.Y];
            }
        }
    }
    
    public void PlaceSixel(SKImage image, Point2D offset, Size2D size, int zIndex = 0) {
        _images.Add(new SixelImageInfo(image, size, offset, zIndex));
    }

    public ISixelImageInfo[] GetSixelImages() {
        return _images.Select(i => (ISixelImageInfo)i).ToArray();
    }

    public void Resize(int width, int height) {
        _data.Resize(width, height, new CellInfo(null, NKStyle.Default, true, 0));
    }
   
    public void Clear() {
        _data.Clear();
    }
    
    public CellInfo[,] GetCellsArray() {
        return _data.ToArray();
    }
}