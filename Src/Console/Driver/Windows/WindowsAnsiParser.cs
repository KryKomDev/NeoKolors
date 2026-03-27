//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using NeoKolors.Console.Ansi;
using NeoKolors.Console.Input;

namespace NeoKolors.Console.Driver.Windows;

/// <summary>
/// A specialized ANSI escape code parser for Windows console input.
/// </summary>
/// <remarks>
/// The WindowsAnsiParser is designed to process Windows-specific console input and parse ANSI escape sequences.
/// It extends the base <see cref="AnsiParser"/> functionality, using <see cref="WindowsInput"/>
/// to interact with the underlying input system.
/// </remarks>
internal class WindowsAnsiParser : AnsiParser {

    private readonly WindowsInput                      _input;
    private readonly Action<WinImports.WinInputRecord> _onOtherEvent;
    private readonly Queue<WinImports.WinInputRecord>  _inputQueue = new();
    
    public WindowsAnsiParser(
        WindowsInput                      input, 
        Action<WinImports.WinInputRecord> onOtherEvent,
        TimeSpan?                         timeout       = null, 
        TimeSpan?                         retryInterval = null
    ) : base(timeout, retryInterval)
    {
        _input        = input;
        _onOtherEvent = onOtherEvent;
    } 
    
    public override bool Peek() {
        return _input.HasInput();
    }

    public override KeyEventArgs Read() {
        while (true) {
            var record = _input.ReadInput();

            if (record is null)
                throw new InvalidOperationException("No records read from input.");

            var r = record.Value;
            
            if (r.Type == WinImports.WinEventType.KEY) {
                _inputQueue.Enqueue(r);
                return new KeyEventArgs(r.Key);
            }

            _onOtherEvent?.Invoke(r);
        }
    }

    public override char ReadChar() {
        while (true) {
            var record = _input.ReadInput();

            if (record is null)
                throw new InvalidOperationException("No records read from input.");

            var r = record.Value;
            
            if (r.Type == WinImports.WinEventType.KEY) {
                _inputQueue.Enqueue(r);
                return r.Key.Char;
            }

            _onOtherEvent?.Invoke(r);
        }
    }

    public override void ReleaseRead(bool success) {
        if (success) {
            _inputQueue.Clear();
            return;
        }

        foreach (var i in _inputQueue) {
            _onOtherEvent.Invoke(i);
        }
        
        _inputQueue.Clear();
    }
}