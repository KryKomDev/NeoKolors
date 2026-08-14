// NeoKolors
// Copyright (c) 2025 KryKom

using System.ComponentModel;
using Metriks;
using NeoKolors.Console.Ansi;
using static NeoKolors.Common.EscapeCodes;

namespace NeoKolors.Console;

/// <summary>
/// Represents the .NET input driver implementation for handling console input events.
/// </summary>
/// <remarks>
/// This class is responsible for managing and processing console input events such as keyboard input,
/// mouse events, focus events, paste events, and terminal queries. It uses pure ANSI parsing for decoding
/// and interpreting input sequences. This driver is designed to work in conjunction with the NeoKolors
/// console framework.
/// </remarks>
public class DotnetInputDriver : IInputDriver<DotnetInputDriverConfig> {

    private static readonly NKLogger LOGGER = NKDebug.GetLogger("DotnetInputDriver");

    public event MouseEventHandler?      Mouse    = delegate { };
    public event KeyEventHandler?        Key      = delegate { };
    public event FocusInEventHandler?    FocusIn  = delegate { };
    public event FocusOutEventHandler?   FocusOut = delegate { };
    public event ResizeEventHandler?     Resize   = delegate { };
    public event PasteEventHandler?      Paste    = delegate { };
    public event VTQueryResponseHandler? VTQuery  = delegate { };

    private bool                             _disposed = false;
    private Thread?                          _inputThread;
    private bool                             _isStopped = true;
    private event Action                     _onStopped = delegate { };
    private readonly DotnetAnsiParser        _parser;
    private readonly LockObject              _queueLock    = new();
    private readonly Queue<VTQuery>          _requestQueue = new();
    private          DotnetInputDriverConfig _config;

    public bool IsRunning { get; private set; }

    public DotnetInputDriverConfig Config {
        get => _config;
        set {
            _config.PropertyChanged -= HandleConfigChange;

            _config = value;

            value.PropertyChanged += HandleConfigChange;
        }
    }

    private void HandleConfigChange(object? sender, PropertyChangedEventArgs e) {
        LOGGER.Debug($"Config change: {e.PropertyName}");
        
        switch (e.PropertyName) {
            case nameof(Config.MouseReportLevel) when e is IPropertyChangedEventTrackingArgs<ReportedMouseEvents> l: {
                SetReportedMouseEvents(l.OldValue, l.NewValue);

                break;
            }
            case nameof(Config.MouseReportLevel): {
                SetReportedMouseEvents(_config.MouseReportLevel);

                break;
            }
            case nameof(Config.MouseReportProtocol) when e is IPropertyChangedEventTrackingArgs<MouseReportProtocol> p: {
                SetMouseReportProtocol(p.OldValue, p.NewValue);

                break;
            }
            case nameof(Config.MouseReportProtocol): {
                SetMouseReportProtocol(_config.MouseReportProtocol);

                break;
            }
            default: {
                LOGGER.Warn($"Unhandled config change: {e.PropertyName}");

                break;
            }
        }
    }

    public DotnetInputDriver(DotnetInputDriverConfig? config = null) {
        _parser = new DotnetAnsiParser(HandleUnused);
        _config = config ?? new DotnetInputDriverConfig();

        try {
            Stdio.TreatControlCAsInput = !Config.CtrlCForceQuits;
        }
        catch (Exception ex) {
            LOGGER.Warn($"Failed to set TreatControlCAsInput: {ex.Message}");
        }
    }

    private void HandleUnused(ConsoleKeyInfo[] keys) {
        if (!IsRunning)
            return;

        foreach (var k in keys) {
            Key?.Invoke(new KeyEventArgs(k));
        }
    }

    public void RequestVTQuery(VTQuery request) {
        lock (_queueLock) {
            _requestQueue.Enqueue(request);
        }
    }

    public virtual void Start() {
        if (IsRunning)
            return;

        IsRunning = true;

        _inputThread = new Thread(Intercept) {
            IsBackground = true,
            Priority     = ThreadPriority.BelowNormal,
            Name         = "NeoKolors .NET Input Interceptor"
        };

        _inputThread.Start();
    }

    public virtual void Stop() => IsRunning = false;

    private void Intercept() {
        _isStopped = false;

        while (IsRunning) {
            // try to get some input
            try {
                if (Stdio.KeyAvailable) {
                    ProcessInput(_parser.Parse());
                }
            }
            catch (Exception e) when (e is InvalidOperationException or EndOfStreamException) {
                break;
            }
            catch (Exception e) {
                LOGGER.Error($"Input error: {e.Message}");
            }

            ProcessRequests();
        }

        _isStopped = true;
        _onStopped.Invoke();
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
            }

                break;
            default: throw new ArgumentOutOfRangeException();
        }
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
            foreach (var f in failed)
                _requestQueue.Enqueue(f);
        }
    }

    public Size2D GetSize() {
        try {
            return new Size2D(Stdio.BufferWidth, Stdio.BufferHeight);
        }
        catch (Exception) {
            return new Size2D(80, 25);
        }
    }

    public virtual void Dispose() {
        if (_disposed)
            return;

        LOGGER.Info("Stopping Dotnet input interceptor...");

        Stop();

        if (!_isStopped) {
            var are     = new AutoResetEvent(false);
            var handler = void () => are.Set();

            _onStopped += handler;
            are.WaitOne();
            _onStopped -= handler;
        }

        _disposed = true;
    }

    private static void SetMouseReportProtocol(MouseReportProtocol newValue) {
        SetMouseReportProtocol(MouseReportProtocol.X10, newValue);
    }

    private static void SetMouseReportProtocol(MouseReportProtocol oldValue, MouseReportProtocol newValue) {
        var d = oldValue switch {
            MouseReportProtocol.X10        => null,
            MouseReportProtocol.UTF8       => MOUSE_EV_UTF8_OFF,
            MouseReportProtocol.SGR        => MOUSE_EV_SGR_OFF,
            MouseReportProtocol.SGR_PIXELS => MOUSE_EV_SGR_PIXELS_OFF,
            _                              => throw new ArgumentOutOfRangeException(nameof(oldValue), oldValue, null),
        };

        var e = newValue switch {
            MouseReportProtocol.X10        => null,
            MouseReportProtocol.UTF8       => MOUSE_EV_UTF8_ON,
            MouseReportProtocol.SGR        => MOUSE_EV_SGR_ON,
            MouseReportProtocol.SGR_PIXELS => MOUSE_EV_SGR_PIXELS_ON,
            _                              => throw new ArgumentOutOfRangeException(nameof(newValue), newValue, null),
        };

        // disable the original protocol
        if (d != null)
            NKConsole.OutputDriver.Write(d);

        // enable the new protocol
        if (e != null)
            NKConsole.OutputDriver.Write(e);

        LOGGER.Info($"Mouse reporting protocol set to {newValue}");
    }

    private static void SetReportedMouseEvents(ReportedMouseEvents newValue) {
        SetReportedMouseEvents(ReportedMouseEvents.NONE, newValue);
    }

    private static void SetReportedMouseEvents(ReportedMouseEvents oldValue, ReportedMouseEvents newValue) {
        var d = oldValue switch {
            ReportedMouseEvents.NONE       => null,
            <= ReportedMouseEvents.DOWN    => MOUSE_EV_ON_P_OFF,
            <= ReportedMouseEvents.RELEASE => MOUSE_EV_ON_PR_OFF,
            <= ReportedMouseEvents.DRAG    => MOUSE_EV_ON_PRD_OFF,
            <= ReportedMouseEvents.ALL     => MOUSE_EV_ON_ALL_OFF,
            _                              => throw new ArgumentOutOfRangeException(nameof(oldValue), oldValue, null),
        };

        var e = newValue switch {
            ReportedMouseEvents.NONE       => null,
            <= ReportedMouseEvents.DOWN    => MOUSE_EV_ON_P_ON,
            <= ReportedMouseEvents.RELEASE => MOUSE_EV_ON_PR_ON,
            <= ReportedMouseEvents.DRAG    => MOUSE_EV_ON_PRD_ON,
            <= ReportedMouseEvents.ALL     => MOUSE_EV_ON_ALL_ON,
            _                              => throw new ArgumentOutOfRangeException(nameof(newValue), newValue, null),
        };

        // disable the original level
        if (d != null)
            NKConsole.OutputDriver.Write(d);

        // enable the new level
        if (e != null)
            NKConsole.OutputDriver.Write(e);

        LOGGER.Info($"Mouse reporting level set to {newValue}.");
    }
}