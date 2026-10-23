// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Game2D;

/// <summary>
///     Execution context supplied during deterministic fixed-timestep simulation updates
///     (e.g., physics, collision).
/// </summary>
public readonly struct Game2DFixedContext {
    /// <summary>
    ///     Gets the fixed duration for this simulation step.
    /// </summary>
    public TimeSpan FixedDelta { get; }

    /// <summary>
    ///     Gets the step duration in fractional seconds.
    /// </summary>
    public double FixedDeltaSeconds => FixedDelta.TotalSeconds;

    /// <summary>
    ///     Gets the step duration as a single-precision float in seconds.
    /// </summary>
    public float FixedDeltaSecondsF => (float)FixedDelta.TotalSeconds;

    /// <summary>
    ///     Gets the sequential index of the fixed physics tick.
    /// </summary>
    public ulong FixedTickIndex { get; }

    /// <summary>
    ///     Gets optional user or game state passed to the fixed tick.
    /// </summary>
    public object? State { get; }

    public Game2DFixedContext(TimeSpan fixedDelta, ulong fixedTickIndex, object? state = null) {
        FixedDelta = fixedDelta;
        FixedTickIndex = fixedTickIndex;
        State = state;
    }
}

/// <summary>
///     Execution context supplied during variable frame rate logic updates.
/// </summary>
public readonly struct Game2DUpdateContext {
    /// <summary>
    ///     Gets the variable frame timing data.
    /// </summary>
    public DeltaTime Time { get; }

    /// <summary>
    ///     Gets optional user or game state passed to the update tick.
    /// </summary>
    public object? State { get; }

    public Game2DUpdateContext(DeltaTime time, object? state = null) {
        Time = time;
        State = state;
    }
}

/// <summary>
///     Execution context supplied to 2D rendering passes, containing the interpolation alpha
///     and camera.
/// </summary>
public readonly struct Game2DRenderContext {
    /// <summary>
    ///     Gets the fractional remainder (between 0.0 and 1.0) between the previous and current
    ///     physics ticks.
    ///     Used to interpolate sprite positions smoothly across frames.
    /// </summary>
    public double Alpha { get; }

    /// <summary>
    ///     Gets the variable frame timing data.
    /// </summary>
    public DeltaTime Time { get; }

    /// <summary>
    ///     Gets the primary 2D camera viewport.
    /// </summary>
    public Camera2D Camera { get; }

    /// <summary>
    ///     Gets optional user or rendering backend state.
    /// </summary>
    public object? State { get; }

    public Game2DRenderContext(double alpha, DeltaTime time, Camera2D camera, object? state = null) {
        Alpha = alpha;
        Time = time;
        Camera = camera;
        State = state;
    }
}
