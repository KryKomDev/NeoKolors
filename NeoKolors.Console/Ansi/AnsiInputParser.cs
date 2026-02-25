// NeoKolors
// Copyright (c) 2025 KryKom

using System.Text;
using Metriks;
using NeoKolors.Console.Ansi.Mouse;
using NeoKolors.Console.Events;
using NeoKolors.Console.Input;

namespace NeoKolors.Console.Ansi;

/// <summary>
/// A stateful parser for ANSI escape sequences and console input.
/// </summary>
public class AnsiInputParser {
    private static readonly NKLogger LOGGER = NKDebug.GetLogger("AnsiInputParser");
  
    public event MouseEventHandler?      Mouse;
    public event KeyEventHandler?        Key;
    public event FocusInEventHandler?    FocusIn;
    public event FocusOutEventHandler?   FocusOut;
    public event PasteEventHandler?      Paste;
    public event VTQueryResponseHandler? VTQuery;

    private readonly Func<ConsoleKeyInfo> _readNext;
    private readonly Action<Action>       _invoke;

    public AnsiInputParser(Func<ConsoleKeyInfo> readNext, Action<Action> invoke) {
        _readNext = readNext;
        _invoke   = invoke;
    }

    /// <summary>
    /// Parses the next input character/key.
    /// </summary>
    /// <param name="info">The key info to parse.</param>
    public void Parse(ConsoleKeyInfo info) {
        if (info is { KeyChar: '\e', Modifiers: 0 }) {
            HandleEsc();
        }
        else {
            _invoke(() => Key?.Invoke(new KeyEventArgs(info)));
        }
    }

    private void HandleEsc() {
        var next = _readNext();
        switch (next.KeyChar) {
            case '[': HandleCsi(); break;
            case ']': HandleOsc(); break;
            default:
                _invoke(() => Key?.Invoke(new KeyEventArgs(ConsoleKey.Escape, 0, '\e')));
                _invoke(() => Key?.Invoke(new KeyEventArgs(next)));
                break;
        }
    }

    private void HandleCsi() {
        var next = _readNext();
        switch (next.KeyChar) {
            case 'O': {
                _invoke(() => FocusOut?.Invoke());
            } break;
            case 'I': {
                _invoke(() => FocusIn?.Invoke());
            } break;
            case 'M': {
                HandleX10Mouse(); 
            } break;
            case '1': {
                ReadUntil('t'); 
                _invoke(() => {
                    VTQuery?.Invoke(VTQueryResponse.WinState(true));
                });
            } break;
            case '2': {
                var k = _readNext();
                switch (k.KeyChar) {
                    case '0': {
                        HandlePaste();
                    } break;
                    case 't': {
                        _invoke(() => {
                            VTQuery?.Invoke(VTQueryResponse.WinState(false));
                        });
                    } break;
                }
            } break;
            case '3': {
                HandleWinOps((x, y) => {
                    _invoke(() => VTQuery?.Invoke(VTQueryResponse.WinPos(new Point2D(x, y))));
                });
            } break;
            case '4': {
                HandleWinOps((h, w) => {
                    _invoke(() => VTQuery?.Invoke(VTQueryResponse.WinSizePx(new Size2D(w, h))));
                });
            } break;
            case '8': {
                HandleWinOps((h, w) => {
                    _invoke(() => VTQuery?.Invoke(VTQueryResponse.WinSize(new Size2D(w, h))));
                });
            } break;
            case '9': {
                HandleWinOps((h, w) => {
                    _invoke(() => VTQuery?.Invoke(VTQueryResponse.ScreenSize(new Size2D(w, h))));
                });
            } break;
            case '?': {
                HandleDecReq();
            } break;
            case '<': {
                HandleSGRMouse();
            } break;
        }
    }

    private void HandleWinOps(Action<int, int> action) {
        var parts = ReadUntil('t').TrimStart(';').Split(';');
        if (parts.Length >= 2 && int.TryParse(parts[0], out var first) && int.TryParse(parts[1], out var second))
            action(first, second);
    }

    private void HandleOsc() {
        var next = _readNext();

        if (next.KeyChar is 'L') {
            var label = ReadUntil('\x0f');
            _invoke(() => VTQuery?.Invoke(VTQueryResponse.IconTitle(label)));
            return;
        }

        if (next.KeyChar is 'l') {
            var label = ReadUntil('\x0f');
            _invoke(() => VTQuery?.Invoke(VTQueryResponse.WinTitle(label)));
            return;
        }

        var mode = ReadUntil(';');

        if (mode == "4") {
            
        }
    }

    private void HandleDecReq() {
        var modeStr = ReadUntil(';');
        var typeStr = ReadUntil('$');
        
        _readNext(); // skip 'y'
        
        if (int.TryParse(modeStr, out var mode) && int.TryParse(typeStr, out var type))
            _invoke(() => VTQuery?.Invoke(VTQueryResponse.Dec(mode, (DecReqResponseType)type)));
    }

    private void HandleX10Mouse() {
        var t = _readNext(); 
        var x = _readNext(); 
        var y = _readNext();
        
        _invoke(() => Mouse?.Invoke(MouseEventDecomposer.DecomposeUtf8(t, x, y)));
    }

    private void HandleSGRMouse() {
        var content = ReadUntil('m', 'M', out var last);
        var parts   = content.Split(';');
       
        if (parts.Length != 3) return;

        if (!(
            int.TryParse(parts[0], out var t) &&
            int.TryParse(parts[1], out var x) &&
            int.TryParse(parts[2], out var y))
        ) return;
        
        _invoke(() => {
            Mouse?.Invoke(MouseEventDecomposer.DecomposeSGR(t, x, y, last));
        });
    }

    private void HandlePaste() {
        ReadUntil('~');
        var s = ReadUntilSequence("\e[201~");
        _invoke(() => Paste?.Invoke(s));
    }

    private string ReadUntil(char terminator) {
        var sb = new StringBuilder();
        
        while (true) {
            var c = _readNext().KeyChar;
            
            if (c == terminator)
                break;
        
            sb.Append(c);
        }
        
        return sb.ToString();
    }

    private string ReadUntil(char t1, char t2, out char last) {
        var sb = new StringBuilder();
        
        while (true) {
            last = _readNext().KeyChar;
            
            if (last == t1 || last == t2)
                break;
            
            sb.Append(last);
        }
        
        return sb.ToString();
    }

    private string ReadUntilSequence(string sequence) {
        var buffer = new StringBuilder();
        var index  = 0;
        
        while (true) {
            var ch = _readNext().KeyChar;
            buffer.Append(ch);
        
            if (ch == sequence[index]) {
                if (++index == sequence.Length) 
                    return buffer.ToString()[..^sequence.Length];
            }
            else {
                index = ch == sequence[0] 
                    ? 1 
                    : 0;
            }
        }
    }
}
