//
// NeoKolors
// Copyright (c) 2026 KryKom
//

#if NK_ENABLE_NATIVE_IO

using Metriks;

namespace NeoKolors.Console;

/// <summary>
/// Provides lifecycle management for the Linux terminal session, configuring raw mode
/// and guaranteeing proper terminal restoration upon process termination or signal delivery.
/// </summary>
public sealed class LinuxTerminal : IDisposable {
    private static readonly NKLogger LOGGER = NKDebug.GetLogger<LinuxTerminal>();

    private static readonly LockObject s_lock = new();
    private static LinuxImports.Termios s_originalTermios;
    private static bool s_hasOriginalTermios;
    private static bool s_isRawMode;

    private bool _disposed;
    private readonly bool _ctrlCForceQuits;

    static LinuxTerminal() {
        try {
            AppDomain.CurrentDomain.ProcessExit += (_, _) => RestoreGlobalMode();
            System.Console.CancelKeyPress += (_, _) => RestoreGlobalMode();
        }
        catch {
            // In constrained environments where signal hooks might not be permissible
        }
    }

    /// <summary>
    /// Gets a value indicating whether standard input is attached to a terminal (TTY).
    /// </summary>
    public static bool IsStdinATty => LinuxImports.isatty(LinuxImports.STDIN_FILENO) != 0;

    /// <summary>
    /// Gets a value indicating whether standard output is attached to a terminal (TTY).
    /// </summary>
    public static bool IsStdoutATty => LinuxImports.isatty(LinuxImports.STDOUT_FILENO) != 0;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinuxTerminal"/> class.
    /// Terminal mode changes are activated when <see cref="EnableRawMode"/> is called.
    /// </summary>
    /// <param name="ctrlCForceQuits">
    /// If <c>true</c>, pressing Ctrl+C will trigger the operating system SIGINT handler and terminate the process.
    /// If <c>false</c>, raw ISIG is disabled and Ctrl+C is delivered as a normal input byte (0x03).
    /// </param>
    public LinuxTerminal(bool ctrlCForceQuits = true) {
        _ctrlCForceQuits = ctrlCForceQuits;
        if (IsStdinATty) {
            EnsureCapturedOriginal();
        }
    }

    private static void EnsureCapturedOriginal() {
        lock (s_lock) {
            if (!s_hasOriginalTermios && IsStdinATty) {
                if (LinuxImports.tcgetattr(LinuxImports.STDIN_FILENO, out s_originalTermios) == 0) {
                    s_hasOriginalTermios = true;
                }
                else {
                    LOGGER.Warn("Failed to retrieve terminal attributes via tcgetattr.");
                }
            }
        }
    }

    /// <summary>
    /// Configures the terminal into non-canonical, raw mode.
    /// </summary>
    public static void EnableRawMode(bool ctrlCForceQuits = true) {
        lock (s_lock) {
            if (!IsStdinATty || s_isRawMode)
                return;

            EnsureCapturedOriginal();
            if (!s_hasOriginalTermios)
                return;

            var raw = s_originalTermios;

            // Input modes: disable break, CR translation, parity checking, 8-bit stripping, and software flow control
            raw.c_iflag &= ~(LinuxImports.IGNBRK | LinuxImports.BRKINT | LinuxImports.PARMRK |
                             LinuxImports.ISTRIP | LinuxImports.INLCR | LinuxImports.IGNCR |
                             LinuxImports.ICRNL | LinuxImports.IXON);

            // Output modes: keep OPOST intact for standard line-ending behaviors

            // Local modes: disable echo, canonical mode, and extended input processing
            raw.c_lflag &= ~(LinuxImports.ECHO | LinuxImports.ECHONL | LinuxImports.ICANON | LinuxImports.IEXTEN);

            // If Ctrl+C should not force quit, disable ISIG so byte 0x03 is passed to read()
            if (!ctrlCForceQuits) {
                raw.c_lflag &= ~LinuxImports.ISIG;
            }

            // Control characters: non-blocking read for poll() multiplexing
            unsafe {
                raw.c_cc[LinuxImports.VMIN]  = 0;
                raw.c_cc[LinuxImports.VTIME] = 0;
            }

            if (LinuxImports.tcsetattr(LinuxImports.STDIN_FILENO, LinuxImports.TCSAFLUSH, in raw) != 0) {
                LOGGER.Warn("Failed to activate raw mode via tcsetattr.");
                return;
            }

            s_isRawMode = true;
            LOGGER.Debug("Terminal raw mode successfully enabled.");
        }
    }

    /// <summary>
    /// Restores the terminal attributes to their original cooked mode.
    /// </summary>
    public static void RestoreGlobalMode() {
        lock (s_lock) {
            if (!s_isRawMode || !s_hasOriginalTermios || !IsStdinATty)
                return;

            LinuxImports.tcsetattr(LinuxImports.STDIN_FILENO, LinuxImports.TCSANOW, in s_originalTermios);
            s_isRawMode = false;
            LOGGER.Debug("Terminal original mode successfully restored.");
        }
    }

    /// <summary>
    /// Queries the terminal window dimensions in character cells.
    /// </summary>
    public static bool TryGetWindowSize(out Size2D size) {
        if (LinuxImports.ioctl(LinuxImports.STDOUT_FILENO, LinuxImports.TIOCGWINSZ, out var ws) == 0 &&
            ws.ws_col > 0 && ws.ws_row > 0) {
            size = new Size2D(ws.ws_col, ws.ws_row);
            return true;
        }

        if (LinuxImports.ioctl(LinuxImports.STDIN_FILENO, LinuxImports.TIOCGWINSZ, out ws) == 0 &&
            ws.ws_col > 0 && ws.ws_row > 0) {
            size = new Size2D(ws.ws_col, ws.ws_row);
            return true;
        }

        size = default;
        return false;
    }

    /// <summary>
    /// Queries the terminal window dimensions in physical pixels.
    /// </summary>
    public static bool TryGetWindowPixelSize(out Size2D pixelSize) {
        if (LinuxImports.ioctl(LinuxImports.STDOUT_FILENO, LinuxImports.TIOCGWINSZ, out var ws) == 0 &&
            (ws.ws_xpixel > 0 || ws.ws_ypixel > 0)) {
            pixelSize = new Size2D(ws.ws_xpixel, ws.ws_ypixel);
            return true;
        }

        if (LinuxImports.ioctl(LinuxImports.STDIN_FILENO, LinuxImports.TIOCGWINSZ, out ws) == 0 &&
            (ws.ws_xpixel > 0 || ws.ws_ypixel > 0)) {
            pixelSize = new Size2D(ws.ws_xpixel, ws.ws_ypixel);
            return true;
        }

        pixelSize = default;
        return false;
    }

    public void Dispose() {
        if (_disposed)
            return;

        RestoreGlobalMode();
        _disposed = true;
    }
}

#endif
