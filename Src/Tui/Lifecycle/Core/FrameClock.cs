// NeoKolors
// Copyright (c) 2026 KryKom

using System.Diagnostics;

namespace NeoKolors.Tui.Lifecycle;

/// <summary>
/// A high-precision frame clock that measures frame intervals, tracks total running time,
/// and computes smoothed frame rates.
/// </summary>
public sealed class FrameClock {
    private long _lastTimestamp;
    private TimeSpan _totalTime = TimeSpan.Zero;
    private ulong _frameIndex = 0;
    private double _smoothedFps = 0.0;
    private const double FPS_SMOOTHING_FACTOR = 0.1;

    /// <summary>
    /// Gets or sets the maximum frame interval to record, clamping large pause anomalies. Default is 250 ms.
    /// </summary>
    public TimeSpan MaxDeltaTime { get; set; } = TimeSpan.FromSeconds(0.25);

    /// <summary>
    /// Gets the total execution duration recorded by this clock.
    /// </summary>
    public TimeSpan TotalTime => _totalTime;

    /// <summary>
    /// Gets the number of frames elapsed.
    /// </summary>
    public ulong FrameCount => _frameIndex;

    /// <summary>
    /// Gets the exponential moving average frames-per-second (FPS).
    /// </summary>
    public double SmoothedFps => _smoothedFps;

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameClock"/> class and starts measuring.
    /// </summary>
    public FrameClock() {
        Reset();
    }

    /// <summary>
    /// Resets the clock to the current timestamp.
    /// </summary>
    public void Reset() {
        _lastTimestamp = Stopwatch.GetTimestamp();
        _totalTime = TimeSpan.Zero;
        _frameIndex = 0;
        _smoothedFps = 0.0;
    }

    /// <summary>
    /// Advances the clock by one frame, computing delta time and updating metrics.
    /// </summary>
    /// <returns>A <see cref="DeltaTime"/> containing the frame interval and total time.</returns>
    public DeltaTime Tick() {
        long currentTimestamp = Stopwatch.GetTimestamp();
        double elapsedSeconds = (double)(currentTimestamp - _lastTimestamp) / Stopwatch.Frequency;
        _lastTimestamp = currentTimestamp;

        if (elapsedSeconds < 0.0) {
            elapsedSeconds = 0.0;
        }

        TimeSpan rawDelta = TimeSpan.FromSeconds(elapsedSeconds);
        TimeSpan clampedDelta = rawDelta > MaxDeltaTime ? MaxDeltaTime : rawDelta;

        _totalTime += clampedDelta;
        _frameIndex++;

        // Update smoothed FPS
        if (clampedDelta.TotalSeconds > 0.00001) {
            double instantFps = 1.0 / clampedDelta.TotalSeconds;
            if (_smoothedFps <= 0.0) {
                _smoothedFps = instantFps;
            } else {
                _smoothedFps = (_smoothedFps * (1.0 - FPS_SMOOTHING_FACTOR)) + (instantFps * FPS_SMOOTHING_FACTOR);
            }
        }

        return new DeltaTime(clampedDelta, _totalTime, _frameIndex);
    }
}
