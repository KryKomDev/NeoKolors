//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using NeoKolors.Console.Ansi;
using NeoKolors.Console.Input;

namespace NeoKolors.Console.Driver.Dotnet;

/// <summary>
/// A parser that extends <see cref="AnsiParser"/> to handle ANSI escape code
/// parsing specific to .NET input handling.
/// </summary>
/// <remarks>
/// The <c>DotnetAnsiParser</c> provides customized functionality for processing
/// VT input sequences in a .NET environment. It supports reading input from
/// the console, peeking for input availability, and managing unused input
/// based on success or failure scenarios. The unused input is processed
/// through an action delegate passed during instantiation.
/// </remarks>
public class DotnetAnsiParser : AnsiParser {
    
    // private          bool                     _useReadKey = true;
    private readonly Action<ConsoleKeyInfo[]> _handleUnused;
    private readonly Queue<ConsoleKeyInfo>    _inputQueue = new();

    public DotnetAnsiParser(Action<ConsoleKeyInfo[]> handleUnused) {
        _handleUnused = handleUnused;
    }
    
    public override bool Peek() => Stdio.PeekU();

    public override KeyEventArgs Read() {
        var key = Stdio.ReadKeyU(intercept: true);

        if (key == null)
            throw new InvalidOperationException();
        
        _inputQueue.Enqueue(key.Value);

        return new KeyEventArgs(key.Value);
    }

    public override char ReadChar() {
        var key = Stdio.ReadKeyU(intercept: true);

        if (key == null)
            throw new InvalidOperationException();
        
        _inputQueue.Enqueue(key.Value);
        
        return key.Value.KeyChar;
    }

    public override void ReleaseRead(bool success) {
        if (success)
            _inputQueue.Clear();
        else
            _handleUnused(_inputQueue.ToArray());
    }
}