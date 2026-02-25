//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.Diagnostics;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console.Ansi;
using NeoKolors.Console.Events;
using NeoKolors.Console.Input;

namespace NeoKolors.Console.Driver.Windows;

public sealed class WindowsInputDriver : IInputDriver {
    
    private static readonly NKLogger LOGGER = NKDebug.GetLogger(nameof(WindowsInputDriver));

    public event MouseEventHandler?    Mouse    = (_) => { };
    public event KeyEventHandler?      Key      = (_) => { };
    public event FocusInEventHandler?  FocusIn  = ( ) => { };
    public event FocusOutEventHandler? FocusOut = ( ) => { };
    public event ResizeEventHandler?   Resize   = (_) => { };
    public event PasteEventHandler?    Paste    = (_) => { };
    
    public event VTQueryResponseHandler? VTQuery {
        add    => _parser.VTQuery += value;
        remove => _parser.VTQuery -= value;
    }
    
    private          bool                 _useReadKey;
    private          bool                 _disposed = false;
    private          Thread?              _inputThread;
    private readonly WindowsInput         _input;
    private readonly AnsiInputParser      _parser;
    private readonly WinInputDriverConfig _config;

    private readonly Queue<EscapeCodes.DecMode> _decReqQueue = new();
    private readonly Queue<EscapeCodes.OscMode> _oscReqQueue = new();
    
    public void RequestDecMode(EscapeCodes.DecMode mode) => _decReqQueue.Enqueue(mode);
    public void RequestOscMode(EscapeCodes.OscMode mode) => _oscReqQueue.Enqueue(mode);

    public bool IsRunning { get; private set; }

    public WindowsInputDriver(WinInputDriverConfig config) {
        _config = config;
        _input  = new WindowsInput();
        _parser = new AnsiInputParser(ReadNext, Invoke);

        // enable ctrl+c force quit
        if (_config.CtrlCForceQuits) {
            Key += a => {
                if (a is { Down: true, Key: KeyCode.C } && a.Modifiers.GetHasCtrl())
                    Process.GetCurrentProcess().Kill();
            };
        }
    }
    
    public void Start() {
        if (IsRunning || _disposed) 
            return;
        
        IsRunning    = true;
        _inputThread = new Thread(Intercept) {
            IsBackground = true,
            Priority     = ThreadPriority.BelowNormal,
            Name         = "NeoKolors Win32 Input Interceptor"
        };
        
        _inputThread.Start();
    }

    public void Stop() {
        IsRunning = false;
    }

    private void Intercept() {
        var stopwatch = new Stopwatch();
        
        while (IsRunning) {
            stopwatch.Restart();

            if (_input.TryGetInput(out var records)) {
                foreach (var r in records) {
                    Decode(r);
                }
            }
            
            // DECREQ and OSCREQ
            
            var elapsed = stopwatch.Elapsed;

            if (elapsed <= _config.RefreshInterval && !_input.HasInput()) {
                Thread.Sleep(_config.RefreshInterval - elapsed);
            }
        }
    }

    private void Decode(WinImports.WinInputRecord record) {
        switch (record.Type) {
            case WinImports.WinEventType.KEY:    { InvokeKey(record.Key);         } break;
            case WinImports.WinEventType.MOUSE:  { InvokeMouse(record.Mouse);     } break;
            case WinImports.WinEventType.RESIZE: { InvokeResize(record.Resize);   } break;
            case WinImports.WinEventType.MENU:   { /* ignored, used internally */ } break;
            case WinImports.WinEventType.FOCUS:  { InvokeFocus(record.Focus);     } break;
            default:                             throw new ArgumentOutOfRangeException();
        }
    }

    private void InvokeKey(WinImports.WinKeyEvent keyEvent) {
        Key?.Invoke(new KeyEventArgs(
            (KeyCode)keyEvent.VirtualKeyCode, 
            (KeyModifiers)keyEvent.Modifiers, 
            keyEvent.Char,
            keyEvent.Down
        ));
    }

    private void InvokeResize(WinImports.WinResizeEvent resizeEvent) => 
        Resize?.Invoke(new ResizeEventArgs(new Size2D(resizeEvent.Size.X, resizeEvent.Size.Y)));

    private void InvokeMouse(WinImports.WinMouseEvent mouseEvent) {
        var mb = mouseEvent.Button;
        
        // translate the key press
        var button = mb switch {
            _ when mouseEvent.Flags.GetHasWheel() => 
                mouseEvent.Button < 0 
                    ? MouseButton.WHEEL_DOWN 
                    : MouseButton.WHEEL_UP,
            _ when mb.GetHasLmb() => MouseButton.LEFT,
            _ when mb.GetHasRmb() => MouseButton.RIGHT,
            _ when mb.GetHasMb2() => MouseButton.MIDDLE,
            _ when mb.GetHasMb3() => MouseButton.MB6,
            _ when mb.GetHasMb4() => MouseButton.MB7,
            0                     => MouseButton.RELEASE,
            _                     => throw new InvalidOperationException()
        };
        
        Mouse?.Invoke(new MouseEventArgs(
            button,
            (KeyModifiers)mouseEvent.Modifiers,
            new Point2D(mouseEvent.Coord.X, mouseEvent.Coord.Y),
            mb == 0,
            mouseEvent.Flags.GetHasMoved()
        ));
    }

    private void InvokeFocus(WinImports.WinFocusEvent focusEvent) {
        if (focusEvent.FocusSet) 
            FocusIn?.Invoke();
        else 
            FocusOut?.Invoke();
    }

    private ConsoleKeyInfo ReadNext() {
        if (_useReadKey) {
            try {
                return Stdio.ReadKey(true);
            }
            catch (InvalidOperationException) {
                _useReadKey = false;
            }
        }
        
        int c = Stdio.Read();
        
        return c == -1 
            ? throw new EndOfStreamException() 
            : new ConsoleKeyInfo((char)c, 0, false, false, false);
    }

    private void Invoke(Action action) {
        Task.Run(() => {
            if (IsRunning) action();
        });
    }
    
    public void Dispose() {
        if (_disposed) 
            return;
        
        Stop();
        
        _disposed = true;
        _input.Dispose();
    }
}