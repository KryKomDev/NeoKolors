// NeoKolors
// Copyright (c) krystof 2026

using System.Runtime.CompilerServices;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Extensions;
using SkiaSharp;

namespace NeoKolors.Tui.Core;

public class NKCharCanvas : ICharCanvas {
    protected List2D<NKCell>    _data;
    protected List2D<int>       _zLay;
    protected List<NKImageData> _images;

    public IReadOnlyList2D<NKCell> Data => _data;

    public Size2D Size {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _data.Size;
    }

    public int Width {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _data.XSize;
    }

    public int Height {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _data.YSize;
    }

    public int XSize {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _data.XSize;
    }

    public int YSize {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _data.YSize;
    }

    public NKCharCanvas(Size2D size) : this(size.X, size.Y) { }

    public NKCharCanvas(int x, int y) {
        _data   = ListPool2D<NKCell>.Rent(x, y);
        _zLay   = ListPool2D<int>.Rent(x, y);
        _images = [];

        _data.Resize(x, y);
        _zLay.Resize(x, y);

        _data.Fill(NKCell.Default);
        _zLay.Fill(int.MinValue);
    }

    #region Drawing

    public void Draw(ICharCanvas canvas, int xOffset, int yOffset, int z) {
        if (xOffset                 >= XSize ||
            yOffset                 >= YSize ||
            xOffset + canvas.Size.X < 0      ||
            yOffset + canvas.Size.Y < 0
        ) {
            return;
        }

        var xs = xOffset.BottomClamp(0);
        var ys = yOffset.BottomClamp(0);
        var xe = (xs + canvas.Size.X).TopClamp(XSize);
        var ye = (ys + canvas.Size.Y).TopClamp(YSize);

        // clear clusters

        if (xOffset > 0) {
            for (int yi = ys; yi < ye; yi++) {
                ClearClusterLeft(xOffset, yi);
            }
        }

        var r = xOffset + canvas.Size.X;

        if (r < XSize - 1) {
            for (int yi = ys; yi < ye; yi++) {
                ClearClusterRight(r, yi);
            }
        }

        // draw
        for (int x = xs; x < xe; x++) {
            for (int y = ys; y < ye; y++) {
                if (_zLay[x, y] > z || canvas.Data[x - xOffset, y - yOffset].Flags.GetIsTransparent())
                    continue;

                _data[x, y] = canvas.Data[x - xOffset, y - yOffset];
                _zLay[x, y] = z;
            }
        }
    }

    public void Draw(AnsiChar c, int xOffset, int yOffset, int z) {
        if (xOffset < 0      ||
            yOffset < 0      ||
            xOffset >= XSize ||
            yOffset >= YSize ||
            z       < _zLay[xOffset, yOffset]
        ) {
            return;
        }

        ClearCluster(xOffset, yOffset);

        var cell = new NKCell(c, NKCellFlags.SINGLE);
        _data[xOffset, yOffset] = cell with { Style = _data[xOffset, yOffset].Style.With(cell.Style) };
        _zLay[xOffset, yOffset] = z;
    }

    public void Draw(AnsiString str, int xOffset, int yOffset, int z) {
        if (AnsiString.IsNullOrEmpty(str) || yOffset >= YSize)
            return;

        var lines = str.Split('\n');

        if (yOffset + lines.Length < 0)
            return;

        for (int y = yOffset.BottomClamp(0); y < (yOffset + lines.Length).TopClamp(YSize); y++) {
            var line = lines[y - yOffset];

            if (xOffset > 0 && xOffset < XSize)
                ClearClusterLeft(xOffset, y);

            if (xOffset + line.Length >= 0 && xOffset + line.Length < XSize)
                ClearClusterRight(xOffset + line.Length, y);

            var cells = NKCell.FromString(line);

            for (int x = xOffset.BottomClamp(0); x < (xOffset + cells.Length).TopClamp(XSize); x++) {
                if (_zLay[x, y] > z)
                    continue;

                var cell = cells[x - xOffset];
                _data[x, y] = cell with { Style = _data[x, y].Style.With(cell.Style) };
                _zLay[x, y] = z;
            }
        }
    }

    public void Draw(
        SKImage           image,
        int               xOffset,
        int               yOffset,
        int               xSize,
        int               ySize,
        int               zLayer,
        SKSamplingOptions samplingOptions = default
    ) {
        _images.Add(
            new NKImageData {
                Image           = image,
                SamplingOptions = samplingOptions,
                ZLayer          = zLayer,
                Position        = new Point2D(xOffset, yOffset),
                Size            = new Size2D(xSize, ySize),
            }
        );

        for (int x = xOffset.BottomClamp(0); x < (xOffset + xSize).TopClamp(XSize); x++)
        for (int y = yOffset.BottomClamp(0); y < (yOffset + ySize).TopClamp(YSize); y++) {
            if (_zLay[x, y] > zLayer)
                continue;

            _zLay[x, y] = zLayer;
        }
    }

    public void DrawVerticalLine(AnsiChar c, int x, int y, int length, int z) {
        if (x < 0 || x >= XSize || y < 0 || y >= YSize)
            return;

        for (int yi = y.BottomClamp(0); yi < (y + length).TopClamp(YSize); yi++) {
            if (_zLay[x, yi] > z)
                continue;

            var cell = new NKCell(c, NKCellFlags.SINGLE);
            _data[x, yi] = cell with { Style = _data[x, yi].Style.With(cell.Style) };
            _zLay[x, yi] = z;
        }
    }

    public void DrawHorizontalLine(AnsiChar c, int x, int y, int length, int z) {
        if (y < 0 || y >= YSize || x >= XSize || x + length < 0)
            return;

        if (x >= 0)
            ClearClusterLeft(x, y);

        if (x + length < XSize)
            ClearClusterRight(x + length, y);

        for (int xi = x.BottomClamp(0); xi < (x + length).TopClamp(XSize); xi++) {
            if (_zLay[xi, y] > z)
                continue;

            var cell = new NKCell(c, NKCellFlags.SINGLE);
            _data[xi, y] = cell with { Style = _data[xi, y].Style.With(cell.Style) };
            _zLay[xi, y] = z;
        }
    }

    public void DrawRect(AnsiChar c, int x, int y, int width, int height, int z) {
        for (int yi = y.BottomClamp(0); yi < (y + height).TopClamp(YSize); yi++) {
            DrawHorizontalLine(c, x, yi, width, z);
        }
    }

    public void DrawBorder(NKBorderOld border, int x, int y, int width, int height, int z) {
        throw new NotImplementedException();
    }
    
    #endregion

    #region Helper Methods

    private void ClearCluster(int x, int y) {
        if (!_data[x, y].Flags.GetIsCluster())
            return;

        for (int xi = x; xi >= 0; xi--) {
            _data[xi, y] = NKCell.Default with { Style = _data[xi, y].Style };

            if (_data[xi, y].Flags.GetIsClusterStart() ||
                (xi - 1 >= 0 && !_data[xi - 1, y].Flags.GetIsCluster())
            ) {
                break;
            }
        }

        if (x + 1 < XSize && !_data[x + 1, y].Flags.GetIsClusterContinuation())
            return;

        for (int xi = x + 1; xi < XSize; xi++) {
            _data[xi, y] = NKCell.Default with { Style = _data[xi, y].Style };

            if (xi + 1 <= XSize                        ||
                !_data[xi + 1, y].Flags.GetIsCluster() ||
                _data[xi + 1, y].Flags.GetIsClusterStart()
            ) {
                break;
            }
        }
    }
    
    private void ClearClusterLeft(int x, int y) {
        if (!_data[x, y].Flags.GetIsCluster()) {
            return;
        }

        for (int xi = x; xi >= 0; xi--) {
            _data[xi, y] = NKCell.Default with { Style = _data[xi, y].Style };

            if (_data[xi, y].Flags.GetIsClusterStart() ||
                (xi - 1 >= 0 && !_data[xi - 1, y].Flags.GetIsCluster())
            ) {
                break;
            }
        }
    }

    private void ClearClusterRight(int x, int y) {
        if (!_data[x, y].Flags.GetIsCluster()) {
            return;
        }

        for (int xi = x; xi < XSize; xi++) {
            _data[xi, y] = NKCell.Default with { Style = _data[xi, y].Style };

            if (xi + 1 <= XSize                        ||
                !_data[xi + 1, y].Flags.GetIsCluster() ||
                _data[xi + 1, y].Flags.GetIsClusterStart()
            ) {
                break;
            }
        }
    }

    #endregion

    public void Resize(Size2D size) {
        _data.Resize(size.X, size.Y);
        _zLay.Resize(size.X, size.Y);
    }

    public void ResetZ() {
        _zLay.Fill(int.MinValue);
    }

    public void Clean() {
        _data.Fill(NKCell.Default);
        _zLay.Fill(int.MinValue);
    }

    public void Dispose() {
        if (_data is null)
            return;

        _data.Clear();
        ListPool2D<NKCell>.Return(_data);
        _data = null!;

        _zLay.Clear();
        ListPool2D<int>.Return(_zLay);
        _zLay = null!;

        GC.SuppressFinalize(this);
    }
}