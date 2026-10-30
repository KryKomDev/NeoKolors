// NeoKolors
// Copyright (c) 2026 KryKom

using System.Text;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Widgets.Rendering;

/// <summary>
/// A high-performance double-buffered character canvas presenter specifically designed for inline console widgets.
/// Uses relative ANSI cursor sequences to overwrite previous lines without requiring full-screen alternate buffers.
/// </summary>
public class NKInlineScreen : NKCharCanvas, IInlineScreen {
    private readonly StringBuilder _sb             = new();
    private readonly object        _renderLock     = new();
    private          int           _renderedHeight = 0;

    /// <inheritdoc/>
    public int RenderedHeight {
        get {
            lock (_renderLock) {
                return _renderedHeight;
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of <see cref="NKInlineScreen"/> with the specified dimensions.
    /// </summary>
    public NKInlineScreen(Size2D size) : base(Math.Max(1, size.X), Math.Max(1, size.Y)) { }

    /// <summary>
    /// Initializes a new instance of <see cref="NKInlineScreen"/> with the specified width and height.
    /// </summary>
    public NKInlineScreen(int width, int height) : base(Math.Max(1, width), Math.Max(1, height)) { }

    /// <inheritdoc/>
    public void Render() {
        lock (_renderLock) {
            _sb.Clear();

            // 1. Move cursor back to the starting line of the previously rendered frame
            if (_renderedHeight > 1)
                _sb.Append($"\e[{_renderedHeight - 1}A\r");
            else if (_renderedHeight == 1)
                _sb.Append('\r');

            var currentHeight = Height;
            var currentWidth  = Width;
            var prevStyle     = NKStyle.Default;

            // 2. Render each row
            for (var y = 0; y < currentHeight; y++) {
                if (y > 0)
                    _sb.Append("\r\n");
                else
                    _sb.Append('\r');

                // Find the last styled or non-empty character to avoid printing excess spaces
                var lastActiveX = -1;

                for (var x = currentWidth - 1; x >= 0; x--) {
                    var cell = _data[x, y];

                    if ((cell.Char != '\0' && cell.Char != ' ') || cell.Style != NKStyle.Default) {
                        lastActiveX = x;

                        break;
                    }
                }

                var renderCount = lastActiveX >= 0 ? lastActiveX + 1 : 0;

                for (var x = 0; x < renderCount; x++) {
                    var cell = _data[x, y];

                    if (cell.Style != prevStyle) {
                        _sb.Append(NKStyle.GetEscSeq(prevStyle, cell.Style));
                        prevStyle = cell.Style;
                    }

                    var c = cell.Char;
                    _sb.Append(char.IsControl(c) || c == '\0' ? ' ' : c);
                }

                // Reset style and erase to the end of the line
                if (prevStyle != NKStyle.Default) {
                    _sb.Append("\e[0m");
                    prevStyle = NKStyle.Default;
                }

                _sb.Append("\e[K");
            }

            // 3. If previous render had more rows than current, clear trailing leftover lines below
            if (_renderedHeight > currentHeight) {
                var extraLines = _renderedHeight - currentHeight;

                for (var i = 0; i < extraLines; i++)
                    _sb.Append("\r\n\e[2K");

                _sb.Append($"\e[{extraLines}A");
            }

            _renderedHeight = currentHeight;

            if (_sb.Length > 0)
                NKConsole.Write(_sb.ToString());
        }
    }

    /// <inheritdoc/>
    public void Finish(bool persist = true) {
        lock (_renderLock) {
            if (_renderedHeight == 0)
                return;

            _sb.Clear();

            if (persist) {
                // Ensure styles reset and advance cursor to the line directly below the widget
                _sb.Append("\e[0m\r\n");
            }
            else {
                // Wipe the rendered widget lines completely
                if (_renderedHeight > 1)
                    _sb.Append($"\e[{_renderedHeight - 1}A\r");
                else
                    _sb.Append('\r');

                for (var y = 0; y < _renderedHeight; y++) {
                    _sb.Append("\e[2K");

                    if (y < _renderedHeight - 1)
                        _sb.Append("\r\n");
                }

                if (_renderedHeight > 1)
                    _sb.Append($"\e[{_renderedHeight - 1}A\r");
                else
                    _sb.Append('\r');
            }

            _renderedHeight = 0;

            if (_sb.Length > 0)
                NKConsole.Write(_sb.ToString());
        }
    }
}