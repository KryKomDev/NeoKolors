//
// NeoKolors
// Copyright (c) 2026 KryKom
//

#if NK_ENABLE_NATIVE_IO

using System.Text;

namespace NeoKolors.Console;

/// <summary>
/// Manages native low-level input streaming from STDIN using POSIX poll() multiplexing
/// and self-pipe signaling for instantaneous interruption.
/// </summary>
public sealed class LinuxInput : IDisposable {
    private readonly int _stdinFd;
    private LinuxImports.PipeFds _pipeFds;
    private readonly LockObject _queueLock = new();
    private readonly Queue<char> _charQueue = new();
    private readonly Decoder _decoder = Encoding.UTF8.GetDecoder();
    private readonly byte[] _byteBuffer = new byte[4096];
    private readonly char[] _charBuffer = new char[4096];

    private bool _disposed;
    private bool _eof;

    public LinuxInput(int stdinFd = LinuxImports.STDIN_FILENO) {
        _stdinFd = stdinFd;

        if (LinuxImports.pipe(out _pipeFds) != 0) {
            throw new IOException("Failed to create self-pipe for Linux input driver.");
        }

        // Set non-blocking on pipe descriptors to prevent write/read stalls
        LinuxImports.fcntl(_pipeFds.ReadFd, LinuxImports.F_SETFL, LinuxImports.O_NONBLOCK);
        LinuxImports.fcntl(_pipeFds.WriteFd, LinuxImports.F_SETFL, LinuxImports.O_NONBLOCK);
    }

    /// <summary>
    /// Signals the polling loop to interrupt immediately.
    /// </summary>
    public unsafe void WakeUp() {
        if (_disposed)
            return;

        byte b = 1;
        LinuxImports.write(_pipeFds.WriteFd, &b, 1);
    }

    /// <summary>
    /// Checks whether character input is available without blocking.
    /// </summary>
    public bool HasInput() {
        lock (_queueLock) {
            if (_charQueue.Count > 0)
                return true;
        }

        return PollAndBuffer(timeoutMs: 0);
    }

    /// <summary>
    /// Polls the input stream and decodes any available bytes into characters.
    /// </summary>
    /// <param name="timeoutMs">Timeout in milliseconds to wait for input.</param>
    /// <returns><c>true</c> if characters are ready to read; otherwise, <c>false</c>.</returns>
    public unsafe bool PollAndBuffer(int timeoutMs) {
        lock (_queueLock) {
            if (_charQueue.Count > 0)
                return true;
        }

        if (_disposed || _eof)
            return false;

        LinuxImports.PollFd* fds = stackalloc LinuxImports.PollFd[2];
        fds[0].fd = _stdinFd;
        fds[0].events = LinuxImports.POLLIN;
        fds[0].revents = 0;

        fds[1].fd = _pipeFds.ReadFd;
        fds[1].events = LinuxImports.POLLIN;
        fds[1].revents = 0;

        int ret = LinuxImports.poll(fds, 2, timeoutMs);

        if (ret <= 0)
            return false;

        // Drain wake-up self-pipe if signaled
        if ((fds[1].revents & LinuxImports.POLLIN) != 0) {
            byte dummy;
            while (LinuxImports.read(_pipeFds.ReadFd, &dummy, 1) > 0) { }
        }

        // Read and decode bytes from stdin
        if ((fds[0].revents & LinuxImports.POLLIN) != 0) {
            fixed (byte* pBytes = _byteBuffer) {
                nint bytesRead = LinuxImports.read(_stdinFd, pBytes, (nuint)_byteBuffer.Length);

                if (bytesRead > 0) {
                    fixed (char* pChars = _charBuffer) {
                        int charsDecoded = _decoder.GetChars(pBytes, (int)bytesRead, pChars, _charBuffer.Length, flush: false);
                        lock (_queueLock) {
                            for (int i = 0; i < charsDecoded; i++) {
                                _charQueue.Enqueue(pChars[i]);
                            }
                        }
                    }
                }
                else if (bytesRead == 0) {
                    _eof = true;
                }
            }
        }

        lock (_queueLock) {
            return _charQueue.Count > 0;
        }
    }

    /// <summary>
    /// Reads the next available character from the stream, waiting if necessary.
    /// </summary>
    public KeyEventArgs ReadKey(int timeoutMs = -1) {
        while (!_disposed && !_eof) {
            lock (_queueLock) {
                if (_charQueue.Count > 0) {
                    char c = _charQueue.Dequeue();
                    return CharToKeyEventArgs(c);
                }
            }

            if (!PollAndBuffer(timeoutMs < 0 ? 50 : timeoutMs)) {
                if (timeoutMs >= 0)
                    throw new TimeoutException("Timed out waiting for character input.");
            }
        }

        if (_eof)
            throw new EndOfStreamException();

        throw new ObjectDisposedException(nameof(LinuxInput));
    }

    internal static KeyEventArgs CharToKeyEventArgs(char c) {
        if (c == '\e')
            return new KeyEventArgs(KeyCode.ESCAPE, KeyModifiers.NONE, '\e');
        if (c is '\r' or '\n')
            return new KeyEventArgs(KeyCode.RETURN, KeyModifiers.NONE, '\r');
        if (c == '\t')
            return new KeyEventArgs(KeyCode.TAB, KeyModifiers.NONE, '\t');
        if (c is '\b' or (char)127)
            return new KeyEventArgs(KeyCode.BACKSPACE, KeyModifiers.NONE, '\b');
        if (c == ' ')
            return new KeyEventArgs(KeyCode.SPACE, KeyModifiers.NONE, ' ');

        if (c is >= (char)1 and <= (char)26) {
            var key = (KeyCode)((int)KeyCode.A + (c - 1));
            return new KeyEventArgs(key, KeyModifiers.LEFT_CTRL, (char)('a' + c - 1));
        }

        if (c is >= 'a' and <= 'z') {
            var key = (KeyCode)((int)KeyCode.A + (c - 'a'));
            return new KeyEventArgs(key, KeyModifiers.NONE, c);
        }

        if (c is >= 'A' and <= 'Z') {
            var key = (KeyCode)((int)KeyCode.A + (c - 'A'));
            return new KeyEventArgs(key, KeyModifiers.SHIFT, c);
        }

        if (c is >= '0' and <= '9') {
            var key = (KeyCode)((int)KeyCode.D0 + (c - '0'));
            return new KeyEventArgs(key, KeyModifiers.NONE, c);
        }

        return new KeyEventArgs(KeyCode.NONE, KeyModifiers.NONE, c);
    }

    public void Dispose() {
        if (_disposed)
            return;

        _disposed = true;
        WakeUp();

        if (_pipeFds.ReadFd > 0)
            LinuxImports.close(_pipeFds.ReadFd);
        if (_pipeFds.WriteFd > 0)
            LinuxImports.close(_pipeFds.WriteFd);
    }
}

#endif
