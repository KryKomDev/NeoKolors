// NeoKolors
// Copyright (c) 2026 KryKom

using System.Diagnostics;

namespace NeoKolors.Tui.Lifecycle.Apps;

/// <summary>
/// A throttled loop strategy that dynamically adjusts target frame rates based on activation and focus states.
/// </summary>
public sealed class ThrottledAppLoopStrategy : IAppLoopStrategy, IDisposable {
    private readonly AutoResetEvent _wakeSignal = new(false);
    private          bool           _disposed;

    /// <summary>
    /// Gets or sets the target frames-per-second when the application is active and focused. Default is 60 FPS.
    /// </summary>
    public int ActiveFps { get; set; } = 60;

    /// <summary>
    /// Gets or sets the target frames-per-second when the application is unfocused or backgrounded. Default is 5 FPS.
    /// </summary>
    public int BackgroundFps { get; set; } = 5;

    /// <summary>
    /// Gets or sets a value indicating whether rendering only occurs when the view is dirty. Default is false.
    /// </summary>
    public bool RenderOnlyWhenDirty { get; set; } = false;

    /// <inheritdoc/>
    public void SignalWakeup() {
        if (!_disposed) {
            _wakeSignal.Set();
        }
    }

    /// <inheritdoc/>
    public void Run(IAppLifecycleHost host, IAppView view, CancellationToken cancellationToken = default) {
        ArgumentNullException.ThrowIfNull(host);
        ArgumentNullException.ThrowIfNull(view);

        var stopwatch = new Stopwatch();

        while (host.IsRunning && !cancellationToken.IsCancellationRequested) {
            if (host.IsPaused || host.ActivationState == AppActivationState.SUSPENDED) {
                Thread.Sleep(50);

                continue;
            }

            stopwatch.Restart();

            int targetFps = host.ActivationState == AppActivationState.ACTIVE 
                ? ActiveFps 
                : BackgroundFps;

            if (targetFps < 1)
                targetFps = 1;

            var targetFrameTime = TimeSpan.FromSeconds(1.0 / targetFps);

            if (!RenderOnlyWhenDirty || view.IsDirty) {
                view.Render();
                view.ResetDirty();
            }

            var elapsed = stopwatch.Elapsed;

            if (elapsed >= targetFrameTime)
                continue;

            var remaining = targetFrameTime - elapsed;
            int      sleepMs   = (int)remaining.TotalMilliseconds;

            if (sleepMs <= 0)
                continue;

            try {
                _wakeSignal.WaitOne(sleepMs);
            }
            catch (ObjectDisposedException) {
                break;
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose() {
        if (_disposed)
            return;

        _disposed = true;
        _wakeSignal.Set();
        _wakeSignal.Dispose();
    }
}