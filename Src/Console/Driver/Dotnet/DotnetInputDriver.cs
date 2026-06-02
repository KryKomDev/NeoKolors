// NeoKolors
// Copyright (c) 2025 KryKom

using Metriks;
using NeoKolors.Console.Ansi;
using NeoKolors.Console.Events;
using NeoKolors.Console.Input;

namespace NeoKolors.Console.Driver.Dotnet;

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

    private          bool             _disposed     = false;
    private          Thread?          _inputThread;
    private          bool             _isStopped    = true;
    private event    Action           _onStopped    = delegate { };
    private readonly DotnetAnsiParser _parser;
    private readonly LockObject       _queueLock    = new();
    private readonly Queue<VTQuery>   _requestQueue = new();
    private DotnetInputDriverConfig   _config;

    public bool IsRunning { get; private set; }

    public DotnetInputDriverConfig Config {
        get => _config;
        set => _config = value;
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
        if (IsRunning) {
            foreach (var k in keys) {
                Key?.Invoke(new KeyEventArgs(k));
            }
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
        
        IsRunning    = true;
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
            case AnsiRecordType.NONE:                                break;
            case AnsiRecordType.KEY:      Key?    .Invoke(r.Key);    break;
            case AnsiRecordType.MOUSE:    Mouse?  .Invoke(r.Mouse);  break;
            case AnsiRecordType.PASTE:    Paste?  .Invoke(r.Pasted); break;
            case AnsiRecordType.VT_QUERY: VTQuery?.Invoke(r.Query);  break;
            case AnsiRecordType.FOCUS: {
                if (r.HasFocus) 
                    FocusIn?.Invoke();
                else 
                    FocusOut?.Invoke();
            } break;
            default:
                throw new ArgumentOutOfRangeException();
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
            foreach (var f in failed) _requestQueue.Enqueue(f);
        }
    }
    
    public Size2D GetSize() => new(Stdio.BufferWidth, Stdio.BufferHeight);

    public virtual void Dispose() {
        if (_disposed)
            return;
        
        LOGGER.Info("Stopping Dotnet input interceptor...");
        
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
