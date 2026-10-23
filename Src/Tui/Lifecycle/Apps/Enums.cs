// NeoKolors
// Copyright (c) 2026 KryKom

using System.ComponentModel;

namespace NeoKolors.Tui.Lifecycle.Apps;

/// <summary>
/// Defines the operational visibility and focus activation states of an application.
/// </summary>
public enum AppActivationState {
    /// <summary>
    /// The application is active, has user focus, and is running at standard interactive rate.
    /// </summary>
    ACTIVE,

    /// <summary>
    /// The application is visible but does not currently hold focus.
    /// </summary>
    INACTIVE,

    /// <summary>
    /// The application is running in the background (minimized or obscured).
    /// </summary>
    BACKGROUND,

    /// <summary>
    /// The application is suspended by the operating system or power policy.
    /// </summary>
    SUSPENDED
}

/// <summary>
/// Specifies the source or cause of an application close request.
/// </summary>
public enum CloseReason {
    /// <summary>
    /// The user requested to close the application (e.g., clicking close, shortcut key).
    /// </summary>
    USER_REQUEST,

    /// <summary>
    /// An interrupt signal was triggered (e.g., SIGINT, Ctrl+C).
    /// </summary>
    INTERRUPT,

    /// <summary>
    /// The host operating system or session is shutting down.
    /// </summary>
    SYSTEM_SHUTDOWN,

    /// <summary>
    /// The application logic explicitly initiated an exit sequence.
    /// </summary>
    APPLICATION_EXIT
}

/// <summary>
/// Provides data for the close request event with cancellation support.
/// </summary>
public sealed class CloseCancelEventArgs : CancelEventArgs {
    /// <summary>
    /// Gets the reason why the application was requested to close.
    /// </summary>
    public CloseReason Reason { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CloseCancelEventArgs"/> class.
    /// </summary>
    public CloseCancelEventArgs(CloseReason reason) : base(false) {
        Reason = reason;
    }
}

/// <summary>
/// Provides data for window or terminal buffer size change events.
/// </summary>
public sealed class SizeChangedEventArgs : EventArgs {
    public int Width { get; }
    public int Height { get; }
    public int PreviousWidth { get; }
    public int PreviousHeight { get; }

    public SizeChangedEventArgs(int width, int height, int prevWidth, int prevHeight) {
        Width = width;
        Height = height;
        PreviousWidth = prevWidth;
        PreviousHeight = prevHeight;
    }
}
