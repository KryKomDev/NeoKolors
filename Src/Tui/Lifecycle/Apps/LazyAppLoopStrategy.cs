// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Apps;

/// <summary>
/// A zero-CPU idle loop strategy that sleeps until user input, window events, or explicit invalidations wake it up.
/// </summary>
public sealed class LazyAppLoopStrategy : IAppLoopStrategy, IDisposable {
    private readonly AutoResetEvent _wakeSignal = new(false);
    private bool _disposed;

    /// <summary>
    /// Gets or sets an optional periodic heartbeat timeout (e.g. for background timers or cursor blinking).
    /// Default is <c>null</c> (wait indefinitely until signaled).
    /// </summary>
    public TimeSpan? HeartbeatInterval { get; set; }

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

        // Initial render on start
        if (host.IsRunning) {
            view.Render();
            view.ResetDirty();
        }

        while (host.IsRunning && !cancellationToken.IsCancellationRequested) {
            if (host.IsPaused) {
                // Sleep while paused
                Thread.Sleep(50);
                continue;
            }

            int timeoutMs = HeartbeatInterval.HasValue ? (int)HeartbeatInterval.Value.TotalMilliseconds : Timeout.Infinite;

            try {
                // Wait for input or invalidation signal
                _wakeSignal.WaitOne(timeoutMs);
            } catch (ObjectDisposedException) {
                break;
            }

            if (!host.IsRunning || cancellationToken.IsCancellationRequested) {
                break;
            }

            // Only render if dirty or invalidation occurred
            if (view.IsDirty) {
                view.Render();
                view.ResetDirty();
            }
        }
    }

    /// <inheritdoc/>
    public void Dispose() {
        if (!_disposed) {
            _disposed = true;
            _wakeSignal.Set();
            _wakeSignal.Dispose();
        }
    }
}
