//
// NeoKolors.Test
// Copyright (c) 2026 KryKom
//

using System.Diagnostics;
using System.Reflection;

namespace NeoKolors.Tui.Tests;

public class NKApplicationTests {
    private class DummyRenderable : IRenderable {
        public void Render(NKCharScreen screen) { }
        public void Render(ICharCanvas  canvas) {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public void UpdateFps_ShouldCalculateFpsCorrectly() {
        var app = new NKApplication(new NKAppConfig(
            fpsUpdateInterval: TimeSpan.FromSeconds(1)
        ), new DummyRenderable());

        var updateFpsMethod = typeof(NKApplication).GetMethod("UpdateFps", BindingFlags.NonPublic | BindingFlags.Instance);
        var lastFpsUpdateField = typeof(NKApplication).GetField("_lastFpsUpdate", BindingFlags.NonPublic | BindingFlags.Instance);
        var renderedCountField = typeof(NKApplication).GetField("_renderedCount", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.NotNull(updateFpsMethod);
        Assert.NotNull(lastFpsUpdateField);
        Assert.NotNull(renderedCountField);

        // 1. Invoke once to start
        updateFpsMethod.Invoke(app, null);
        Assert.Equal(0f, app.Fps); // should still be 0 as interval is 1s and no time passed

        // 2. Mock 5 renders, and set the last update timestamp to 1 second ago
        renderedCountField.SetValue(app, 5L);
        long frequency = Stopwatch.Frequency;
        long oneSecondAgo = Stopwatch.GetTimestamp() - frequency;
        lastFpsUpdateField.SetValue(app, oneSecondAgo);

        // 3. Trigger UpdateFps. It should see that 1s has elapsed, calculate FPS as (5 + 1) / 1.0 = 6.0
        updateFpsMethod.Invoke(app, null);

        // Since _renderedCount was incremented by UpdateFps, the total count evaluated was 6
        // And the elapsed seconds will be slightly more than 1.0. Let's assert it's around 6.0
        Assert.True(app.Fps >= 5.9f && app.Fps <= 6.1f, $"Expected FPS around 6.0, but got {app.Fps}");
    }
}
