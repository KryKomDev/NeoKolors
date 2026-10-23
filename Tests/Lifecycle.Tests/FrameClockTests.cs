// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Tests;

public class FrameClockTests {
    [Fact]
    public void Clock_Tick_IncrementsFrameCount() {
        var clock = new FrameClock();

        DeltaTime time1 = clock.Tick();
        Assert.Equal(1UL, time1.FrameIndex);
        Assert.Equal(1UL, clock.FrameCount);

        DeltaTime time2 = clock.Tick();
        Assert.Equal(2UL, time2.FrameIndex);
        Assert.Equal(2UL, clock.FrameCount);
    }

    [Fact]
    public void Clock_Reset_RestoresInitialState() {
        var clock = new FrameClock();
        clock.Tick();
        clock.Tick();

        clock.Reset();
        Assert.Equal(0UL, clock.FrameCount);
        Assert.Equal(TimeSpan.Zero, clock.TotalTime);
        Assert.Equal(0.0, clock.SmoothedFps);
    }

    [Fact]
    public void DeltaTime_FromSeconds_InitializesProperly() {
        DeltaTime dt = DeltaTime.FromSeconds(0.016, 1.5, 42);

        Assert.Equal(0.016, dt.DeltaSeconds, precision: 3);
        Assert.Equal(1.5, dt.TotalSeconds, precision: 1);
        Assert.Equal(42UL, dt.FrameIndex);
    }
}
