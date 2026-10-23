// NeoKolors
// Copyright (c) krystof 2026

using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;

namespace NeoKolors.Tui.Core;

public class NKCharScreen : NKCharCanvas, ICharScreen {

    private          List2D<NKCell>?         _prevData;
    private          List<NKImageData>?      _prevImages;
    private readonly Profiler.ScreenProfiler _screenProfiler;
    private readonly StringBuilder           _sb         = new();
    private readonly object                  _renderLock = new();

    public NKCharScreen(Size2D size) : base(size) {
        _screenProfiler = new Profiler.ScreenProfiler();
        Profiler.Global.Register(_screenProfiler);
    }

    public NKCharScreen(int x, int y) : base(x, y) {
        _screenProfiler = new Profiler.ScreenProfiler();
        Profiler.Global.Register(_screenProfiler);
    }

    public void Render() {
        lock (_renderLock) {
            _screenProfiler.Start_Screen_Total();

            ForceRenderImages();

            RenderChars();

            _screenProfiler.Stop_Screen_Total();
        }
    }

    private void ForceRenderImages() {
        _screenProfiler.Start_Screen_RenderImage();
       
        for (int i = 0; i < _images.Count; i++) {
            var img = _images[i];

            NKConsole.WriteImage(img.Image, img.Position, img.Size, img.SamplingOptions);
        }

        _prevImages = [.. _images];
        
        _screenProfiler.Stop_Screen_RenderImage();
    }
    
    private void RenderImages() {
        throw new NotImplementedException();
        
        // _screenProfiler.Start_Screen_RenderImage();
        //
        // _screenProfiler.Stop_Screen_RenderImage();
    }

    private void ForceRenderChars() => RenderCharsInternal(force: true);

    private void RenderChars() => RenderCharsInternal(force: false);

    private void RenderCharsInternal(bool force) {
        _screenProfiler.Start_Screen_RenderChars();

        if (_prevData == null) {
            _prevData = ListPool2D<NKCell>.Rent(Width, Height);
            _prevData.Resize(Width, Height);
            force = true;
        }
        else if (_prevData.Size != Size) {
            ListPool2D<NKCell>.Return(_prevData);
            _prevData = ListPool2D<NKCell>.Rent(Width, Height);
            _prevData.Resize(Width, Height);
            force = true;
        }

        _sb.Clear();
        var prevStyle = NKStyle.Default;
        var ys = _data.YSize;
        var xs = _data.XSize;

        for (int y = 0; y < ys; y++) {
            var isBehind = true;

            for (int x = 0; x < xs; x++) {
                var cell = _data[x, y];

                if (!force && cell == _prevData[x, y]) {
                    isBehind = true;
                    continue;
                }

                if (isBehind) {
                    _sb.Append($"\e[{y + 1};{x + 1}H");
                    isBehind = false;
                }

                if (cell.Style != prevStyle) {
                    _sb.Append(NKStyle.GetEscSeq(prevStyle, cell.Style));
                    prevStyle = cell.Style;
                }

                var c = cell.Char;
                _sb.Append(char.IsControl(c) || c == '\0' ? ' ' : c);
            }
        }

        if (_sb.Length > 0) {
            if (prevStyle != NKStyle.Default) {
                _sb.Append("\e[0m");
            }

            NKConsole.Write(_sb.ToString());
        }

        _prevData.CopyFrom(_data);

        _screenProfiler.Stop_Screen_RenderChars();
    }

    public new void Dispose() {
        if (_prevData != null) {
            _prevData.Clear();
            ListPool2D<NKCell>.Return(_prevData);
            _prevData = null;
        }

        base.Dispose();
        GC.SuppressFinalize(this);
    }
}