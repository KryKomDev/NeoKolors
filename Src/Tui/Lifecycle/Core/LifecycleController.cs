// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Core;

/// <summary>
/// A thread-safe, robust state machine implementation for managing component and application lifecycles.
/// </summary>
public class LifecycleController : ILifecycle {
    private readonly object _syncRoot = new();
    private LifecycleState _state = LifecycleState.CREATED;

    /// <inheritdoc/>
    public LifecycleState State {
        get {
            lock (_syncRoot) {
                return _state;
            }
        }
    }

    /// <inheritdoc/>
    public bool IsRunning => State == LifecycleState.RUNNING;

    /// <inheritdoc/>
    public bool IsPaused => State == LifecycleState.PAUSED;

    /// <summary>
    /// Gets the synchronization root object.
    /// </summary>
    public object SyncRoot => _syncRoot;

    /// <inheritdoc/>
    public event EventHandler<LifecycleStateChangedEventArgs>? StateChanged;

    /// <summary>
    /// Occurs immediately before a state transition begins.
    /// </summary>
    public event EventHandler<LifecycleStateChangedEventArgs>? StateChanging;

    /// <inheritdoc/>
    public void Initialize() {
        TransitionTo(LifecycleState.INITIALIZING);
        OnInitializing();
        TransitionTo(LifecycleState.INITIALIZED);
        OnInitialized();
    }

    /// <inheritdoc/>
    public void Start() {
        lock (_syncRoot) {
            if (_state == LifecycleState.CREATED) {
                Initialize();
            }
        }

        TransitionTo(LifecycleState.STARTING);
        OnStarting();
        TransitionTo(LifecycleState.RUNNING);
        OnStarted();
    }

    /// <inheritdoc/>
    public void Pause() {
        lock (_syncRoot) {
            if (_state != LifecycleState.RUNNING) {
                return;
            }
        }

        TransitionTo(LifecycleState.PAUSING);
        OnPausing();
        TransitionTo(LifecycleState.PAUSED);
        OnPaused();
    }

    /// <inheritdoc/>
    public void Resume() {
        lock (_syncRoot) {
            if (_state != LifecycleState.PAUSED) {
                return;
            }
        }

        TransitionTo(LifecycleState.RESUMING);
        OnResuming();
        TransitionTo(LifecycleState.RUNNING);
        OnResumed();
    }

    /// <inheritdoc/>
    public void Stop() {
        lock (_syncRoot) {
            if (_state is LifecycleState.STOPPED or LifecycleState.DISPOSED or LifecycleState.STOPPING) {
                return;
            }
        }

        TransitionTo(LifecycleState.STOPPING);
        OnStopping();
        TransitionTo(LifecycleState.STOPPED);
        OnStopped();
    }

    /// <inheritdoc/>
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Protected dispose pattern implementation.
    /// </summary>
    protected virtual void Dispose(bool disposing) {
        lock (_syncRoot) {
            if (_state is LifecycleState.DISPOSING or LifecycleState.DISPOSED) {
                return;
            }
        }

        TransitionTo(LifecycleState.DISPOSING);
        if (disposing) {
            OnDisposing();
        }
        TransitionTo(LifecycleState.DISPOSED);
    }

    /// <summary>
    /// Safely transitions the state and triggers associated events.
    /// </summary>
    protected void TransitionTo(LifecycleState newState) {
        LifecycleState previousState;
        LifecycleStateChangedEventArgs args;

        lock (_syncRoot) {
            if (_state == newState) {
                return;
            }

            previousState = _state;
            args = new LifecycleStateChangedEventArgs(previousState, newState);
            StateChanging?.Invoke(this, args);
            _state = newState;
        }

        StateChanged?.Invoke(this, args);
    }

    // Virtual extension hooks
    protected virtual void OnInitializing() { }
    protected virtual void OnInitialized() { }
    protected virtual void OnStarting() { }
    protected virtual void OnStarted() { }
    protected virtual void OnPausing() { }
    protected virtual void OnPaused() { }
    protected virtual void OnResuming() { }
    protected virtual void OnResumed() { }
    protected virtual void OnStopping() { }
    protected virtual void OnStopped() { }
    protected virtual void OnDisposing() { }
}