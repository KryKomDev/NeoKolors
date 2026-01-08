// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Console.Mouse;

namespace NeoKolors.Tui;

public readonly struct NKAppConfig {
    
    /// <summary>
    /// Represents the configuration settings related to rendering in the application.
    /// This property defines the behavior and constraints used for rendering operations,
    /// encapsulated in the RenderingConfig structure.
    /// </summary>
    public RenderingConfig Rendering { get; }

    /// <summary>
    /// Indicates whether the application will forcefully terminate when the Ctrl+C key combination is pressed.
    /// This property determines the behavior of the application in response to interrupt signals,
    /// providing a mechanism to control graceful or abrupt termination scenarios.
    /// </summary>
    public bool CtrlCForceQuits { get; }

    /// <summary>
    /// Represents the key combination used to signal an interruption in the application.
    /// This property is configured to trigger a predefined action upon the specified key press,
    /// where the default combination is `Ctrl+C`.
    /// </summary>
    public ConsoleKeyInfo InterruptCombination { get; }

    /// <summary>
    /// Specifies the mouse reporting protocol used by the application for capturing
    /// and handling mouse input in the terminal. This property defines the mode of
    /// encoding mouse events, which determines how the terminal communicates mouse
    /// actions to the application.
    /// </summary>
    public MouseReportProtocol MouseReportProtocol { get; }

    /// <summary>
    /// Specifies the level of mouse event reporting used within the application.
    /// This property allows customization of mouse event handling behavior,
    /// ranging from no events to comprehensive event tracking, as defined by
    /// the <see cref="NeoKolors.Console.Mouse.MouseReportLevel"/> enumeration.
    /// </summary>
    public MouseReportLevel MouseReportLevel { get; }

    /// <summary>
    /// Indicates whether the application supports bracketed paste mode.
    /// This property defines if paste operations input text as a distinct action,
    /// allowing for better handling of multiline or complex input scenarios.
    /// </summary>
    public bool BracketedPaste { get; }

    /// <summary>
    /// Indicates whether the application should automatically pause when it loses focus.
    /// This property provides a mechanism to manage application behavior in environments
    /// where focus changes can impact user experience or application state.
    /// </summary>
    public bool PauseOnFocusLost { get; }

    /// <summary>
    /// Specifies whether the cursor should remain disabled within the application.
    /// This property controls the visibility and interaction state of the cursor,
    /// and is primarily intended for terminal-based user interface configurations
    /// where cursor management is required for optimal user experience.
    /// </summary>
    public bool KeepCursorDisabled { get; }
    
    public NKAppConfig(
        RenderingConfig?    rendering            = null, 
        bool                ctrlCForceQuits      = false,
        ConsoleKeyInfo?     interruptCombination = null, 
        MouseReportProtocol mouseReportProtocol  = MouseReportProtocol.SGR,
        MouseReportLevel    mouseReportLevel     = MouseReportLevel.ALL, 
        bool                bracketedPaste       = false, 
        bool                pauseOnFocusLost     = true,
        bool                keepCursorDisabled   = true) 
    {
        Rendering            = rendering ?? RenderingConfig.Limited(24);
        CtrlCForceQuits      = ctrlCForceQuits;
        InterruptCombination = interruptCombination ?? new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, true);
        MouseReportProtocol  = mouseReportProtocol;
        MouseReportLevel     = mouseReportLevel;
        BracketedPaste       = bracketedPaste;
        PauseOnFocusLost     = pauseOnFocusLost;
        KeepCursorDisabled   = keepCursorDisabled;
    }

    public NKAppConfig() {
        Rendering            = RenderingConfig.Limited(24);
        CtrlCForceQuits      = false;
        InterruptCombination = new ConsoleKeyInfo('q', ConsoleKey.Q, false, false, true);
        MouseReportProtocol  = MouseReportProtocol.SGR;
        MouseReportLevel     = MouseReportLevel.ALL;
        BracketedPaste       = false;
        PauseOnFocusLost     = true;
        KeepCursorDisabled   = true;
    }

    public override string ToString() {
        return $"Rendering: {Rendering.ToString()}, " +
               $"Interrupt: {InterruptCombination.AsString()}, " +
               $"Mouse Protocol: {MouseReportProtocol.ToString()}, " +
               $"Mouse Level: {MouseReportLevel.ToString()}" +
               (CtrlCForceQuits    ? ", Ctrl+C Force Quits"    : "Ctrl+C Is Input") +
               (PauseOnFocusLost   ? ", Pauses On Focus Lost"  : "Runs Without Focus") +
               (BracketedPaste     ? ", Bracketed Paste"       : "") +
               (KeepCursorDisabled ? ", Keeps Cursor Disabled" : "");
    }
}