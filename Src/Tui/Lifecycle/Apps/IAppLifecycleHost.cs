// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Apps;

/// <summary>
/// Contract representing an application lifecycle coordinator with focus, activation, and graceful close interception.
/// </summary>
public interface IAppLifecycleHost : ILifecycle {
    /// <summary>
    /// Gets the current operational activation state.
    /// </summary>
    AppActivationState ActivationState { get; }

    /// <summary>
    /// Gets a value indicating whether the application currently has user input focus.
    /// </summary>
    bool HasFocus { get; }

    /// <summary>
    /// Occurs when the input focus state changes.
    /// </summary>
    event EventHandler<bool>? FocusChanged;

    /// <summary>
    /// Occurs when the application activation state changes.
    /// </summary>
    event EventHandler<AppActivationState>? ActivationChanged;

    /// <summary>
    /// Occurs when an exit is requested, allowing handlers to cancel the termination.
    /// </summary>
    event EventHandler<CloseCancelEventArgs>? CloseRequested;

    /// <summary>
    /// Occurs when the window or display buffer is resized.
    /// </summary>
    event EventHandler<SizeChangedEventArgs>? WindowResized;

    /// <summary>
    /// Requests the application to close gracefully.
    /// </summary>
    /// <param name="reason">The reason for initiating closure.</param>
    /// <returns><c>true</c> if the closure was accepted and shutdown initiated; <c>false</c> if canceled.</returns>
    bool RequestClose(CloseReason reason = CloseReason.USER_REQUEST);

    /// <summary>
    /// Suspends the application lifecycle.
    /// </summary>
    void Suspend();

    /// <summary>
    /// Resumes the application lifecycle from a suspended state.
    /// </summary>
    void ResumeApp();

    /// <summary>
    /// Notifies the host that focus has changed.
    /// </summary>
    void NotifyFocusChanged(bool hasFocus);

    /// <summary>
    /// Notifies the host that the window size has changed.
    /// </summary>
    void NotifyWindowResized(int width, int height);
}
