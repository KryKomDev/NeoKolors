// NeoKolors
// Copyright (c) 2026 KryKom

using System.Diagnostics;

namespace NeoKolors.Tui.Widgets.Lifecycle;

/// <summary>
/// Provides execution scope state, timing information, and control methods for an active inline widget session.
/// </summary>
public class InlineContext {
    private readonly InlineLifecycleHost _host;
    private readonly Stopwatch           _stopwatch = Stopwatch.StartNew();

    /// <summary>
    /// Gets or sets whether the widget's final rendered state should remain on the console upon exiting.
    /// If <c>false</c>, the widget lines are completely wiped when execution finishes.
    /// Default is <c>true</c>.
    /// </summary>
    public bool PersistOnExit { get; set; } = true;

    /// <summary>
    /// Gets the total elapsed time since the inline session started.
    /// </summary>
    public TimeSpan ElapsedTime => _stopwatch.Elapsed;

    /// <summary>
    /// Gets the cancellation token associated with this session.
    /// </summary>
    public CancellationToken CancellationToken { get; }

    /// <summary>
    /// Gets or sets arbitrary user state attached to the context.
    /// </summary>
    public object? State { get; set; }

    public InlineContext(InlineLifecycleHost host, CancellationToken cancellationToken = default) {
        _host             = host;
        CancellationToken = cancellationToken;
    }

    /// <summary>
    /// Requests an immediate re-render of the inline widget hierarchy without waiting for the next periodic frame tick.
    /// </summary>
    public void Invalidate() => _host.Invalidate();

    /// <summary>
    /// Requests the inline lifecycle host to stop running and conclude the session.
    /// </summary>
    public void RequestExit() => _host.RequestExit();
}