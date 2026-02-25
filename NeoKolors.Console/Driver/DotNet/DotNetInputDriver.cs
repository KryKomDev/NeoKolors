// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Common;
using NeoKolors.Console.Ansi;
using NeoKolors.Console.Events;
using Std = System.Console;

namespace NeoKolors.Console.Driver.DotNet;

public class DotNetInputDriver : IInputDriver {
    private static readonly NKLogger LOGGER = NKDebug.GetLogger("DotNetInputDriver");
    
    public event MouseEventHandler? Mouse {
        add    => _parser.Mouse += value;          
        remove => _parser.Mouse -= value;
    }
    
    public event KeyEventHandler? Key {
        add    => _parser.Key += value;            
        remove => _parser.Key -= value;
    }
    
    public event FocusInEventHandler? FocusIn {
        add    => _parser.FocusIn += value;        
        remove => _parser.FocusIn -= value;
    }
    
    public event FocusOutEventHandler? FocusOut {
        add    => _parser.FocusOut += value;      
        remove => _parser.FocusOut -= value;
    }

    public event PasteEventHandler? Paste {
        add    => _parser.Paste += value;         
        remove => _parser.Paste -= value;
    }

    public event VTQueryResponseHandler? VTQuery {
        add    => _parser.VTQuery += value;
        remove => _parser.VTQuery -= value;
    }

    public event ResizeEventHandler? Resize;

    private          Thread?         _inputThread;
    private          bool            _useReadKey = true;
    private readonly AnsiInputParser _parser;

    public void RequestDecMode(EscapeCodes.DecMode mode) => Stdio.Write(EscapeCodes.GetDecReq(mode));

    public void RequestOscMode(EscapeCodes.OscMode mode) {
        Stdio.Write(EscapeCodes.GetOscReq(mode));
    }
    
    public bool IsRunning { get; private set; }

    public DotNetInputDriver() {
        _parser = new AnsiInputParser(ReadNext, Invoke);
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

    public virtual void Stop()    => IsRunning = false;
    public virtual void Dispose() => Stop();

    protected virtual void Intercept() {
        while (IsRunning) {
            try {
                var info = ReadNext();
                _parser.Parse(info);
            }
            catch (Exception e) when (e is InvalidOperationException or EndOfStreamException) {
                break;
            }
            catch (Exception e) {
                LOGGER.Error($"Input error: {e.Message}");
            }
        }
    }

    private ConsoleKeyInfo ReadNext() {
        if (_useReadKey) {
            try {
                return Std.ReadKey(true);
            }
            catch (InvalidOperationException) {
                _useReadKey = false;
            }
        }
        
        int c = Std.Read();
        
        return c == -1 
            ? throw new EndOfStreamException() 
            : new ConsoleKeyInfo((char)c, 0, false, false, false);
    }

    private void Invoke(Action action) {
        Task.Run(() => {
            if (IsRunning) action();
        });
    }
}
