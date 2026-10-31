// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Core;

/// <summary>
/// Contract representing an object with an observable and manageable execution lifecycle.
/// </summary>
public interface ILifecycle : IDisposable {
    /// <summary>
    /// Gets the current state of this lifecycle.
    /// </summary>
    LifecycleState State { get; }

    /// <summary>
    /// Gets a value indicating whether the instance is in the running state.
    /// </summary>
    bool IsRunning => State == LifecycleState.RUNNING;

    /// <summary>
    /// Gets a value indicating whether the instance is currently paused.
    /// </summary>
    bool IsPaused => State == LifecycleState.PAUSED;

    /// <summary>
    /// Occurs whenever the lifecycle transitions to a new state.
    /// </summary>
    event EventHandler<LifecycleStateChangedEventArgs>? StateChanged;

    /// <summary>
    /// Transitions from <see cref="LifecycleState.CREATED"/> to <see cref="LifecycleState.INITIALIZED"/>.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Transitions from <see cref="LifecycleState.INITIALIZED"/> (or <see cref="LifecycleState.STOPPED"/>) to <see cref="LifecycleState.RUNNING"/>.
    /// </summary>
    void Start();

    /// <summary>
    /// Transitions from <see cref="LifecycleState.RUNNING"/> to <see cref="LifecycleState.PAUSED"/>.
    /// </summary>
    void Pause();

    /// <summary>
    /// Transitions from <see cref="LifecycleState.PAUSED"/> to <see cref="LifecycleState.RUNNING"/>.
    /// </summary>
    void Resume();

    /// <summary>
    /// Transitions from <see cref="LifecycleState.RUNNING"/> or <see cref="LifecycleState.PAUSED"/> to <see cref="LifecycleState.STOPPED"/>.
    /// </summary>
    void Stop();
}
