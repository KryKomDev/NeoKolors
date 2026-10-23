// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Lifecycle.Apps;

namespace NeoKolors.Tui.Lifecycle.Tests;

public class AppLifecycleTests {
    private sealed class MockAppView : IAppView {
        public bool IsDirty { get; private set; } = true;
        public int RenderCount { get; private set; }

        public void Invalidate() => IsDirty = true;
        public void ResetDirty() => IsDirty = false;

        public void Render() {
            RenderCount++;
        }
    }

    [Fact]
    public void AppLifecycleHost_FocusAndActivation_TransitionsCorrectly() {
        var host = new AppLifecycleHost();
        host.Start();

        // Focused on start
        Assert.Equal(AppActivationState.ACTIVE, host.ActivationState);
        Assert.True(host.HasFocus);

        // Loss of focus -> Inactive
        host.NotifyFocusChanged(false);
        Assert.Equal(AppActivationState.INACTIVE, host.ActivationState);
        Assert.False(host.HasFocus);

        // Gain focus -> Active
        host.NotifyFocusChanged(true);
        Assert.Equal(AppActivationState.ACTIVE, host.ActivationState);
        Assert.True(host.HasFocus);

        // Suspend
        host.Suspend();
        Assert.Equal(AppActivationState.SUSPENDED, host.ActivationState);
        Assert.True(host.IsPaused);

        // Resume
        host.ResumeApp();
        Assert.Equal(AppActivationState.ACTIVE, host.ActivationState);
        Assert.False(host.IsPaused);
    }

    [Fact]
    public void AppLifecycleHost_CloseRequest_CanBeCanceled() {
        var host = new AppLifecycleHost();
        host.Start();

        // Attach cancel handler
        host.CloseRequested += (_, args) => {
            if (args.Reason == CloseReason.USER_REQUEST) {
                args.Cancel = true;
            }
        };

        bool accepted = host.RequestClose(CloseReason.USER_REQUEST);
        Assert.False(accepted);
        Assert.True(host.IsRunning);

        // System shutdown is not cancelled
        accepted = host.RequestClose(CloseReason.SYSTEM_SHUTDOWN);
        Assert.True(accepted);
        Assert.False(host.IsRunning);
    }

    [Fact]
    public void LazyAppLoopStrategy_RendersWhenDirtyAndWoken() {
        var host = new AppLifecycleHost();
        var view = new MockAppView();
        using var strategy = new LazyAppLoopStrategy();

        host.Start();

        using var cts = new CancellationTokenSource();
        var thread = new Thread(() => strategy.Run(host, view, cts.Token));
        thread.Start();

        // Initial render occurred
        Thread.Sleep(50);
        Assert.True(view.RenderCount >= 1);
        int initialCount = view.RenderCount;

        // Invalidate view and signal wakeup
        view.Invalidate();
        strategy.SignalWakeup();
        Thread.Sleep(50);
        Assert.True(view.RenderCount > initialCount);

        // Stop host and join thread
        host.Stop();
        strategy.SignalWakeup();
        cts.Cancel();
        thread.Join(500);
    }
}
