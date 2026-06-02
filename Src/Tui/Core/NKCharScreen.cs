// NeoKolors
// Copyright (c) 2026 KryKom

using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;

#if NK_RENDERING_PROFILING
using System.Diagnostics;
#endif

namespace NeoKolors.Tui.Core;

public class NKCharScreen : NKCharCanvas, ICharScreen {

    #if NK_RENDERING_PROFILING
    private readonly Stopwatch _writingTime     = new();
    private readonly Stopwatch _screenTotalTime = new();
    private readonly Stopwatch _sixelTime       = new();
    private readonly Stopwatch _accessTime      = new();
    private readonly Stopwatch _positionTime    = new();
    private readonly Stopwatch _escseqTime      = new();
    
    public TimeSpan WritingTime => _writingTime.Elapsed;
    public TimeSpan CompTime => _screenTotalTime.Elapsed - _writingTime.Elapsed;
    public TimeSpan ScrTotalTime => _screenTotalTime.Elapsed;
    public TimeSpan SixelTime => _sixelTime.Elapsed;
    public TimeSpan AccessTime => _accessTime.Elapsed;
    public TimeSpan PosTime => _positionTime.Elapsed;
    public TimeSpan EscseqTime => _escseqTime.Elapsed;
    #endif
    
    private readonly List<SixelImageInfo> _prevImages = [];
    private readonly StringBuilder        _sb         = new();
    
    public NKCharScreen(int width, int height) : base(width, height) { }
    public NKCharScreen(Size2D size) : base(size.X, size.Y) { }
    
    public void Render() {
        var prevStyle = NKStyle.Default;

        #if NK_RENDERING_PROFILING
        _screenTotalTime.Start();
        #endif

        _sb.Clear();
        _sb.Append("\e[0m");
        
        for (int y = 0; y < Height; y++) {
            prevStyle = RenderLine(y, prevStyle);
        }

        #if NK_RENDERING_PROFILING
        _screenTotalTime.Stop();
        #endif
        
        var ri = _images.Except(_prevImages).ToArray();
        
        for (int i = 0; i < ri.Length; i++) {
            var b = ri[i];
            
            #if NK_RENDERING_PROFILING
            _sixelTime.Start();
            #endif
            
            NKConsole.WriteSixel(b.Image, b.Offset.X, b.Offset.Y, b.Size.X, b.Size.Y);
            
            #if NK_RENDERING_PROFILING
            _sixelTime.Stop();
            #endif
        }

        _prevImages.Clear();
        _prevImages.AddRange(_images);
        _images.Clear();
    }

    private NKStyle RenderLine(int y, NKStyle prevStyle) {
        var isBehind  = true;

        for (int x = 0; x < Width; x++) {
            
            #if NK_RENDERING_PROFILING
            _accessTime.Start();
            #endif
            
            // get the cell data
            var cell = _data[x, y];

            #if NK_RENDERING_PROFILING
            _accessTime.Stop();
            #endif
                
            var (c, nkStyle, changed, _) = cell;
                
            if (!changed) {
                if (_sb.Length > 0) {
                    
                    #if NK_RENDERING_PROFILING
                    _writingTime.Start();
                    #endif
                        
                    NKConsole.Write(_sb.ToString());
                    _sb.Clear();
                        
                    #if NK_RENDERING_PROFILING
                    _writingTime.Stop();
                    #endif
                }
                isBehind = true;
                continue;
            }

            cell.SetUpdated();

            if (isBehind) {

                #if NK_RENDERING_PROFILING
                _positionTime.Start();
                #endif
                
                NKConsole.TrySetCursorPosition(x, y);
                
                #if NK_RENDERING_PROFILING
                _positionTime.Stop();
                #endif
            }

            isBehind = false;

            #if NK_RENDERING_PROFILING
            _escseqTime.Start();
            #endif
            
            _sb.Append(NKStyle.GetEscSeq(prevStyle, nkStyle));
            
            #if NK_RENDERING_PROFILING
            _escseqTime.Stop();
            #endif
            
            prevStyle = nkStyle;

            var actualChar = c ?? ' ';
            if (char.IsControl(actualChar))
                _sb.Append(' ');
            else
                _sb.Append(actualChar);
        }

        if (_sb.Length <= 0) 
            return prevStyle;
            
        #if NK_RENDERING_PROFILING
        _writingTime.Start();
        #endif
                    
        NKConsole.Write(_sb.ToString());
        _sb.Clear();
                    
        #if NK_RENDERING_PROFILING
        _writingTime.Stop();
        #endif
        return prevStyle;
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