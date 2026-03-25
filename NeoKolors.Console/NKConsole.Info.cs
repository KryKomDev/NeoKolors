// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

#if NET7_0_OR_GREATER
#define NK_LIBIMPORT
#endif

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using NeoKolors.Console.Driver.Windows;

namespace NeoKolors.Console;

public static partial class NKConsole {
    
    /// <summary>
    /// Represents the platform or terminal emulator where the virtual terminal
    /// is currently being executed.
    /// </summary>
    /// <remarks>
    /// This enum categorizes various terminal environments to enable context-aware
    /// behavior within different terminal emulators. It includes platforms such as
    /// Windows Terminal, VS Code, Apple Terminal, and others.
    /// </remarks>
    public enum VTPlatform {
        UNKNOWN,
        WINDOWS_TERMINAL,
        WINDOWS_CONHOST,
        CON_EMU,
        VSCODE,
        APPLE_TERMINAL,
        MINTTY,
        JETBRAINS,
        TMUX,
        XTERM,
        LINUX_TTY,
        GNU_SCREEN,
        ITERM,
        GENERIC,
    }
    
    public readonly record struct VTInfo {
        public VTPlatform Platform       { get; init; }
        public string?    AdditionalInfo { get; init; }

        public VTInfo(VTPlatform platform, string? additionalInfo = null) {
            Platform       = platform;
            AdditionalInfo = additionalInfo;
        }

        public override string ToString() => $"{Platform}{(AdditionalInfo is not null ? ": " + AdditionalInfo : "")}";
    }
    
    /// <summary>
    /// Detects the name of the current virtual terminal or terminal emulator.
    /// </summary>
    /// <returns>A string representing the name of the terminal.</returns>
    public static VTInfo GetTerminalName() {
        
        // 1. check for Windows Terminal
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            var win = GetWindowsVTInfo();
        
            if (win is not null) 
                return win.Value;
        }
        
        // 2. check for ConEmu / Cmder
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ConEmuPID"))) {
            return new VTInfo(VTPlatform.CON_EMU);
        }

        // 3. check for programs that explicitly set TERM_PROGRAM (VS Code, Apple Terminal, iTerm2, etc.)
        var termProgram = Environment.GetEnvironmentVariable("TERM_PROGRAM");
        
        if (!string.IsNullOrEmpty(termProgram)) {
            return termProgram switch {
                "vscode"         => new VTInfo(VTPlatform.VSCODE),
                "Apple_Terminal" => new VTInfo(VTPlatform.APPLE_TERMINAL),
                "iTerm.app"      => new VTInfo(VTPlatform.ITERM),
                "mintty"         => new VTInfo(VTPlatform.MINTTY),
                _                => new VTInfo(VTPlatform.GENERIC, termProgram) 
                // return the raw name if we don't have a friendly mapping
            };
        }

        // 4. check for JetBrains IDE terminals (Rider, IntelliJ, etc.)
        var termEmulator = Environment.GetEnvironmentVariable("TERMINAL_EMULATOR");
        
        if (!string.IsNullOrEmpty(termEmulator)) {
            return termEmulator.Contains("JetBrains") 
                ? new VTInfo(VTPlatform.JETBRAINS)
                : new VTInfo(VTPlatform.GENERIC, termEmulator);
        }

        // 5. check for Multiplexers (tmux, screen)
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TMUX"))) {
            return new VTInfo(VTPlatform.TMUX);
        }

        // 6. fallback to the generic TERM variable (common on Linux/macOS)
        var term = Environment.GetEnvironmentVariable("TERM");
        
        if (!string.IsNullOrEmpty(term)) {
            if (term.Contains("xterm")) 
                return new VTInfo(VTPlatform.XTERM);
            
            return term switch {
                "linux"  => new VTInfo(VTPlatform.LINUX_TTY),
                "screen" => new VTInfo(VTPlatform.GNU_SCREEN),
                _        => new VTInfo(VTPlatform.GENERIC, term)
            };
        }

        // 7. if everything fails, it's likely the classic Windows Console or an unknown environment
        return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
            ? new VTInfo(VTPlatform.WINDOWS_CONHOST) 
            : new VTInfo(VTPlatform.UNKNOWN);
    }

    #if NK_LIBIMPORT
    [LibraryImport("kernel32.dll")]
    private static partial IntPtr GetConsoleWindow();
    #else
    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();
    #endif

    #if NK_LIBIMPORT
    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    #else
    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
    #endif
    
    private static VTInfo? GetWindowsVTInfo() {
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WT_SESSION")) ||
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WT_PROFILE_ID")))
        {
            return new VTInfo(VTPlatform.WINDOWS_TERMINAL);
        }

        var consoleWindow = GetConsoleWindow();
        
        if (consoleWindow == IntPtr.Zero) 
            return null;
        
        if (GetWindowThreadProcessId(consoleWindow, out uint processId) == 0)
            Win32Exception.ThrowLast();

        try {
            using var p = Process.GetProcessById((int)processId);
            string processName = p.ProcessName.ToLowerInvariant();

                if (processName.Contains("windowsterminal") || processName.Contains("openconsole"))
                return new VTInfo(VTPlatform.WINDOWS_TERMINAL);
            
            if (processName.Contains("conhost"))
                return new VTInfo(VTPlatform.WINDOWS_CONHOST);
        }
        catch {
            // Catch access denied exceptions just in case
        }

        return null;
    }
}