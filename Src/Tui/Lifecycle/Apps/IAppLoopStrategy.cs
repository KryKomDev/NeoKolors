// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Lifecycle.Apps;

/// <summary>
/// Strategy contract for driving the application frame and event loop execution.
/// </summary>
public interface IAppLoopStrategy {
    /// <summary>
    /// Runs the loop until the host stops or cancellation is signaled.
    /// </summary>
    void Run(IAppLifecycleHost host, IAppView view, CancellationToken cancellationToken = default);

    /// <summary>
    /// Signals to the loop that an event or input occurred, waking up lazy loops or scheduling an immediate frame.
    /// </summary>
    void SignalWakeup();
}
