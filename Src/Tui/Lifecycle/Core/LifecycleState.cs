// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Core;

/// <summary>
/// Defines the lifecycle states of an application, service, or game loop.
/// </summary>
public enum LifecycleState {
    /// <summary>
    /// The lifecycle instance has been constructed but not initialized.
    /// </summary>
    CREATED,

    /// <summary>
    /// Dependencies and subsystems are being initialized.
    /// </summary>
    INITIALIZING,

    /// <summary>
    /// Subsystems are fully initialized and ready to start.
    /// </summary>
    INITIALIZED,

    /// <summary>
    /// Resources are being allocated and the execution loop is starting.
    /// </summary>
    STARTING,

    /// <summary>
    /// The execution loop is actively running and processing ticks.
    /// </summary>
    RUNNING,

    /// <summary>
    /// The execution loop is transitioning into a suspended or paused state.
    /// </summary>
    PAUSING,

    /// <summary>
    /// The execution loop is paused. Updates or rendering may be suspended or throttled.
    /// </summary>
    PAUSED,

    /// <summary>
    /// The execution loop is resuming from a paused state.
    /// </summary>
    RESUMING,

    /// <summary>
    /// Termination has been requested and graceful shutdown is underway.
    /// </summary>
    STOPPING,

    /// <summary>
    /// The execution loop has completely terminated.
    /// </summary>
    STOPPED,

    /// <summary>
    /// Native handles, buffers, and resources are being disposed.
    /// </summary>
    DISPOSING,

    /// <summary>
    /// The lifecycle instance has been completely disposed and cannot be reused.
    /// </summary>
    DISPOSED
}
