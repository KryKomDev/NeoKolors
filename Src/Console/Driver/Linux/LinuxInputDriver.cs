//
// NeoKolors
// Copyright (c) 2026 KryKom
//

#if NK_ENABLE_NATIVE_IO

using System.ComponentModel;
using System.Diagnostics;
#if NET6_0_OR_GREATER
using System.Runtime.InteropServices;
#endif
using Metriks;
using NeoKolors.Console.Ansi;
using static NeoKolors.Common.EscapeCodes;

namespace NeoKolors.Console;

/// <summary>
/// Native Linux input driver utilizing POSIX libc termios, poll(), and direct STDIN reading
/// for zero-syscall-overhead input multiplexing, real-time mouse tracking, and SIGWINCH resizing.
/// </summary>
public sealed class LinuxInputDriver : IInputDriver<LinuxInputDriverConfig> {
    private static readonly NKLogger LOGGER = NKDebug.GetLogger<LinuxInputDriver>();

    public event MouseEventHandler?    Mouse    = delegate { };
    public event KeyEventHandler?      Key      = delegate { };
    public event FocusInEventHandler?  FocusIn  = delegate { };
    public event FocusOutEventHandler? FocusOut = delegate { };
    public event ResizeEventHandler?   Resize   = delegate { };
    public event PasteEventHandler?    Paste    = delegate { };
    public event VTQueryResponseHandler? VTQuery = delegate { };

    private bool _disposed;
    private Thread? _inputThread;
    private bool _isStopped = true;
    private readonly LinuxTerminal _terminal;
    private readonly LinuxInput _input;
    private readonly LinuxAnsiParser _parser;
    private readonly LockObject _queueLock = new();
    private readonly Queue<VTQuery> _requestQueue = new();
    private LinuxInputDriverConfig _config = null!;
    private Size2D _lastSize;

    private ReportedMouseEvents _activeMouseLevel = ReportedMouseEvents.NONE;
    private MouseReportProtocol _activeMouseProtocol = MouseReportProtocol.X10;
    private bool _activeFocusReporting;
    private bool _activeBracketedPaste;

#if NET6_0_OR_GREATER
    private PosixSignalRegistration? _sigwinchRegistration;
#endif

    private event Action OnStopped = delegate { };

    public bool IsRunning { get; private set; }

    public LinuxInputDriverConfig Config {
        get => _config;
        set {
            if (_config != null)
                _config.PropertyChanged -= HandleConfigChange;

            _config = value;

            if (_config != null) {
                _config.PropertyChanged += HandleConfigChange;
                ApplyMouseReporting();
                ApplyFocusReporting();
                ApplyBracketedPaste();
            }
        }
    }

    public LinuxInputDriver(LinuxInputDriverConfig? config = null) {
        _config = config ?? new LinuxInputDriverConfig();
        _terminal = new LinuxTerminal(_config.CtrlCForceQuits);
        _input = new LinuxInput();
        _parser = new LinuxAnsiParser(_input, k => Key?.Invoke(k), _config.VTRequestTimeout, _config.RefreshInterval);

        _config.PropertyChanged += HandleConfigChange;

        if (!LinuxTerminal.TryGetWindowSize(out _lastSize)) {
            _lastSize = new Size2D(80, 25);
        }

        if (_config.CtrlCForceQuits) {
            Key += a => {
                if (a is { Down: true, Key: KeyCode.C } && a.Modifiers.GetHasCtrl()) {
                    LinuxTerminal.RestoreGlobalMode();
                    Process.GetCurrentProcess().Kill();
                }
            };
        }
    }

    /// <summary>
    /// Backwards compatibility constructor supporting <see cref="DotnetInputDriverConfig"/>.
    /// </summary>
    public LinuxInputDriver(DotnetInputDriverConfig? config) : this(
        config != null
            ? new LinuxInputDriverConfig(
                config.ReportFocus,
                config.ReportResize,
                config.BracketedPaste,
                config.MouseReportLevel,
                config.MouseReportProtocol,
                config.CtrlCForceQuits
            )
            : null
    ) { }

    private void HandleConfigChange(object? sender, PropertyChangedEventArgs e) {
        switch (e.PropertyName) {
            case nameof(Config.MouseReportLevel):
            case nameof(Config.MouseReportProtocol):
                ApplyMouseReporting();
                break;
            case nameof(Config.ReportFocus):
                ApplyFocusReporting();
                break;
            case nameof(Config.BracketedPaste):
                ApplyBracketedPaste();
                break;
        }
    }

    private void ApplyMouseReporting() {
        var targetLevel = _config.MouseReportLevel;
        var targetProtocol = targetLevel != ReportedMouseEvents.NONE
            ? _config.MouseReportProtocol
            : MouseReportProtocol.X10;

        if (_activeMouseLevel != targetLevel) {
            SetReportedMouseEvents(_activeMouseLevel, targetLevel);
            _activeMouseLevel = targetLevel;
        }

        if (_activeMouseProtocol != targetProtocol) {
            SetMouseReportProtocol(_activeMouseProtocol, targetProtocol);
            _activeMouseProtocol = targetProtocol;
        }
    }

    private void ResetMouseReporting() {
        if (_activeMouseLevel != ReportedMouseEvents.NONE) {
            SetReportedMouseEvents(_activeMouseLevel, ReportedMouseEvents.NONE);
            _activeMouseLevel = ReportedMouseEvents.NONE;
        }

        if (_activeMouseProtocol != MouseReportProtocol.X10) {
            SetMouseReportProtocol(_activeMouseProtocol, MouseReportProtocol.X10);
            _activeMouseProtocol = MouseReportProtocol.X10;
        }
    }

    private void ApplyFocusReporting() {
        if (_config.ReportFocus && !_activeFocusReporting) {
            WriteTerminal("\e[?1004h");
            _activeFocusReporting = true;
        }
        else if (!_config.ReportFocus && _activeFocusReporting) {
            WriteTerminal("\e[?1004l");
            _activeFocusReporting = false;
        }
    }

    private void ApplyBracketedPaste() {
        if (_config.BracketedPaste && !_activeBracketedPaste) {
            WriteTerminal("\e[?2004h");
            _activeBracketedPaste = true;
        }
        else if (!_config.BracketedPaste && _activeBracketedPaste) {
            WriteTerminal("\e[?2004l");
            _activeBracketedPaste = false;
        }
    }

    private static void SetReportedMouseEvents(ReportedMouseEvents oldValue, ReportedMouseEvents newValue) {
        var d = oldValue switch {
            ReportedMouseEvents.NONE       => null,
            <= ReportedMouseEvents.DOWN    => MOUSE_EV_ON_P_OFF,
            <= ReportedMouseEvents.RELEASE => MOUSE_EV_ON_PR_OFF,
            <= ReportedMouseEvents.DRAG    => MOUSE_EV_ON_PRD_OFF,
            <= ReportedMouseEvents.ALL     => MOUSE_EV_ON_ALL_OFF,
            _                              => null,
        };

        var e = newValue switch {
            ReportedMouseEvents.NONE       => null,
            <= ReportedMouseEvents.DOWN    => MOUSE_EV_ON_P_ON,
            <= ReportedMouseEvents.RELEASE => MOUSE_EV_ON_PR_ON,
            <= ReportedMouseEvents.DRAG    => MOUSE_EV_ON_PRD_ON,
            <= ReportedMouseEvents.ALL     => MOUSE_EV_ON_ALL_ON,
            _                              => null,
        };

        if (d != null) WriteTerminal(d);
        if (e != null) WriteTerminal(e);
    }

    private static void SetMouseReportProtocol(MouseReportProtocol oldValue, MouseReportProtocol newValue) {
        var d = oldValue switch {
            MouseReportProtocol.X10        => null,
            MouseReportProtocol.UTF8       => MOUSE_EV_UTF8_OFF,
            MouseReportProtocol.SGR        => MOUSE_EV_SGR_OFF,
            MouseReportProtocol.SGR_PIXELS => MOUSE_EV_SGR_PIXELS_OFF,
            _                              => null,
        };

        var e = newValue switch {
            MouseReportProtocol.X10        => null,
            MouseReportProtocol.UTF8       => MOUSE_EV_UTF8_ON,
            MouseReportProtocol.SGR        => MOUSE_EV_SGR_ON,
            MouseReportProtocol.SGR_PIXELS => MOUSE_EV_SGR_PIXELS_ON,
            _                              => null,
        };

        if (d != null) WriteTerminal(d);
        if (e != null) WriteTerminal(e);
    }

    private static void WriteTerminal(string seq) {
        try {
            NKConsole.OutputDriver.Write(seq);
        }
        catch (Exception ex) {
            LOGGER.Warn($"Failed to write terminal escape sequence: {ex.Message}");
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

        LinuxTerminal.EnableRawMode(_config.CtrlCForceQuits);
        IsRunning = true;

        ApplyMouseReporting();
        ApplyFocusReporting();
        ApplyBracketedPaste();

#if NET6_0_OR_GREATER
        try {
#pragma warning disable CA1416
            _sigwinchRegistration = PosixSignalRegistration.Create(PosixSignal.SIGWINCH, _ => OnWindowResized());
#pragma warning restore CA1416
        }
        catch (Exception ex) {
            LOGGER.Warn($"Failed to register SIGWINCH listener: {ex.Message}");
        }
#endif

        _inputThread = new Thread(Intercept) {
            IsBackground = true,
            Priority = ThreadPriority.BelowNormal,
            Name = "NeoKolors Native Linux Input Interceptor"
        };

        _inputThread.Start();
    }

    public void Stop() {
        if (!IsRunning)
            return;

        IsRunning = false;
        _input.WakeUp();

        ResetMouseReporting();

        if (_activeFocusReporting) {
            WriteTerminal("\e[?1004l");
            _activeFocusReporting = false;
        }

        if (_activeBracketedPaste) {
            WriteTerminal("\e[?2004l");
            _activeBracketedPaste = false;
        }

#if NET6_0_OR_GREATER
        _sigwinchRegistration?.Dispose();
        _sigwinchRegistration = null;
#endif

        LinuxTerminal.RestoreGlobalMode();
    }

    private void Intercept() {
        _isStopped = false;

        while (IsRunning) {
            try {
                if (_input.PollAndBuffer(timeoutMs: 20)) {
                    while (IsRunning && _parser.Peek()) {
                        var record = _parser.Parse();
                        ProcessInput(record);
                    }
                }

                if (_config.ReportResize) {
                    CheckWindowResized();
                }

                ProcessRequests();
            }
            catch (Exception e) when (e is InvalidOperationException or EndOfStreamException) {
                break;
            }
            catch (Exception e) {
                LOGGER.Error($"Linux input interceptor error: {e}");
            }
        }

        _isStopped = true;
        OnStopped.Invoke();
    }

    private void CheckWindowResized() {
        if (LinuxTerminal.TryGetWindowSize(out var newSize)) {
            if (newSize != _lastSize) {
                _lastSize = newSize;
                Resize?.Invoke(new ResizeEventArgs(newSize));
            }
        }
    }

    private void OnWindowResized() {
        if (_config.ReportResize && LinuxTerminal.TryGetWindowSize(out var newSize)) {
            if (newSize != _lastSize) {
                _lastSize = newSize;
                Resize?.Invoke(new ResizeEventArgs(newSize));
            }
        }
    }

    private void ProcessInput(AnsiRecord? record) {
        if (record == null)
            return;

        var r = record.Value;
        switch (r.Type) {
            case AnsiRecordType.NONE:     break;
            case AnsiRecordType.KEY:      Key?.Invoke(r.Key); break;
            case AnsiRecordType.MOUSE:    Mouse?.Invoke(r.Mouse); break;
            case AnsiRecordType.PASTE:    Paste?.Invoke(r.Pasted); break;
            case AnsiRecordType.VT_QUERY: VTQuery?.Invoke(r.Query); break;
            case AnsiRecordType.FOCUS: {
                if (r.HasFocus)
                    FocusIn?.Invoke();
                else
                    FocusOut?.Invoke();
                break;
            }
            default: break;
        }
    }

    private void ProcessRequests() {
        Queue<VTQuery>? currentRequests = null;

        lock (_queueLock) {
            if (_requestQueue.Count != 0) {
                currentRequests = new Queue<VTQuery>(_requestQueue);
                _requestQueue.Clear();
            }
        }

        if (currentRequests == null)
            return;

        var failed = new List<VTQuery>();

        foreach (var request in currentRequests) {
            NKConsole.Write(request.GetEscSeq());
            var parserResult = _parser.Parse(in request, out var response);

            LOGGER.Debug(parserResult);

            if (parserResult == AnsiParser.ParserResult.SUCCESS)
                VTQuery?.Invoke(response!.Value);
            else
                failed.Add(request);
        }

        if (failed.Count == 0)
            return;

        lock (_queueLock) {
            foreach (var f in failed)
                _requestQueue.Enqueue(f);
        }
    }

    public Size2D GetSize() {
        if (LinuxTerminal.TryGetWindowSize(out var size)) {
            _lastSize = size;
            return size;
        }
        return _lastSize;
    }

    public void Dispose() {
        if (_disposed)
            return;

        Stop();

        if (!_isStopped) {
            var waitStop = new ManualResetEventSlim(false);
            Action handler = () => waitStop.Set();
            OnStopped += handler;

            _input.WakeUp();
            waitStop.Wait(TimeSpan.FromMilliseconds(200));
            OnStopped -= handler;
        }

        _input.Dispose();
        _terminal.Dispose();

        _disposed = true;
        GC.SuppressFinalize(this);
    }
}

#endif
