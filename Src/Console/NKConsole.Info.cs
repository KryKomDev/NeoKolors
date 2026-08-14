// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

#if NET7_0_OR_GREATER
#define NK_LIBIMPORT
#endif

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using HasFlagExtension;
using System.Collections;
using static NeoKolors.Console.NKConsole.VTPlatform;

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
    [FlagGroup("Windows", "Is")]
    [FlagGroup("Linux",   "Is")]
    [FlagGroup("MacOs",   "Is")]
    public enum VTPlatform {
        UNKNOWN,

        [FlagGroup("Windows")] WINDOWS_TERMINAL,
        [FlagGroup("Windows")] WINDOWS_CONHOST,
        [FlagGroup("Windows")] MINTTY,
        [FlagGroup("Windows")] CON_EMU,

        [FlagGroup("Windows")] [FlagGroup("Linux")] [FlagGroup("MacOs")]
        VSCODE,

        [FlagGroup("MacOs")] APPLE_TERMINAL,

        [FlagGroup("Windows")] [FlagGroup("Linux")] [FlagGroup("MacOs")]
        JETBRAINS,

        [FlagGroup("Windows")] [FlagGroup("Linux")] [FlagGroup("MacOs")]
        TMUX,

        [FlagGroup("Windows")] [FlagGroup("Linux")] [FlagGroup("MacOs")]
        XTERM,

        [FlagGroup("Linux")] LINUX_TTY,

        [FlagGroup("Linux")] [FlagGroup("MacOs")]
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
    public static VTInfo GetVTInfo() {
        // 1. check for explicit environment variables first
        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WT_SESSION")) ||
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WT_PROFILE_ID"))) {
            return new VTInfo(WINDOWS_TERMINAL);
        }

        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ConEmuPID"))) {
            return new VTInfo(CON_EMU);
        }

        var termProgram = Environment.GetEnvironmentVariable("TERM_PROGRAM");

        if (!string.IsNullOrEmpty(termProgram)) {
            return termProgram switch {
                "vscode"         => new VTInfo(VSCODE),
                "Apple_Terminal" => new VTInfo(APPLE_TERMINAL),
                "iTerm.app"      => new VTInfo(ITERM),
                "mintty"         => new VTInfo(MINTTY),
                _                => new VTInfo(GENERIC, termProgram)
            };
        }

        var termEmulator = Environment.GetEnvironmentVariable("TERMINAL_EMULATOR");

        if (!string.IsNullOrEmpty(termEmulator)) {
            return termEmulator.Contains("JetBrains")
                ? new VTInfo(JETBRAINS)
                : new VTInfo(GENERIC, termEmulator);
        }

        if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("TMUX"))) {
            return new VTInfo(TMUX);
        }

        // 2. if no specific environment variables match, try window/process inspection on Windows
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
            var win = GetWindowsVTInfo();

            if (win is not null)
                return win.Value;
        }

        // 3. fallback to the generic TERM variable (common on Linux/macOS)
        var term = Environment.GetEnvironmentVariable("TERM");

        if (string.IsNullOrEmpty(term)) {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? new VTInfo(WINDOWS_CONHOST)
                : new VTInfo(UNKNOWN);
        }

        if (term.Contains("xterm"))
            return new VTInfo(XTERM);

        return term switch {
            "linux"  => new VTInfo(LINUX_TTY),
            "screen" => new VTInfo(GNU_SCREEN),
            _        => new VTInfo(GENERIC, term)
        };
    }

    #if NK_LIBIMPORT

    [LibraryImport("kernel32.dll")]
    private static partial nint GetConsoleWindow();

    #else
    [DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    #endif

    #if NK_LIBIMPORT

    [LibraryImport("user32.dll", SetLastError = true)]
    private static partial uint GetWindowThreadProcessId(nint hWnd, out uint lpdwProcessId);

    [LibraryImport("user32.dll")]
    private static partial nint GetForegroundWindow();

    #else
    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll", ExactSpelling = true)]
    private static extern nint GetForegroundWindow();

    #endif

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private struct PROCESSENTRY32 {
        public uint   dwSize;
        public uint   cntUsage;
        public uint   th32ProcessID;
        public IntPtr th32DefaultHeapID;
        public uint   th32ModuleID;
        public uint   cntThreads;
        public uint   th32ParentProcessID;
        public int    pcPriClassBase;
        public uint   dwFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szExeFile;
    }

    // ReSharper disable once InconsistentNaming

    #pragma warning disable SYSLIB1054
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

    [DllImport("kernel32.dll", EntryPoint = "Process32FirstW", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool Process32First(nint hSnapshot, ref PROCESSENTRY32 lppe);

    [DllImport("kernel32.dll", EntryPoint = "Process32NextW", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern bool Process32Next(nint hSnapshot, ref PROCESSENTRY32 lppe);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(nint hObject);

    [DllImport("user32.dll", EntryPoint = "GetClassNameW", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern int GetClassName(nint hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);
    #pragma warning restore SYSLIB1054

    private static int GetParentProcessId(int processId) {
        var snapshot = CreateToolhelp32Snapshot(0x2, 0); // TH32CS_SNAPPROCESS = 0x2

        if (snapshot is 0 or -1)
            return 0;

        try {
            var entry = new PROCESSENTRY32 {
                dwSize = (uint)Marshal.SizeOf<PROCESSENTRY32>()
            };

            if (Process32First(snapshot, ref entry)) {
                do {
                    if (entry.th32ProcessID == processId) {
                        return (int)entry.th32ParentProcessID;
                    }
                }
                while (Process32Next(snapshot, ref entry));
            }
        }
        catch {
            // Ignore
        }
        finally {
            CloseHandle(snapshot);
        }

        return 0;
    }

    private static string GetWindowClassName(nint hWnd) {
        var sb = new System.Text.StringBuilder(256);

        if (GetClassName(hWnd, sb, sb.Capacity) == 0)
            return string.Empty;

        return sb.ToString();
    }

    private static bool IsRiderOrIdeaInParentChain() {
        try {
            int currentPid = Process.GetCurrentProcess().Id;
            int parentId   = currentPid;
            int depth      = 0;

            while (parentId != 0 && depth < 5) {
                parentId = GetParentProcessId(parentId);

                if (parentId != 0) {
                    using var parent = Process.GetProcessById(parentId);
                    string    name   = parent.ProcessName.ToLowerInvariant();

                    if (name.Contains("rider")    || name.Contains("idea")     || name.Contains("clion")    ||
                        name.Contains("webstorm") || name.Contains("phpstorm") || name.Contains("pycharm")  ||
                        name.Contains("rubymine") || name.Contains("goland")   || name.Contains("datagrip") ||
                        name.Contains("studio")   || name.Contains("jetbrains")) {
                        return true;
                    }
                }

                depth++;
            }
        }
        catch {
            // ignore
        }

        return false;
    }

    private static bool IsVSCodeInParentChain() {
        try {
            int currentPid = Process.GetCurrentProcess().Id;
            int parentId   = currentPid;
            int depth      = 0;

            while (parentId != 0 && depth < 5) {
                parentId = GetParentProcessId(parentId);

                if (parentId != 0) {
                    using var parent = Process.GetProcessById(parentId);
                    string    name   = parent.ProcessName.ToLowerInvariant();

                    if (name.Contains("vscode") || name.Contains("code")) {
                        return true;
                    }
                }

                depth++;
            }
        }
        catch {
            // ignore
        }

        return false;
    }

    private static VTInfo? GetWindowsVTInfo() {
        // 1. Check the foreground window (active window) process name first.
        // This is highly reliable for distinguishing between Rider's internal run console (where Rider is in the foreground)
        // and Rider's external console WT configuration (where Windows Terminal is in the foreground).
        var foregroundWindow = GetForegroundWindow();

        if (foregroundWindow != 0) {
            if (GetWindowThreadProcessId(foregroundWindow, out uint fgOwnerPid) != 0) {
                try {
                    using var fgOwner = Process.GetProcessById((int)fgOwnerPid);
                    string    name    = fgOwner.ProcessName.ToLowerInvariant();

                    if (name.Contains("windowsterminal") || name.Contains("openconsole")) {
                        return new VTInfo(WINDOWS_TERMINAL);
                    }

                    if (name.Contains("rider")     || name.Contains("idea")     ||
                        name.Contains("clion")     || name.Contains("webstorm") ||
                        name.Contains("phpstorm")  || name.Contains("pycharm")  ||
                        name.Contains("rubymine")  || name.Contains("goland")   ||
                        name.Contains("datagrip")  || name.Contains("studio")   ||
                        name.Contains("jetbrains") || name.Contains("fleet")) {
                        return new VTInfo(JETBRAINS);
                    }

                    if (name.Contains("vscode") || name.Contains("code")) {
                        return new VTInfo(VSCODE);
                    }
                }
                catch {
                    // Ignore and fall through
                }
            }
        }

        var consoleWindow = GetConsoleWindow();

        if (consoleWindow == IntPtr.Zero)
            return null;

        string className = GetWindowClassName(consoleWindow).ToLowerInvariant();

        // A. If we are running under ConPTY (modern terminal)
        if (className.Contains("pseudoconsole")) {
            // Distinguish between Rider's/VSCode's internal console (where output is redirected to Rider's panel)
            // and an external modern terminal (like Windows Terminal) spawned by Rider.
            if (!Stdio.IsOutputRedirected)
                return new VTInfo(WINDOWS_TERMINAL);

            if (IsRiderOrIdeaInParentChain()) {
                return new VTInfo(JETBRAINS);
            }

            return IsVSCodeInParentChain()
                ? new VTInfo(VSCODE)
                : new VTInfo(WINDOWS_TERMINAL);
        }

        // B. If we are running under the legacy console host (conhost.exe)
        if (className.Contains("consolewindowclass")) {
            return new VTInfo(WINDOWS_CONHOST);
        }

        // C. Fallback: inspect the window thread process ID
        if (GetWindowThreadProcessId(consoleWindow, out uint processId) != 0) {
            try {
                using var p           = Process.GetProcessById((int)processId);
                string    processName = p.ProcessName.ToLowerInvariant();

                if (processName.Contains("windowsterminal") || processName.Contains("openconsole"))
                    return new VTInfo(WINDOWS_TERMINAL);

                if (processName.Contains("conhost")) {
                    int parentId = GetParentProcessId((int)processId);

                    if (parentId == 0)
                        return new VTInfo(WINDOWS_CONHOST);

                    try {
                        using var parent     = Process.GetProcessById(parentId);
                        string    parentName = parent.ProcessName.ToLowerInvariant();

                        if (parentName.Contains("windowsterminal")) {
                            return new VTInfo(WINDOWS_TERMINAL);
                        }
                    }
                    catch {
                        // ignore and fall back to WINDOWS_CONHOST
                    }

                    return new VTInfo(WINDOWS_CONHOST);
                }
            }
            catch {
                // ignore
            }
        }

        // D. Fallback: walk the parent process tree of the current process
        try {
            int currentPid = Process.GetCurrentProcess().Id;
            int parentId   = currentPid;
            int depth      = 0;

            while (parentId != 0 && depth < 5) {
                parentId = GetParentProcessId(parentId);

                if (parentId != 0) {
                    using var parent = Process.GetProcessById(parentId);
                    string    name   = parent.ProcessName.ToLowerInvariant();

                    if (name.Contains("rider")    || name.Contains("idea")     || name.Contains("clion")    ||
                        name.Contains("webstorm") || name.Contains("phpstorm") || name.Contains("pycharm")  ||
                        name.Contains("rubymine") || name.Contains("goland")   || name.Contains("datagrip") ||
                        name.Contains("studio")   || name.Contains("jetbrains")) {
                        return new VTInfo(JETBRAINS);
                    }

                    if (name.Contains("vscode") || name.Contains("code")) {
                        return new VTInfo(VSCODE);
                    }

                    if (name.Contains("windowsterminal") || name.Contains("openconsole")) {
                        return new VTInfo(WINDOWS_TERMINAL);
                    }

                    if (name.Contains("conemu")) {
                        return new VTInfo(CON_EMU);
                    }
                }

                depth++;
            }
        }
        catch {
            // Ignore process accessing exceptions
        }

        return null;
    }

    #if DEBUG || NK_VT_DIAGNOSTICS

    public static void DumpDiagnostics() {
        var logPath = @"C:\Users\krystof\Desktop\projects\Libs\NeoKolors\Examples\Testing\diagnostics.log";
        var sb      = new System.Text.StringBuilder();
        sb.AppendLine("=== DIAGNOSTICS ===");
        sb.AppendLine($"OS: {Environment.OSVersion.VersionString}");
        sb.AppendLine($"Current Time: {DateTime.Now}");

        sb.AppendLine("\n--- Environment Variables ---");

        foreach (DictionaryEntry de in Environment.GetEnvironmentVariables()) {
            string key = de.Key?.ToString()   ?? "";
            string val = de.Value?.ToString() ?? "";

            if (key.Contains("WT")      || key.Contains("TERM")      || key.Contains("IDEA")  ||
                key.Contains("RIDER")   || key.Contains("JETBRAINS") || key.Contains("COLOR") ||
                key.Contains("SESSION") || key.Contains("PROMPT")) 
            {
                sb.AppendLine($"{key} = {val}");
            }
        }

        sb.AppendLine("\n--- Current Process Tree ---");

        try {
            var curr = Process.GetCurrentProcess();
            sb.AppendLine($"Current: PID={curr.Id}, Name={curr.ProcessName}");
            int parentId = GetParentProcessId(curr.Id);
            int depth    = 0;

            while (parentId != 0 && depth < 5) {
                using var parent = Process.GetProcessById(parentId);
                sb.AppendLine($"Parent({depth}): PID={parent.Id}, Name={parent.ProcessName}");
                parentId = GetParentProcessId(parent.Id);
                depth++;
            }
        }
        catch (Exception ex) {
            sb.AppendLine($"Error dumping current process tree: {ex.Message}");
        }

        sb.AppendLine("\n--- Console Window Process Tree ---");

        try {
            var consoleWindow = GetConsoleWindow();
            sb.AppendLine($"ConsoleWindow HWND: {consoleWindow}");

            if (consoleWindow != IntPtr.Zero) {
                GetWindowThreadProcessId(consoleWindow, out uint processId);
                sb.AppendLine($"ConsoleWindow Owner PID: {processId}");

                if (processId != 0) {
                    using var owner = Process.GetProcessById((int)processId);
                    sb.AppendLine($"ConsoleWindow Owner Name: {owner.ProcessName}");
                    int parentId = GetParentProcessId(owner.Id);
                    int depth    = 0;

                    while (parentId != 0 && depth < 5) {
                        using var parent = Process.GetProcessById(parentId);
                        sb.AppendLine($"ConsoleWindow Owner Parent({depth}): PID={parent.Id}, Name={parent.ProcessName}");
                        parentId = GetParentProcessId(parent.Id);
                        depth++;
                    }
                }
            }
        }
        catch (Exception ex) {
            sb.AppendLine($"Error dumping console window process tree: {ex.Message}");
        }

        File.WriteAllText(logPath, sb.ToString());
    }

    #endif
}