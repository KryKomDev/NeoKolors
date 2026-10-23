// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle;

/// <summary>
/// Contract for components that require one-time initialization.
/// </summary>
public interface IInitializable {
    /// <summary>
    /// Initializes internal resources and state.
    /// </summary>
    void Initialize();
}

/// <summary>
/// Contract for components that can be started.
/// </summary>
public interface IStartable {
    /// <summary>
    /// Starts execution or activates the component.
    /// </summary>
    void Start();
}

/// <summary>
/// Contract for components that can pause and resume execution.
/// </summary>
public interface IPausable {
    /// <summary>
    /// Gets a value indicating whether the component is currently paused.
    /// </summary>
    bool IsPaused { get; }

    /// <summary>
    /// Pauses execution.
    /// </summary>
    void Pause();

    /// <summary>
    /// Resumes execution from a paused state.
    /// </summary>
    void Resume();
}

/// <summary>
/// Contract for components that can be stopped.
/// </summary>
public interface IStoppable {
    /// <summary>
    /// Stops execution and terminates active work.
    /// </summary>
    void Stop();
}

/// <summary>
/// Contract for components that receive variable-delta time ticks.
/// </summary>
public interface ITickable {
    /// <summary>
    /// Updates the component based on the elapsed variable frame time.
    /// </summary>
    void Update(DeltaTime time);
}

/// <summary>
/// Contract for components that receive deterministic fixed-delta time ticks (e.g. physics).
/// </summary>
public interface IFixedTickable {
    /// <summary>
    /// Updates the component based on a deterministic fixed time step.
    /// </summary>
    void FixedUpdate(TimeSpan fixedDelta);
}

/// <summary>
/// Composite contract for subsystems participating in a lifecycle.
/// </summary>
public interface ILifecycleParticipant : IInitializable, IStartable, IPausable, IStoppable, IDisposable {
}
