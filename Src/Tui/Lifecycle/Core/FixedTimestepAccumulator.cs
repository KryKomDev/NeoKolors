// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle;

/// <summary>
/// Implements a fixed-timestep accumulator with delta clamping to prevent simulation death spirals
/// and computes the interpolation alpha for rendering.
/// </summary>
public class FixedTimestepAccumulator {
    private TimeSpan _accumulated = TimeSpan.Zero;

    /// <summary>
    /// Gets or sets the target duration for each fixed simulation step. Default is 60 Hz (~16.666 ms).
    /// </summary>
    public TimeSpan TargetStep { get; set; } = TimeSpan.FromSeconds(1.0 / 60.0);

    /// <summary>
    /// Gets or sets the maximum time delta that can be accumulated in a single frame.
    /// Clamping prevents the "spiral of death" during extreme lag spikes. Default is 250 ms.
    /// </summary>
    public TimeSpan MaxAccumulatedDelta { get; set; } = TimeSpan.FromSeconds(0.25);

    /// <summary>
    /// Gets the current unprocessed duration stored in the accumulator.
    /// </summary>
    public TimeSpan Accumulated => _accumulated;

    /// <summary>
    /// Gets the fractional remainder between 0.0 and 1.0 representing the progress toward the next fixed step.
    /// Used by rendering engines to interpolate smoothly between previous and current physics states.
    /// </summary>
    public double Alpha {
        get {
            if (TargetStep <= TimeSpan.Zero || _accumulated <= TimeSpan.Zero) {
                return 0.0;
            }

            double alpha = _accumulated.TotalSeconds / TargetStep.TotalSeconds;
            return alpha switch {
                < 0.0 => 0.0,
                > 1.0 => 1.0,
                _ => alpha
            };
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedTimestepAccumulator"/> class.
    /// </summary>
    public FixedTimestepAccumulator() {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedTimestepAccumulator"/> class with a custom step duration.
    /// </summary>
    public FixedTimestepAccumulator(TimeSpan targetStep, TimeSpan? maxAccumulatedDelta = null) {
        TargetStep = targetStep;
        if (maxAccumulatedDelta.HasValue) {
            MaxAccumulatedDelta = maxAccumulatedDelta.Value;
        }
    }

    /// <summary>
    /// Adds elapsed frame time to the accumulator, applying maximum clamping limits.
    /// </summary>
    public void Accumulate(TimeSpan rawDelta) {
        if (rawDelta <= TimeSpan.Zero) {
            return;
        }

        TimeSpan clampedDelta = rawDelta > MaxAccumulatedDelta ? MaxAccumulatedDelta : rawDelta;
        _accumulated += clampedDelta;

        if (_accumulated > MaxAccumulatedDelta) {
            _accumulated = MaxAccumulatedDelta;
        }
    }

    /// <summary>
    /// Attempts to consume one fixed simulation step from the accumulator.
    /// </summary>
    /// <param name="stepDelta">When this method returns <c>true</c>, contains the fixed step duration.</param>
    /// <returns><c>true</c> if a step was consumed; otherwise <c>false</c>.</returns>
    public bool Step(out TimeSpan stepDelta) {
        if (_accumulated >= TargetStep && TargetStep > TimeSpan.Zero) {
            _accumulated -= TargetStep;
            stepDelta = TargetStep;
            return true;
        }

        stepDelta = TimeSpan.Zero;
        return false;
    }

    /// <summary>
    /// Resets the accumulator back to zero.
    /// </summary>
    public void Reset() {
        _accumulated = TimeSpan.Zero;
    }
}
