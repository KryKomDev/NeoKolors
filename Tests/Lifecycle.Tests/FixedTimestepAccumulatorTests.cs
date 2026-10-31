// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Lifecycle.Core;

namespace NeoKolors.Tui.Lifecycle.Tests;

public class FixedTimestepAccumulatorTests {
    [Fact]
    public void Accumulator_ConsumesExactSteps() {
        var step = TimeSpan.FromMilliseconds(20);
        var accumulator = new FixedTimestepAccumulator(step);

        // Accumulate 50ms (should consume two 20ms steps with 10ms remaining)
        accumulator.Accumulate(TimeSpan.FromMilliseconds(50));

        Assert.True(accumulator.Step(out var firstStep));
        Assert.Equal(step, firstStep);

        Assert.True(accumulator.Step(out var secondStep));
        Assert.Equal(step, secondStep);

        Assert.False(accumulator.Step(out _));
        Assert.Equal(TimeSpan.FromMilliseconds(10), accumulator.Accumulated);
    }

    [Fact]
    public void Accumulator_ComputesCorrectAlpha() {
        var step = TimeSpan.FromMilliseconds(100);
        var accumulator = new FixedTimestepAccumulator(step);

        accumulator.Accumulate(TimeSpan.FromMilliseconds(50));
        Assert.Equal(0.5, accumulator.Alpha, precision: 3);

        accumulator.Accumulate(TimeSpan.FromMilliseconds(25));
        Assert.Equal(0.75, accumulator.Alpha, precision: 3);
    }

    [Fact]
    public void Accumulator_ClampsMaxDeltaToPreventDeathSpirals() {
        var step = TimeSpan.FromMilliseconds(16);
        var maxClamp = TimeSpan.FromMilliseconds(100);
        var accumulator = new FixedTimestepAccumulator(step, maxClamp);

        // Simulate 5-second extreme stall
        accumulator.Accumulate(TimeSpan.FromSeconds(5));

        Assert.True(accumulator.Accumulated <= maxClamp);
    }

    [Fact]
    public void Accumulator_Reset_ClearsAccumulatedTime() {
        var accumulator = new FixedTimestepAccumulator(TimeSpan.FromMilliseconds(20));
        accumulator.Accumulate(TimeSpan.FromMilliseconds(50));

        accumulator.Reset();
        Assert.Equal(TimeSpan.Zero, accumulator.Accumulated);
        Assert.Equal(0.0, accumulator.Alpha);
    }
}
