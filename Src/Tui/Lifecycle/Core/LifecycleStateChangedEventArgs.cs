// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Core;

/// <summary>
/// Provides data for lifecycle state change events.
/// </summary>
public sealed class LifecycleStateChangedEventArgs : EventArgs {
    /// <summary>
    /// Gets the state prior to the transition.
    /// </summary>
    public LifecycleState PreviousState { get; }

    /// <summary>
    /// Gets the newly entered state.
    /// </summary>
    public LifecycleState NewState { get; }

    /// <summary>
    /// Gets the timestamp at which the transition occurred.
    /// </summary>
    public DateTimeOffset Timestamp { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LifecycleStateChangedEventArgs"/> class.
    /// </summary>
    public LifecycleStateChangedEventArgs(LifecycleState previousState, LifecycleState newState) {
        PreviousState = previousState;
        NewState = newState;
        Timestamp = DateTimeOffset.UtcNow;
    }

    /// <inheritdoc/>
    public override string ToString() => $"{PreviousState} -> {NewState} at {Timestamp:O}";
}
