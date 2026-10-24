//
// NeoKolors
// Copyright (c) 2026 KryKom
//

#if NK_ENABLE_NATIVE_IO

using System.Buffers;
using System.Runtime.InteropServices;
using System.Text;

namespace NeoKolors.Console;

/// <summary>
/// Low-level output handler for Linux writing UTF-8 encoded spans directly to standard output
/// via native libc write() syscalls with zero heap allocation overhead.
/// </summary>
public sealed class LinuxOutput : IDisposable {
    private readonly int _fd;
    private bool _disposed;

    public LinuxOutput(int fd = LinuxImports.STDOUT_FILENO) {
        _fd = fd;
    }

    /// <summary>
    /// Writes the specified span of characters directly to the file descriptor as UTF-8 bytes.
    /// </summary>
    public unsafe void Write(ReadOnlySpan<char> text) {
        if (text.IsEmpty || _disposed)
            return;

        int maxByteCount = Encoding.UTF8.GetMaxByteCount(text.Length);
        byte[]? rented = maxByteCount > 2048 ? ArrayPool<byte>.Shared.Rent(maxByteCount) : null;
        Span<byte> byteBuffer = rented != null ? rented : stackalloc byte[maxByteCount];

        int writtenBytes = Encoding.UTF8.GetBytes(text, byteBuffer);
        fixed (byte* p = byteBuffer) {
            nint totalWritten = 0;
            while (totalWritten < writtenBytes) {
                nint ret = LinuxImports.write(_fd, p + totalWritten, (nuint)(writtenBytes - totalWritten));
                if (ret <= 0) {
                    var err = Marshal.GetLastWin32Error();
                    if (err == 4) // EINTR
                        continue;
                    throw new IOException($"write() to fd {_fd} failed with error code {err}.");
                }
                totalWritten += ret;
            }
        }

        if (rented != null)
            ArrayPool<byte>.Shared.Return(rented);
    }

    public void Dispose() {
        _disposed = true;
    }
}

#endif
