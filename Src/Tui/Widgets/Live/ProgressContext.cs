// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Widgets.Elements;
using NeoKolors.Tui.Widgets.Lifecycle;

namespace NeoKolors.Tui.Widgets.Live;

/// <summary>
/// Execution context for updating an active inline progress bar.
/// </summary>
public class ProgressContext {
    private readonly ProgressBarElement _progressBar;
    private readonly Signal<string>     _descriptionSignal;
    private readonly InlineContext      _inlineContext;

    /// <summary>
    /// Gets or sets the current progress value between 0.0 and 1.0.
    /// </summary>
    public double Value {
        get => _progressBar.Value.AsT;
        set {
            _progressBar.Value = value;
            _inlineContext.Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets the descriptive title/status displayed with the progress bar.
    /// </summary>
    public string Description {
        get => _descriptionSignal.Value;
        set {
            _descriptionSignal.Value = value;
            _inlineContext.Invalidate();
        }
    }

    /// <summary>
    /// Gets the total elapsed time since the progress bar started.
    /// </summary>
    public TimeSpan ElapsedTime => _inlineContext.ElapsedTime;

    /// <summary>
    /// Gets the cancellation token for the active operation.
    /// </summary>
    public CancellationToken CancellationToken => _inlineContext.CancellationToken;

    public ProgressContext(
        ProgressBarElement progressBar,
        Signal<string>     descriptionSignal,
        InlineContext      inlineContext
    ) {
        _progressBar       = progressBar;
        _descriptionSignal = descriptionSignal;
        _inlineContext     = inlineContext;
    }

    /// <summary>
    /// Reports a progress update and immediately requests a screen redraw.
    /// </summary>
    /// <param name="progress">The progress value between 0.0 and 1.0.</param>
    public void Report(double progress) {
        Value = progress;
    }

    /// <summary>
    /// Reports both a progress update and a description update simultaneously.
    /// </summary>
    /// <param name="progress">The progress value between 0.0 and 1.0.</param>
    /// <param name="description">The updated description message.</param>
    public void Report(double progress, string description) {
        _progressBar.Value       = progress;
        _descriptionSignal.Value = description;
        _inlineContext.Invalidate();
    }
}