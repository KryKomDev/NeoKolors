// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle;

/// <summary>
/// Represents high-precision frame timing information passed to update and render routines.
/// </summary>
public readonly struct DeltaTime : IEquatable<DeltaTime> {
    /// <summary>
    /// Gets the duration of the current frame as a <see cref="TimeSpan"/>.
    /// </summary>
    public TimeSpan Delta { get; }

    /// <summary>
    /// Gets the total elapsed running time as a <see cref="TimeSpan"/>.
    /// </summary>
    public TimeSpan Total { get; }

    /// <summary>
    /// Gets the sequential zero-based index of the current frame.
    /// </summary>
    public ulong FrameIndex { get; }

    /// <summary>
    /// Gets the elapsed frame duration in fractional seconds.
    /// </summary>
    public double DeltaSeconds => Delta.TotalSeconds;

    /// <summary>
    /// Gets the elapsed frame duration as a single-precision float in seconds.
    /// </summary>
    public float DeltaSecondsF => (float)Delta.TotalSeconds;

    /// <summary>
    /// Gets the total accumulated running time in fractional seconds.
    /// </summary>
    public double TotalSeconds => Total.TotalSeconds;

    /// <summary>
    /// Represents a zero time interval.
    /// </summary>
    public static DeltaTime Zero => new(TimeSpan.Zero, TimeSpan.Zero, 0);

    /// <summary>
    /// Initializes a new instance of the <see cref="DeltaTime"/> struct.
    /// </summary>
    public DeltaTime(TimeSpan delta, TimeSpan total, ulong frameIndex = 0) {
        Delta = delta;
        Total = total;
        FrameIndex = frameIndex;
    }

    /// <summary>
    /// Creates a <see cref="DeltaTime"/> instance from double-precision seconds.
    /// </summary>
    public static DeltaTime FromSeconds(double deltaSeconds, double totalSeconds, ulong frameIndex = 0) {
        return new DeltaTime(
            TimeSpan.FromSeconds(deltaSeconds),
            TimeSpan.FromSeconds(totalSeconds),
            frameIndex
        );
    }

    /// <inheritdoc/>
    public bool Equals(DeltaTime other) =>
        Delta == other.Delta && Total == other.Total && FrameIndex == other.FrameIndex;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is DeltaTime other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Delta, Total, FrameIndex);

    /// <inheritdoc/>
    public override string ToString() => $"Delta: {DeltaSeconds:F4}s, Total: {TotalSeconds:F2}s, Frame: {FrameIndex}";

    public static bool operator ==(DeltaTime left, DeltaTime right) => left.Equals(right);
    public static bool operator !=(DeltaTime left, DeltaTime right) => !left.Equals(right);
}
