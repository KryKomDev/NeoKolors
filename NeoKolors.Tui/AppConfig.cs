//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common.Util;
using NeoKolors.Console.Mouse;

namespace NeoKolors.Tui;

/// <summary>
/// structure for configuring the application 
/// </summary>
public struct AppConfig {
    
    /// <summary>
    /// determines whether the Ctrl + C combination force quits the application,
    /// when true just makes the System.Console.TreatControlCAsInput false
    /// </summary>
    public bool CtrlCForceQuits {
        get;
        set {
            System.Console.TreatControlCAsInput = !value;

            field = value;
        }
    } = true;

    /// <summary>
    /// the combination that interrupts the application
    /// </summary>
    public ConsoleKeyInfo InterruptCombination { get; set; } = new('c', key: ConsoleKey.C, false, false, true);

    /// <summary>
    /// if true rerenders the application only on a key press 
    /// </summary>
    public bool LazyRender { get; set; } = true;

    /// <summary>
    /// maximum number of updates / renders of the application per second 
    /// </summary>
    /// <exception cref="ArgumentException">set value was less than 1</exception>
    public FramerateLimit FpsLimit { get; set; } = 20;

    /// <summary>
    /// if enabled you can switch to debug log using the key combination set by <see cref="ToggleDebugLogCombination"/>
    /// </summary>
    public bool EnableDebugLogging { get; set; } = false;

    /// <summary>
    /// key combination used to show the debug log, ctrl + alt + D by default
    /// </summary>
    public ConsoleKeyInfo ToggleDebugLogCombination { get; set; } = new('d', ConsoleKey.D, false, true, true);

    public MouseReportProtocol MouseReportProtocol { get; set; } = MouseReportProtocol.SGR;


    /// <param name="ctrlCForceQuits">determines whether the Ctrl + C combination force quits the application</param>
    /// <param name="interruptCombination">the combination that interrupts the application</param>
    /// <param name="lazyRender">if true rerenders the application only on a key press </param>
    /// <param name="fpsLimit">maximum number of updates / renders of the application per second</param>
    /// <param name="enableDebugLogging">
    /// if enabled you can switch to debug log using the key combination set by <see cref="ToggleDebugLogCombination"/>
    /// </param>
    /// <param name="toggleDebugLogCombination">
    /// key combination used to show the debug log, ctrl + alt + D by default,
    /// <see cref="EnableDebugLogging"/> must be true
    /// </param>
    /// <param name="mouseReportProtocol">Defines the protocol for mouse reporting.</param>
    public AppConfig(
        bool ctrlCForceQuits = true,
        ConsoleKeyInfo interruptCombination = default,
        bool lazyRender = true,
        FramerateLimit? fpsLimit = null, 
        bool enableDebugLogging = false, 
        ConsoleKeyInfo toggleDebugLogCombination = default, 
        MouseReportProtocol mouseReportProtocol = MouseReportProtocol.SGR)
    {
        CtrlCForceQuits = ctrlCForceQuits;
        InterruptCombination = interruptCombination == default 
            ? new ConsoleKeyInfo('c', key: ConsoleKey.C, false, false, true)
            : interruptCombination;
        LazyRender = lazyRender;
        FpsLimit = fpsLimit ?? 20;
        EnableDebugLogging = enableDebugLogging;
        ToggleDebugLogCombination = toggleDebugLogCombination == default
            ? new ConsoleKeyInfo('d', ConsoleKey.D, false, true, true)
            : toggleDebugLogCombination;
        MouseReportProtocol = mouseReportProtocol;
    }

    /// <summary>
    /// constructor with default settings
    /// </summary>
    public AppConfig() {
        CtrlCForceQuits = true;
        InterruptCombination = new ConsoleKeyInfo('c', key: ConsoleKey.C, false, false, true);
        LazyRender = true;
        FpsLimit = 20;
        EnableDebugLogging = false;
        ToggleDebugLogCombination = new ConsoleKeyInfo('d', ConsoleKey.D, false, true, true);
        MouseReportProtocol = MouseReportProtocol.SGR;
    }

    public override string ToString() {
        return $"CtrlCForceQuits: {CtrlCForceQuits}, " +
               $"InterruptCombination: {Extensions.ToString(InterruptCombination)}, " +
               $"LazyRender: {LazyRender}, " +
               $"MaxUpdatesPerSecond: {FpsLimit}, " +
               $"EnableDebugLogging: {EnableDebugLogging}, " +
               $"ToggleDebugLogCombination: {Extensions.ToString(ToggleDebugLogCombination)}";
    }

    public readonly struct FramerateLimit {
        private readonly int _framerate;
        private readonly bool _isLimited;
        
        public bool IsLimited => _isLimited;
        public int Framerate => _framerate;
        
        public FramerateLimit Unlimited => new();

        private FramerateLimit(bool isLimited = false, int framerate = 0) {
            _isLimited = isLimited;
            _framerate = framerate;
        }
        
        public FramerateLimit(int framerate) {
            if (framerate < 1) throw new ArgumentException(
                $"Tried to set MaxUpdatesPerSecond to {framerate}. Value must be greater than 0.");
            
            _isLimited = true;
            _framerate = framerate;
        }
        
        public static implicit operator FramerateLimit(int framerate) => new(framerate);

        public override string ToString() => _isLimited ? $"{_framerate} fps" : "Unlimited";
    }
}