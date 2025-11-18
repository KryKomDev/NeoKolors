// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Console.Events;
using NeoKolors.Console.Mouse;
using static NeoKolors.Common.EscapeCodes;
using static NeoKolors.Console.Mouse.MouseReportLevel;
using Std = System.Console;

namespace NeoKolors.Console;

public partial class NKConsole {
    
    private static bool IS_ALT_BUFFER_ON;
    private static MouseReportLevel MOUSE_REPORT_LEVEL = NONE;
    private static bool REPORT_FOCUS;
    private static bool BRACKETED_PASTE_MODE;

    private static readonly NKLogger LOGGER = NKDebug.GetLogger("NKConsole");

    public static bool IsAltBufferOn {
        get => IS_ALT_BUFFER_ON;
        set {
            Std.Write(value ? ALT_BUFF_SC_ENABLE : ALT_BUFF_RC_DISABLE);
            IS_ALT_BUFFER_ON = value;
            LOGGER.Info($"Alt buffer set to {value}");
        }
    }

    /// <summary>
    /// Enables the secondary (alternate) screen buffer and updates the internal state to reflect
    /// that the alternate buffer mode is active. This is typically used for creating temporary
    /// contexts, such as fullscreen terminal applications or temporary rendering surfaces.
    /// </summary>
    public static void EnableAltBuffer() {
        Std.Write(ALT_BUFF_SC_ENABLE);
        IS_ALT_BUFFER_ON = true;
        LOGGER.Info($"Alt buffer set to {IS_ALT_BUFFER_ON}");
    }

    /// <summary>
    /// Disables the secondary (alternate) screen buffer and updates the internal state to reflect
    /// that the alternate buffer mode is inactive. This is commonly used to restore the original
    /// terminal state, which includes switching back to the primary screen buffer.
    /// </summary>
    public static void DisableAltBuffer() {
        Std.Write(ALT_BUFF_RC_DISABLE);
        IS_ALT_BUFFER_ON = false;
        LOGGER.Info($"Alt buffer set to {IS_ALT_BUFFER_ON}");
    }

    /// <summary>
    /// Gets or sets the current mouse reporting protocol for the terminal.
    /// </summary>
    /// <remarks>
    /// The mouse reporting protocol determines the format in which mouse events are sent to the program through
    /// the terminal. The available protocols are:
    /// - X10: The legacy mouse reporting protocol.
    /// - UTF8: A protocol that encodes mouse events using UTF-8.
    /// - SGR: A modern protocol that includes more precise mouse information and extended functionality.
    /// When the property is set to a specific protocol, the terminal will be configured to use that protocol
    /// for later mouse events. Changing the protocol updates both the internal state and the terminal settings.
    /// </remarks>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when an unsupported value is assigned to this property.
    /// </exception>
    public static MouseReportProtocol MouseReportProtocol {
        get;
        set {
            switch (field) {
                case MouseReportProtocol.X10: break;
                case MouseReportProtocol.UTF8: Std.Write(MOUSE_EV_UTF8_OFF); break;
                case MouseReportProtocol.SGR: Std.Write(MOUSE_EV_SGR_OFF); break;
                default: throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
            switch (value) {
                case MouseReportProtocol.X10: break;
                case MouseReportProtocol.UTF8: Std.Write(MOUSE_EV_UTF8_ON); break;
                case MouseReportProtocol.SGR: Std.Write(MOUSE_EV_SGR_ON); break;
                default: throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            field = value;
            LOGGER.Info($"Mouse reporting protocol set to {value}");
        }
    } = MouseReportProtocol.X10;

    /// <summary>
    /// Gets or sets the mouse reporting level for the terminal.
    /// </summary>
    /// <remarks>
    /// The mouse reporting level specifies the types of mouse events that the terminal reports to the program.
    /// The possible levels are:
    /// - NONE: Disable all mouse event reporting.
    /// - PRESS: Report mouse button presses only.
    /// - PRESS_RELEASE: Report mouse button presses and releases.
    /// - DRAG: Report button presses, releases, and drag events.
    /// - MOVE: Report all mouse events, including movement and all button interactions.
    /// Changing the value of this property updates the terminal to reflect the selected reporting level.
    /// </remarks>
    /// <exception cref="System.ArgumentOutOfRangeException">
    /// Thrown when an unsupported value is assigned to this property.
    /// </exception>
    public static MouseReportLevel MouseReportLevel {
        get => MOUSE_REPORT_LEVEL;
        set {
            DisableCurrentMouseReport();
            
            switch (value) {
                case NONE: break;
                case PRESS: Std.Write(MOUSE_EV_ON_P_ON); break;
                case PRESS_RELEASE: Std.Write(MOUSE_EV_ON_PR_ON); break;
                case DRAG: Std.Write(MOUSE_EV_ON_PRD_ON); break;
                case ALL: Std.Write(MOUSE_EV_ON_ALL_ON); break;
                default: throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            MOUSE_REPORT_LEVEL = value;
            LOGGER.Info($"Mouse reporting level set to {value}");
        }
    }

    private static void DisableCurrentMouseReport() {
        switch (MOUSE_REPORT_LEVEL) {
            case NONE: break;
            case PRESS: Std.Write(MOUSE_EV_ON_P_OFF); break;
            case PRESS_RELEASE: Std.Write(MOUSE_EV_ON_PR_OFF); break;
            case DRAG: Std.Write(MOUSE_EV_ON_PRD_OFF); break;
            case ALL: Std.Write(MOUSE_EV_ON_ALL_OFF); break;
            default: throw new ArgumentOutOfRangeException(nameof(MOUSE_REPORT_LEVEL), MOUSE_REPORT_LEVEL, null);
        }
    }

    /// <summary>
    /// Enables mouse event tracking by activating the appropriate mouse handling mode.
    /// Updates the internal state to reflect that mouse events, including movement, are being actively reported.
    /// Commonly used for applications requiring interactive mouse inputs, such as terminal-based UI tools.
    /// </summary>
    public static void EnableMouseEvents() {
        Std.Write(MOUSE_EV_ON_ALL_ON);
        MOUSE_REPORT_LEVEL = ALL;
        LOGGER.Info($"Mouse reporting level set to ALL");
    }

    /// <summary>
    /// Disables mouse event tracking by deactivating the current mouse handling mode.
    /// Updates the internal state to reflect that mouse events, including movement,
    /// are no longer being reported. This is typically used when mouse interactivity
    /// is no longer required in terminal-based applications.
    /// </summary>
    public static void DisableMouseEvents() => DisableCurrentMouseReport();

    /// <summary>
    /// Gets or sets a value indicating whether focus reporting is enabled for the terminal.
    /// </summary>
    /// <remarks>
    /// Focus reporting allows the terminal to notify the program when it gains or loses focus.
    /// When enabled, the terminal sends specific escape sequences to signal focus changes:
    /// - Enabling focus reporting uses the sequence "\e[?1004h".
    /// - Disabling focus reporting uses the sequence "\e[?1004l".
    /// This feature is useful for interactive terminal applications that need to adjust their
    /// behavior based on the terminal's focus state.
    /// Changing this property sends the appropriate escape sequence to the terminal and updates the internal state.
    /// </remarks>
    public static bool ReportFocus {
        get => REPORT_FOCUS;
        set {
            Std.Write(value ? REPORT_FOCUS_ENABLE : REPORT_FOCUS_DISABLE); 
            REPORT_FOCUS = value;
            LOGGER.Info($"Focus reporting set to {value}");
        }
    }

    /// <summary>
    /// Enables focus reporting and updates the internal state to indicate that focus reporting is active.
    /// This functionality is used to track focus-in and focus-out events in the terminal environment,
    /// allowing the application to respond appropriately to focus changes.
    /// </summary>
    public static void EnableFocusReporting() {
        Std.Write(REPORT_FOCUS_ENABLE);
        REPORT_FOCUS = true;
        LOGGER.Info($"Focus reporting set to true");
    }

    /// <summary>
    /// Disables focus event reporting and updates the internal state to reflect
    /// that focus reporting is no longer active. This can be used to stop receiving
    /// notifications for terminal focus gain or loss events.
    /// </summary>
    public static void DisableFocusReporting() {
        Std.Write(REPORT_FOCUS_DISABLE);
        REPORT_FOCUS = false;
        LOGGER.Info($"Focus reporting set to false");
    }

    /// <summary>
    /// Gets or sets the state of Bracketed Paste Mode for the terminal.
    /// </summary>
    /// <remarks>
    /// When Bracketed Paste Mode is enabled, the terminal notifies the application when a user pastes content
    /// by wrapping the pasted text in specific escape sequences. This can help applications differentiate between
    /// user-typed text and pasted text, allowing for more predictable behavior during paste operations.
    /// Enabling or disabling this mode updates the internal state and sends the corresponding escape codes to the terminal:
    /// - Enabling sends the <c>ENABLE_BRACKETED_PASTE_MODE</c> escape code.
    /// - Disabling sends the <c>DISABLE_BRACKETED_PASTE_MODE</c> escape code.
    /// </remarks>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown if the underlying terminal does not support Bracketed Paste Mode.
    /// </exception>
    public static bool BracketedPasteMode {
        get => BRACKETED_PASTE_MODE;
        set {
            Std.Write(value ? ENABLE_BRACKETED_PASTE_MODE : DISABLE_BRACKETED_PASTE_MODE);
            BRACKETED_PASTE_MODE = value;
        }
    }

    /// <summary>
    /// Enables bracketed paste mode in the terminal, allowing applications to handle
    /// paste operations more effectively by distinguishing pasted text from regular input.
    /// Updates the internal state to indicate that bracketed paste mode is active.
    /// </summary>
    public static void EnableBracketedPasteMode() {
        Std.Write(ENABLE_BRACKETED_PASTE_MODE);
        BRACKETED_PASTE_MODE = true;
    }

    /// <summary>
    /// Disables the bracketed paste mode in the terminal and updates the internal state to indicate
    /// that bracketed paste mode is inactive. This is typically used to restore the default clipboard
    /// handling behavior in the terminal environment.
    /// </summary>
    public static void DisableBracketedPasteMode() {
        Std.Write(DISABLE_BRACKETED_PASTE_MODE);
        BRACKETED_PASTE_MODE = false;
    }

    // Events for input interception
    public static event MouseEventHandler          MouseEvent          = delegate { };
    public static event PasteEventHandler          PasteEvent          = delegate { };
    public static event FocusInEventHandler        FocusInEvent        = delegate { };
    public static event FocusOutEventHandler       FocusOutEvent       = delegate { };
    public static event KeyEventHandler            KeyEvent            = delegate { };
    public static event WinOpsResponseEventHandler WinOpsResponseEvent = delegate { };
    public static event DecReqResponseEventHandler DecReqResponseEvent = delegate { };

    private static Task InvokeMouseEvent(MouseEventInfo info) 
        => Task.Run(() => MouseEvent(info));
    
    private static Task InvokePasteEvent(string text) 
        => Task.Run(() => PasteEvent(text));
    
    private static Task InvokeFocusInEvent() 
        => Task.Run(() => FocusInEvent());
    
    private static Task InvokeFocusOutEvent() 
        => Task.Run(() => FocusOutEvent());
    
    private static Task InvokeKeyEvent(ConsoleKeyInfo info) 
        => Task.Run(() => KeyEvent(info));

    private static Task InvokeWinOpsResponseEvent(WinOpsResponseArgs args) 
        => Task.Run(() => WinOpsResponseEvent(args));
    
    private static Task InvokeDecReqResponseEvent(DecReqResponseArgs args) 
        => Task.Run(() => DecReqResponseEvent(args));
    
    /// <summary>
    /// Gets value indicating whether input from the terminal is intercepted by NKConsole.
    /// </summary>
    /// <remarks>
    /// When <c>true</c>, input is captured and processed internally before being passed
    /// to the underlying system via NKConsole's events. This feature is typically used for
    /// handling custom input events or creating interactive terminal-based applications
    /// where low-level input control is required.
    /// Changing the value to <c>false</c> stops input interception and allows the input
    /// to flow directly to the system or parent application.
    /// </remarks>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown when attempting to enable interception in a state where it is not supported
    /// or applicable, such as during shutdown or initialization phases.
    /// </exception>
    public static bool InterceptInput { get; private set; }

    /// <summary>
    /// Activates the input interception mechanism and initiates the input handling thread.
    /// Once initiated, the console will begin to listen for user inputs such as keys, mouse events,
    /// and other interactions, enabling dynamic response handling in terminal applications.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Input interception is already enabled.
    /// </exception>
    public static void StartInputInterception() {
        if (InterceptInput)
            throw new InvalidOperationException("Input interception is already enabled.");
        
        InterceptInput = true;
        LOGGER.Info("Starting input interception...");
        INPUT_THREAD.Start();
    }

    /// <summary>
    /// Stops the interception of input by setting the internal state to false.
    /// This is used to cease capturing or processing input events, thereby
    /// restoring the default behavior where input is no longer intercepted.
    /// </summary>
    public static void StopInputInterception() {
        InterceptInput = false;
        LOGGER.Info("Stopping input interception...");
    }
}