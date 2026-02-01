// NeoKolors
// Copyright (c) 2025 KryKom

using System.Text;
using Metriks;
using NeoKolors.Console.Events;
using NeoKolors.Console.Mouse;
using Std = System.Console;

namespace NeoKolors.Console.Driver.DotNet;

public class DotNetInputDriver : IInputDriver {

    public event MouseEventHandler?          Mouse;
    public event KeyEventHandler?            Key;
    public event FocusInEventHandler?        FocusIn;
    public event FocusOutEventHandler?       FocusOut;
    public event PasteEventHandler?          Paste;
    public event WinOpsResponseEventHandler? WinOpsResponse;
    public event DecReqResponseEventHandler? DecReqResponse;

    protected bool _running;
    protected Thread? _inputThread;
    protected readonly NKLogger _logger = NKDebug.GetLogger("DotNetInputDriver");

    public virtual void Start() {
        if (_running) return;
        _running = true;
        _inputThread = new Thread(Intercept) {
            IsBackground = true,
            Priority = ThreadPriority.BelowNormal,
            Name = "NeoKolors Input Interceptor"
        };
        _inputThread.Start();
    }

    public virtual void Stop() {
        _running = false;
    }

    public virtual void Dispose() {
        Stop();
    }

    protected virtual void Intercept() {
        ConsoleKeyInfo i;
        
        try {
            i = Std.ReadKey(intercept: true);
        }
        catch (InvalidOperationException e) {
            // if the input is redirected or is not from a
            // console or the console is a weird mf like mintty
            _logger.Error(e.Message);
            InterceptCompat();
            return;
        }

        while (_running) {
            if (i is { KeyChar: '\e', Modifiers: 0 }) {
                HandleEscSeq();
            }
            else {
                var i1 = i;
                _ = Task.Run(() => Key?.Invoke(i1));
            }

            if (!_running) break;
            try {
                i = Std.ReadKey(intercept: true);
            } catch (InvalidOperationException) {
                break;
            }
        }
    }

    private void HandleEscSeq() {
        var introducer = Std.ReadKey(intercept: true);

        if (introducer.KeyChar == ']') {
            HandleOsc();
        }
        else if (introducer.KeyChar != '[') {
            _ = Task.Run(() => Key?.Invoke(new ConsoleKeyInfo(
                keyChar: '\e', 
                key: ConsoleKey.Backspace, 
                shift: false, 
                alt: false, 
                control: false)
            ));
            _ = Task.Run(() => Key?.Invoke(introducer));
            return;
        }
        
        HandleCsi();
    }

    private void HandleOsc() {
        var next = Std.ReadKey(intercept: true);

        switch (next.KeyChar) {
            case 'L':
                string label = ReadUntil('\x0f', true);
                _ = Task.Run(() => WinOpsResponse?.Invoke(WinOpsResponseArgs.IconLabel(label)));
                break;
            case 'l':
                string title = ReadUntil('\x0f', true);
                _ = Task.Run(() => WinOpsResponse?.Invoke(WinOpsResponseArgs.WinTitle(title)));
                break;
        }
    }

    private void HandleCsi() {
        var next = Std.ReadKey(intercept: true);
        
        switch (next.KeyChar) {
            case 'O': {
                _ = Task.Run(() => FocusOut?.Invoke()); 
                return;
            }
            case 'I': {
                _ = Task.Run(() => FocusIn?.Invoke());
                return;
            }
            case 'M': {
                HandleX10MouseEvent();  
                return;
            }
            case '1': {
                _ = Task.Run(() => WinOpsResponse?.Invoke(WinOpsResponseArgs.WinState(true))); 
                return;
            }
            case '2': {
                var k = Std.ReadKey(intercept: true);
                switch (k.KeyChar) {
                    case '0': {
                        HandlePaste();                                                                  return;
                    }
                    case 't': {
                        _ = Task.Run(() => WinOpsResponse?.Invoke(WinOpsResponseArgs.WinState(false))); return;
                    }
                    default : {
                        _logger.Error("How the fuck did this even happen?");                            return;
                    }
                }
            }
            case '3': {
                HandleWinPosResponse(); 
                return;
            }
            case '4': {
                HandleWinSizePxResponse();
                return;
            }
            case '8': {
                HandleWinSizeResponse();
                return;
            }
            case '9': {
                HandleScrSizeResponse();
                return;
            }
            case '?': {
                HandleDecReqResponse(); 
                return;
            }
            case '<': {
                HandleSGRMouseEvent();
                return;
            }
        }
    }

    private void HandleScrSizeResponse() {
        var split = ReadUntil('t', true)[1..].Split(';');
        var h = int.Parse(split[0]);
        var w = int.Parse(split[1]);
        _ = Task.Run(() => WinOpsResponse?.Invoke(WinOpsResponseArgs.ScrSize((w, h))));
    }
    
    private void HandleWinSizeResponse() {
        var split = ReadUntil('t', true)[1..].Split(';');
        var h = int.Parse(split[0]);
        var w = int.Parse(split[1]);
        _ = Task.Run(() => WinOpsResponse?.Invoke(WinOpsResponseArgs.WinSize(new Size2D(w, h))));
    }
    
    private void HandleWinSizePxResponse() {
        var split = ReadUntil('t', true)[1..].Split(';');
        var h = int.Parse(split[0]);
        var w = int.Parse(split[1]);
        _ = Task.Run(() => WinOpsResponse?.Invoke(WinOpsResponseArgs.WinSizePx(new Size2D(w, h))));
    }

    private void HandleWinPosResponse() {
        var split = ReadUntil('t', true)[1..].Split(';');
        var x = int.Parse(split[0]);
        var y = int.Parse(split[1]);
        _ = Task.Run(() => WinOpsResponse?.Invoke(WinOpsResponseArgs.WinPos((x, y))));
    }

    private void HandleDecReqResponse() {
        string rawType = ReadUntil(';', true);
        int mode;
        int type;

        try {
            mode = int.Parse(rawType);
        }
        catch (FormatException) {
            _logger.Error($"Invalid mode in DECREQ response: '{rawType}'.");
            return;
        }

        try {
            type = int.Parse(ReadUntil('$', true));
        }
        catch (FormatException) {
            _logger.Error($"Invalid type in DECREQ response: '{rawType}'.");
            return;
        }
        
        // skips the 'y'
        SkipKeys(1);
                
        _ = Task.Run(() => DecReqResponse?.Invoke(new DecReqResponseArgs(mode, (DecReqResponseType)type)));
    }

    private void HandleX10MouseEvent() {
        var type = Std.ReadKey(intercept: true);
        var x = Std.ReadKey(intercept: true);
        var y = Std.ReadKey(intercept: true);
        _ = Task.Run(() => Mouse?.Invoke(MouseEventDecomposer.DecomposeUtf8(type, x, y)));
    }

    private void HandleSGRMouseEvent() {
        string rawType = ReadUntil(';', true);
        string rawX = ReadUntil(';', true);
        string rawY = ReadUntil(out var last, true, 'm', 'M');

        int type;
        try {
            type = int.Parse(rawType);
        }
        catch {
            _logger.Error("Faulty mouse event type detected.");
            return;
        }
        
        int x;
        try {
            x = int.Parse(rawX);
        }
        catch {
            _logger.Error("Faulty mouse event x-axis coordinate detected.");
            return;
        }
        
        int y;
        try {
            y = int.Parse(rawY);
        }
        catch {
            _logger.Error("Faulty mouse event y-axis coordinate detected.");
            return;
        }
        
        _ = Task.Run(() => Mouse?.Invoke(MouseEventDecomposer.DecomposeSGR(type, x, y, last)));
    }

    private void HandlePaste() {
        SkipKeys(3);
        var s = ReadUntil("\e[201~", true);
        _ = Task.Run(() => Paste?.Invoke(s));
    }

    private void SkipKeys(int num) {
        for (int i = 0; i < num; i++) 
            Std.ReadKey(intercept: true);
    }

    private void InterceptCompat() {
        while (_running) {
            if (!Std.KeyAvailable) {
                Thread.Sleep(50); // Avoid tight loop
                continue;
            }
            
            var k = Std.Read();

            switch (k) {
                case -1: {
                    break;
                }
                case '\e': {
                    HandleEscapeCompat(); 
                    break;
                }
                default: {
                    Key?.Invoke(new ConsoleKeyInfo(
                        keyChar: (char)k, 
                        key: (ConsoleKey)char.ToLower((char)k), 
                        shift: char.IsUpper((char)k), 
                        alt: false, 
                        control: false));
                    break;
                }
            }
        }
    }

    private void HandleEscapeCompat() {
        int intro = -1;
        while (intro is -1) {
            intro = Std.Read(); 
        }

        if (intro is ']') {
            
        }
        else if (intro is not '[') {
            Key?.Invoke(new ConsoleKeyInfo(
                keyChar: '\e', 
                key: ConsoleKey.Backspace, 
                shift: false, 
                alt: false, 
                control: false)
            );
            Key?.Invoke(new ConsoleKeyInfo(
                keyChar: (char)intro, 
                key: (ConsoleKey)char.ToLower((char)intro), 
                shift: char.IsUpper((char)intro), 
                alt: false, 
                control: false)
            );
            return;
        }
        
        int next = -1;
        while (next is -1) {
            next = Std.Read(); 
        }
        
        switch (next) {
            case 'O':
                FocusOut?.Invoke();
                break;
            case 'I':
                FocusIn?.Invoke();
                break;
            case 'M':
                HandleX10MouseEvent();
                break;
            case '2':
                HandlePaste();
                break;
            case '<':
                HandleSGRMouseEvent();
                break;
        }
    }

    private static string ReadUntil(string sequence, bool intercept = false) {
        if (string.IsNullOrEmpty(sequence)) {
            throw new ArgumentException("Sequence cannot be null or empty.", nameof(sequence));
        }
    
        var buffer = new StringBuilder();
        var sequenceIndex = 0;
    
        while (true) {
            var key = Std.ReadKey(intercept);
            var ch = key.KeyChar;
        
            buffer.Append(ch);
        
            if (ch == sequence[sequenceIndex]) {
                sequenceIndex++;
                if (sequenceIndex != sequence.Length) 
                    continue;
                
                buffer.Length -= sequence.Length;
                return buffer.ToString();
            }

            sequenceIndex = 0;
            if (ch == sequence[0]) {
                sequenceIndex = 1;
            }
        }
    }

    private static string ReadUntil(out char last, bool intercept = false, params char[] oneOf) {
        var key = Std.ReadKey(intercept);
        string s = "";

        while (!oneOf.Contains(key.KeyChar)) {
            s += key.KeyChar;
            key = Std.ReadKey(intercept);
        }

        last = key.KeyChar;
        return s;
    }

    private string ReadUntil(char c, bool intercept = false) {
        var key = Std.ReadKey(intercept);
        string s = "";

        while (key.KeyChar != c) {
            s += key.KeyChar;
            key = Std.ReadKey(intercept);
        }

        return s;
    }
}