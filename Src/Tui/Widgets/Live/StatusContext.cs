// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Widgets.Elements;
using NeoKolors.Tui.Widgets.Lifecycle;

namespace NeoKolors.Tui.Widgets.Live;

/// <summary>
/// Execution context for managing active status spinners and completion states.
/// </summary>
public class StatusContext {
    private readonly StatusElement _element;
    private readonly InlineContext _inlineContext;

    /// <summary>
    /// Gets or sets the active status message.
    /// </summary>
    public string Status {
        get => _element.Text.AsT;
        set {
            _element.Text = value;
            _inlineContext.Invalidate();
        }
    }

    /// <summary>
    /// Gets the total elapsed time since the status started.
    /// </summary>
    public TimeSpan ElapsedTime => _inlineContext.ElapsedTime;

    /// <summary>
    /// Gets the cancellation token for the active operation.
    /// </summary>
    public CancellationToken CancellationToken => _inlineContext.CancellationToken;

    public StatusContext(StatusElement element, InlineContext inlineContext) {
        _element       = element;
        _inlineContext = inlineContext;
    }

    /// <summary>
    /// Marks the status operation as successfully completed with an optional message.
    /// </summary>
    public void Success(string? message = null) {
        if (message != null)
            _element.SuccessText = message;

        _element.IsCompleted = true;
        _inlineContext.Invalidate();
    }

    /// <summary>
    /// Marks the status operation as failed with an optional error message.
    /// </summary>
    public void Fail(string? message = null) {
        if (message != null)
            _element.ErrorText = message;

        _element.IsFailed = true;
        _inlineContext.Invalidate();
    }
}