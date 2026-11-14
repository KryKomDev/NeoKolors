//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.Diagnostics;
using Metriks;
using NeoKolors.Console.Ansi;

namespace NeoKolors.Console;

public sealed class WindowsInputDriver : IInputDriver<WinInputDriverConfig> {

    private static readonly NKLogger LOGGER = NKDebug.GetLogger(nameof(WindowsInputDriver));

    public event MouseEventHandler?    Mouse    = (_) => { };
    public event KeyEventHandler?      Key      = (_) => { };
    public event FocusInEventHandler?  FocusIn  = () => { };
    public event FocusOutEventHandler? FocusOut = () => { };
    public event ResizeEventHandler?   Resize   = (_) => { };
    public event PasteEventHandler?    Paste    = (_) => { };

    public event VTQueryResponseHandler? VTQuery;
    public event VTQueryTimeoutHandler?  VTQueryTimeout;

    private readonly record struct QueuedVTRequest(VTQuery Query, int Attempts);

    private bool                            _disposed = false;
    private Thread?                         _inputThread;
    private WinInputDriverConfig            _config;
    private Size2D                          _lastSize  = new();
    private bool                            _isStopped = true;
    private event Action                    _onStopped = delegate { };
    private readonly WindowsInput           _input;
    private readonly WindowsAnsiParser      _parser;
    private readonly LockObject             _queueLock    = new();
    private readonly Queue<QueuedVTRequest> _requestQueue = new();

    public bool IsRunning { get; private set; }

    public WinInputDriverConfig Config {
        get => _config;
        set => _config = value;
    }

    public WindowsInputDriver(WinInputDriverConfig? config = null) {
        _config = config ?? new WinInputDriverConfig();
        _input  = new WindowsInput();
        _parser = new WindowsAnsiParser(_input, Decode, _config.VTRequestTimeout, _config.RefreshInterval);

        try {
            _lastSize = new Size2D(Stdio.BufferWidth, Stdio.BufferHeight);
        }
        catch {
            _lastSize = new Size2D(80, 25);
        }

        // enable ctrl+c force quit
        if (_config.CtrlCForceQuits)
            Key += a => {
                if (a is { Down: true, Key: KeyCode.C } && a.Modifiers.GetHasCtrl())
                    Process.GetCurrentProcess().Kill();
            };
    }

    public void RequestVTQuery(VTQuery request) {
        lock (_queueLock) {
            _requestQueue.Enqueue(new QueuedVTRequest(request, 0));
        }
    }

    public bool CancelVTQuery(VTQuery request) {
        lock (_queueLock) {
            var list = _requestQueue.ToList();
            var removed = list.RemoveAll(q => q.Query.Equals(request)) > 0;
            if (removed) {
                _requestQueue.Clear();
                foreach (var item in list)
                    _requestQueue.Enqueue(item);
            }

            return removed;
        }
    }

    public void Start() {
        if (IsRunning || _disposed)
            return;

        _input.Enable();

        IsRunning = true;

        _inputThread = new Thread(Intercept) {
            IsBackground = true,
            Priority     = ThreadPriority.BelowNormal,
            Name         = "NeoKolors Win32 Input Interceptor"
        };

        _inputThread.Start();
    }

    public void Stop() {
        _input.Disable();
        IsRunning = false;
    }

    private void Intercept() {
        var stopwatch = new Stopwatch();
        _isStopped = false;

        while (IsRunning)
            try {
                stopwatch.Restart();

                if (_input.TryGetInputs(out var records))
                    foreach (var r in records)
                        Decode(r);

                // wait with processing ANSI requests until there is no input
                if (_input.HasInput())
                    continue;

                ProcessRequests();

                var elapsed = stopwatch.Elapsed;

                if (elapsed <= _config.RefreshInterval && !_input.HasInput())
                    Thread.Sleep(_config.RefreshInterval - elapsed);
            }
            catch (Exception e) {
                LOGGER.Error($"Input interceptor error: \n{e}");
                Thread.Sleep(_config.RefreshInterval);
            }

        _isStopped = true;
        _onStopped.Invoke();
    }

    private void ProcessRequests() {
        Queue<QueuedVTRequest>? currentRequests = null;

        // copy the queue
        lock (_queueLock) {
            if (_requestQueue.Count != 0) {
                currentRequests = new Queue<QueuedVTRequest>(_requestQueue);
                _requestQueue.Clear();
            }
        }

        // no requests
        if (currentRequests == null)
            return;

        var retried = new List<QueuedVTRequest>();

        // process individual requests
        foreach (var item in currentRequests) {
            NKConsole.Write(item.Query.GetEscSeq());

            var query = item.Query;
            var parserResult = _parser.Parse(in query, out var response);

            LOGGER.Debug(parserResult);

            // determine if the request should be repeated
            if (parserResult == AnsiParser.ParserResult.SUCCESS) {
                VTQuery?.Invoke(response!.Value);
            }
            else if (item.Attempts < Config.VTRequestMaxRetries) {
                retried.Add(new QueuedVTRequest(item.Query, item.Attempts + 1));
            }
            else {
                LOGGER.Warn($"VTQuery {item.Query.Type} failed with {parserResult} after {item.Attempts + 1} attempts.");
                VTQueryTimeout?.Invoke(item.Query, parserResult);
            }
        }

        if (retried.Count == 0)
            return;

        // Enqueue back retried requests
        lock (_queueLock) {
            foreach (var r in retried)
                _requestQueue.Enqueue(r);
        }
    }

    private void Decode(WinImports.WinInputRecord record) {
        try {
            switch (record.Type) {
                case WinImports.WinEventType.KEY: {
                    InvokeKey(record.Key);
                }

                    return;
                case WinImports.WinEventType.MOUSE: {
                    InvokeMouse(record.Mouse);
                }

                    return;
                case WinImports.WinEventType.RESIZE: {
                    InvokeResize(record.Resize);
                }

                    return;
                case WinImports.WinEventType.MENU: {
                    /* ignored, used internally */
                }

                    return;
                case WinImports.WinEventType.FOCUS: {
                    InvokeFocus(record.Focus);
                }

                    return;
                default: {
                    LOGGER.Error("Unexpected WinEventType: " + record.Type);

                    return;
                }
            }
        }
        catch (Exception e) {
            LOGGER.Error("A VTInput listener threw an error: \n" + e);
        }
    }

    private void InvokeKey(WinImports.WinKeyEvent keyEvent) {
        Key?.Invoke(
            new KeyEventArgs(
                (KeyCode)keyEvent.VirtualKeyCode,
                (KeyModifiers)keyEvent.Modifiers,
                keyEvent.Char,
                keyEvent.Down
            )
        );
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

        Mouse?.Invoke(
            new MouseEventArgs(
                button,
                (KeyModifiers)mouseEvent.Modifiers,
                new Point2D(mouseEvent.Coord.X, mouseEvent.Coord.Y),
                mb == 0,
                mouseEvent.Flags.GetHasMoved()
            )
        );
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
            var are     = new AutoResetEvent(false);
            var handler = void () => are.Set();

            _onStopped += handler;
            are.WaitOne();
            _onStopped -= handler;
        }

        _input.Dispose();
        _disposed = true;
    }
}