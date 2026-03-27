// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Console.Ansi.Mouse;
using NeoKolors.Console.Driver;
using NeoKolors.Console.Driver.Dotnet;
using NeoKolors.Console.Driver.Linux;
using NeoKolors.Console.Driver.Windows;
using NeoKolors.Console.Events;
using NeoKolors.Console.Input;
using static NeoKolors.Common.EscapeCodes;
using static NeoKolors.Console.Ansi.Mouse.MouseReportLevel;
using Std = System.Console;

namespace NeoKolors.Console;

public partial class NKConsole {
    
    private static readonly NKLogger LOGGER = NKDebug.GetLogger("NKConsole");
    
    private static bool             IS_ALT_BUFFER_ON;
    private static MouseReportLevel MOUSE_REPORT_LEVEL = NONE;
    private static bool             REPORT_FOCUS;
    private static bool             BRACKETED_PASTE_MODE;

    /// <summary>
    /// Gets or sets the output driver responsible for handling all character-based output to the terminal.
    /// </summary>
    /// <remarks>
    /// The output driver defines the mechanism through which text and formatted output
    /// are rendered in the terminal. Depending on the environment or platform, different
    /// implementations of the output driver may be used. For example,
    /// - Windows systems may use <c>WindowsOutputDriver</c>.
    /// - Unix-based a may use <c>LinuxOutputDriver</c>.
    /// - Other platforms may fall back to <c>DotnetOutputDriver</c>.
    /// This abstraction ensures that output operations are consistently handled across
    /// various platforms, while providing flexibility for future extensions or custom implementations.
    /// Modifying this property allows you to customize how the terminal output is generated.
    /// </remarks>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown if attempting to set the property while the current output driver is in use or disposed.
    /// </exception>
    public static IOutputDriver OutputDriver { get; set; } = GetDefaultOutput();

    /// <summary>
    /// Gets or sets the input driver responsible for processing console input events.
    /// </summary>
    /// <remarks>
    /// The input driver serves as the mechanism for handling various types of input events
    /// from a terminal or console, such as key presses, mouse interactions, focus changes,
    /// and clipboard paste events. It acts as an abstraction layer that ensures input operations
    /// are processed seamlessly across different platforms.
    /// The default implementation of this property is platform-specific:
    /// - For Windows environments, a <c>WindowsInputDriver</c> is used.
    /// - For Unix-based systems, a <c>LinuxInputDriver</c> is employed.
    /// - On other platforms, a generic implementation like <c>DotnetInputDriver</c> may be used.
    /// This flexibility allows developers to customize input handling or extend functionality
    /// by providing alternative driver implementations.
    /// Changing this property during an active input session may lead to undesired behavior
    /// or resource conflicts, as the driver manages its own lifecycle.
    /// </remarks>
    /// <exception cref="System.InvalidOperationException">
    /// Thrown when attempting to modify this property while input interception is enabled or
    /// the driver is actively processing input.
    /// </exception>
    public static IInputDriver InputDriver { get; set; } = GetDefaultInput();

    private static IOutputDriver GetDefaultOutput() {
        #if NK_ENABLE_NATIVE_IO
        return Environment.OSVersion.Platform switch {
            PlatformID.Win32Windows or PlatformID.Win32NT or PlatformID.Win32S or PlatformID.WinCE => 
                new WindowsOutputDriver(),
            PlatformID.Unix =>
                new LinuxOutputDriver(),
            PlatformID.MacOSX =>
                new DotnetOutputDriver(), // TODO: create a frickin MacOSX driver
            _ => new DotnetOutputDriver()
        };
        #else
        return new DotnetOutputDriver();
        #endif
    }

    private static IInputDriver GetDefaultInput() {
        #if NK_ENABLE_NATIVE_IO
        return Environment.OSVersion.Platform switch {
            PlatformID.Win32Windows or PlatformID.Win32NT or PlatformID.Win32S or PlatformID.WinCE => 
                new WindowsInputDriver(),
            PlatformID.Unix =>
                new LinuxInputDriver(),
            PlatformID.MacOSX =>
                new DotnetInputDriver(), // TODO: create a frickin MacOSX driver
            _ => new DotnetInputDriver()
        };
        #else
        return new DotnetInputDriver();
        #endif
    }

    /// <summary>
    /// Resets the input and output drivers to their default implementations
    /// based on the operating system platform. This method is typically used
    /// to restore the I/O drivers to their initial configurations after they
    /// have been modified.
    /// </summary>
    public static void ResetIoDrivers() {
        InputDriver  = GetDefaultInput ();
        OutputDriver = GetDefaultOutput();
    }

    /// <summary>
    /// Configures the console to use .NET-based input and output drivers. This method
    /// replaces the current I/O drivers with instances of DotnetInputDriver and
    /// DotnetOutputDriver, which are designed for compatibility with .NET platforms.
    /// </summary>
    public static void UseDotnetDrivers() {
        InputDriver  = new DotnetInputDriver ();
        OutputDriver = new DotnetOutputDriver();
    }

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
    public static event MouseEventHandler      Mouse           = delegate { };
    public static event PasteEventHandler      Paste           = delegate { };
    public static event FocusInEventHandler    FocusIn         = delegate { };
    public static event FocusOutEventHandler   FocusOut        = delegate { };
    public static event KeyEventHandler        Key             = delegate { };
    public static event VTQueryResponseHandler VTQueryResponse = delegate { };

    private static Task InvokeMouseEvent(MouseEventArgs info) 
        => Task.Run(() => Mouse(info));
    
    private static Task InvokePasteEvent(string text) 
        => Task.Run(() => Paste(text));
    
    private static Task InvokeFocusInEvent() 
        => Task.Run(() => FocusIn());
    
    private static Task InvokeFocusOutEvent() 
        => Task.Run(() => FocusOut());
    
    private static Task InvokeKeyEvent(KeyEventArgs info) 
        => Task.Run(() => Key(info));
    
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
}