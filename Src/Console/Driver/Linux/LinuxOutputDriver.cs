//
// NeoKolors
// Copyright (c) 2026 KryKom
//

#if NK_ENABLE_NATIVE_IO

namespace NeoKolors.Console;

/// <summary>
/// Native Linux output driver writing directly to STDOUT using POSIX libc write() syscalls
/// for maximum frame throughput and zero managed stream buffering overhead.
/// </summary>
public class LinuxOutputDriver : IOutputDriver {
    private bool _disposed;
    private readonly LockObject _lock = new();
    private readonly LinuxOutput _output;

    public LinuxOutputDriver() {
        _output = new LinuxOutput();
    }

    public void Write(ReadOnlySpan<char> value) {
        lock (_lock) {
            _output.Write(value);
        }
    }

    public void Dispose() {
        if (_disposed)
            return;

        _output.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}

#endif
