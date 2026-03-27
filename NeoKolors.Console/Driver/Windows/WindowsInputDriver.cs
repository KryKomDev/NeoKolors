//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.Diagnostics;
using Metriks;
using NeoKolors.Console.Ansi;
using NeoKolors.Console.Events;
using NeoKolors.Console.Input;

namespace NeoKolors.Console.Driver.Windows;

public sealed class WindowsInputDriver : IInputDriver<WinInputDriverConfig> {
    
    private static readonly NKLogger LOGGER = NKDebug.GetLogger(nameof(WindowsInputDriver));

    public event MouseEventHandler?    Mouse    = (_) => { };
    public event KeyEventHandler?      Key      = (_) => { };
    public event FocusInEventHandler?  FocusIn  = ( ) => { };
    public event FocusOutEventHandler? FocusOut = ( ) => { };
    public event ResizeEventHandler?   Resize   = (_) => { };
    public event PasteEventHandler?    Paste    = (_) => { };

    public event VTQueryResponseHandler? VTQuery;
    
    private          bool                 _disposed     = false;
    private          Thread?              _inputThread;
    private          WinInputDriverConfig _config;
    private          Size2D               _lastSize     = new();
    private          bool                 _isStopped    = true;
    private event    Action               _onStopped    = delegate { };
    private readonly WindowsInput         _input;
    private readonly WindowsAnsiParser    _parser;
    private readonly LockObject           _queueLock    = new();
    private readonly Queue<VTQuery>       _requestQueue = new();

    public bool IsRunning { get; private set; }
    
    public WinInputDriverConfig Config {
        get => _config; 
        set => _config = value;
    }

    public WindowsInputDriver(WinInputDriverConfig? config = null) {
        _config = config ?? new WinInputDriverConfig();
        _input  = new WindowsInput();
        _parser = new WindowsAnsiParser(_input, Decode, _config.VTRequestTimeout, _config.RefreshInterval);

        // enable ctrl+c force quit
        if (_config.CtrlCForceQuits) {
            Key += a => {
                if (a is { Down: true, Key: KeyCode.C } && a.Modifiers.GetHasCtrl())
                    Process.GetCurrentProcess().Kill();
            };
        }
    }

    public void RequestVTQuery(VTQuery request) {
        lock (_queueLock) {
            _requestQueue.Enqueue(request);
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
        _isStopped = false;
        
        while (IsRunning) {
            try {
                stopwatch.Restart();
                
                if (_input.TryGetInputs(out var records)) {
                    foreach (var r in records) {
                        Decode(r);
                    }
                }
                
                // wait with processing ANSI requests until there is no input
                if (_input.HasInput()) 
                    continue;
                
                ProcessRequests();

                var elapsed = stopwatch.Elapsed;

                if (elapsed <= _config.RefreshInterval && !_input.HasInput()) {
                    Thread.Sleep(_config.RefreshInterval - elapsed);
                }
            }
            catch (Exception e) {
                LOGGER.Error($"Input interceptor error: {e.Message}");
                Thread.Sleep(_config.RefreshInterval);
            }
        }

        _isStopped = true;
        _onStopped.Invoke();
    }

    private void ProcessRequests() {
        Queue<VTQuery>? currentRequests = null;
        
        // copy the queue
        lock (_queueLock) {
            if (_requestQueue.Count != 0) {
                currentRequests = new Queue<VTQuery>(_requestQueue);
                _requestQueue.Clear();
            }
        }
        
        // no requests
        if (currentRequests == null) 
            return;
        
        var failed = new List<VTQuery>();
                    
        // process individual requests
        foreach (var request in currentRequests) {
            NKConsole.Write(request.GetEscSeq());
            
            var parserResult = _parser.Parse(in request, out var response);
                        
            LOGGER.Debug(parserResult);
            
            // determine if the request should be repeated
            if (parserResult == AnsiParser.ParserResult.SUCCESS)
                VTQuery?.Invoke(response!.Value);
            else
                failed.Add(request);
        }
                    
        if (failed.Count == 0)
            return;
        
        // Enqueue back failed requests
        lock (_queueLock) {
            foreach (var f in failed) _requestQueue.Enqueue(f);
        }
    }

    private void Decode(WinImports.WinInputRecord record) {
        switch (record.Type) {
            case WinImports.WinEventType.KEY:    { InvokeKey   (record.Key   );   } break;
            case WinImports.WinEventType.MOUSE:  { InvokeMouse (record.Mouse );   } break;
            case WinImports.WinEventType.RESIZE: { InvokeResize(record.Resize);   } break;
            case WinImports.WinEventType.MENU:   { /* ignored, used internally */ } break;
            case WinImports.WinEventType.FOCUS:  { InvokeFocus (record.Focus );   } break;
            default: {
                LOGGER.Error("Unexpected WinEventType: " + record.Type);
                // throw new ArgumentOutOfRangeException(nameof(record.Type));
                return;
            }
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

    private void InvokeResize(WinImports.WinResizeEvent resizeEvent) {
        var s = new Size2D(resizeEvent.Size.X, resizeEvent.Size.Y);
        _lastSize = s;
        Resize?.Invoke(new ResizeEventArgs(s));
    }

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

    public Size2D GetSize() => _lastSize;

    public void Dispose() {
        if (_disposed)
            return;
        
        LOGGER.Info("Stopping Windows input interceptor...");
        
        Stop();
        
        if (!_isStopped) {
            var are = new AutoResetEvent(false);
            var handler = void () => are.Set();

            _onStopped += handler;
            are.WaitOne();
            _onStopped -= handler;
        }
        
        _disposed = true;
    }
}