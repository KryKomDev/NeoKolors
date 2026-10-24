//
// NeoKolors
// Copyright (c) 2026 KryKom
//

#if NK_ENABLE_NATIVE_IO

using NeoKolors.Console.Ansi;

namespace NeoKolors.Console;

/// <summary>
/// Specialized ANSI parser for the native Linux input driver reading directly from the native stream.
/// </summary>
internal sealed class LinuxAnsiParser : AnsiParser {
    private readonly LinuxInput _input;
    private readonly Action<KeyEventArgs> _onKey;
    private readonly Queue<KeyEventArgs> _inputQueue = new();

    public LinuxAnsiParser(
        LinuxInput input,
        Action<KeyEventArgs> onKey,
        TimeSpan? timeout = null,
        TimeSpan? retryInterval = null
    ) : base(timeout, retryInterval) {
        _input = input;
        _onKey = onKey;
    }

    public override bool Peek() => _input.HasInput();

    public override KeyEventArgs Read() {
        var key = _input.ReadKey();
        _inputQueue.Enqueue(key);
        return key;
    }

    public override char ReadChar() {
        var key = _input.ReadKey();
        _inputQueue.Enqueue(key);
        return key.Char;
    }

    public override void ReleaseRead(bool success) {
        if (success) {
            _inputQueue.Clear();
            return;
        }

        while (_inputQueue.Count > 0) {
            var k = _inputQueue.Dequeue();
            _onKey(k);
        }
    }
}

#endif
