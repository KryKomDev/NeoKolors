// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Widgets.Elements;

namespace NeoKolors.Tui.Widgets.Elements;

/// <summary>
/// A high-level declarative element combining an animated spinner and a dynamic status message,
/// with automatic transition to completed or failed states upon task conclusion.
/// </summary>
public class StatusElement : IElement, ITickableElement {
    /// <summary>
    /// Gets the underlying spinner component used during execution.
    /// </summary>
    public SpinnerElement Spinner { get; }

    /// <summary>
    /// Gets or sets the active status message displayed beside the spinner.
    /// </summary>
    public Bindable<string> Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an optional override message displayed when completed successfully.
    /// </summary>
    public Bindable<string?> SuccessText { get; set; } = default;

    /// <summary>
    /// Gets or sets an optional override message displayed when execution fails.
    /// </summary>
    public Bindable<string?> ErrorText { get; set; } = default;

    /// <summary>
    /// Gets or sets whether the operation has successfully completed.
    /// </summary>
    public Bindable<bool> IsCompleted { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the operation encountered an error and failed.
    /// </summary>
    public Bindable<bool> IsFailed { get; set; } = false;

    /// <summary>
    /// Gets or sets the glyph displayed upon successful completion.
    /// Default is <c>"✔"</c>.
    /// </summary>
    public Bindable<string> SuccessIcon { get; set; } = "✔";

    /// <summary>
    /// Gets or sets the glyph displayed upon failure.
    /// Default is <c>"✖"</c>.
    /// </summary>
    public Bindable<string> ErrorIcon { get; set; } = "✖";

    /// <summary>
    /// Gets or sets the styling applied to the success glyph.
    /// </summary>
    public Bindable<NKStyle> SuccessStyle { get; set; } = NKStyle.Default.WithFColor(NKColor.FromRgb(0, 220, 100));

    /// <summary>
    /// Gets or sets the styling applied to the error glyph.
    /// </summary>
    public Bindable<NKStyle> ErrorStyle { get; set; } = NKStyle.Default.WithFColor(NKColor.FromRgb(255, 80, 80));

    /// <summary>
    /// Gets or sets the styling applied to the status message.
    /// </summary>
    public Bindable<NKStyle> TextStyle { get; set; } = NKStyle.Default;

    /// <summary>
    /// Gets or sets the horizontal gap in spaces between the icon/spinner and the message text.
    /// Default is 1 space.
    /// </summary>
    public Bindable<int> Gap { get; set; } = 1;

    /// <summary>
    /// Initializes a new instance of <see cref="StatusElement"/>.
    /// </summary>
    public StatusElement(
        Bindable<string> text         = default,
        SpinnerStyle?    spinnerStyle = null
    ) {
        Spinner = new SpinnerElement(spinnerStyle ?? SpinnerStyle.Dots);

        if (text.Value is not null)
            Text = text;
    }

    /// <inheritdoc/>
    public void Tick(TimeSpan delta) {
        if (!IsCompleted.AsT && !IsFailed.AsT)
            Spinner.Tick(delta);
    }

    /// <inheritdoc/>
    public Size2D Measure(LayoutContext context) {
        var iconWidth = GetIconText().Length;
        var gap       = Math.Max(0, Gap.AsT);
        var textWidth = GetDisplayText().Length;

        return new Size2D(iconWidth + gap + textWidth, 1);
    }

    /// <inheritdoc/>
    public void Render(RenderContext context, Size2D parent) {
        var icon      = GetIconText();
        var iconStyle = GetIconStyle(context);
        var text      = GetDisplayText();
        var textStyle = context.CurrentStyle.With(TextStyle.AsT);
        var gap       = Math.Max(0, Gap.AsT);

        var cells = new List<AnsiChar>(icon.Length + gap + text.Length);

        for (var i = 0; i < icon.Length; i++)
            cells.Add(new AnsiChar(icon[i], iconStyle));

        for (var i = 0; i < gap; i++)
            cells.Add(new AnsiChar(' ', context.CurrentStyle));

        for (var i = 0; i < text.Length; i++)
            cells.Add(new AnsiChar(text[i], textStyle));

        context.Draw(new AnsiString(cells), 0, 0, 0);
    }

    private string GetIconText() {
        if (IsFailed.AsT)
            return ErrorIcon.AsT;

        if (IsCompleted.AsT)
            return SuccessIcon.AsT;

        var frames = Spinner.Style.AsT.Frames;

        return frames.Count > 0 ? frames[Math.Clamp(Spinner.FrameIndex, 0, frames.Count - 1)] : string.Empty;
    }

    private NKStyle GetIconStyle(RenderContext context) {
        if (IsFailed.AsT)
            return context.CurrentStyle.With(ErrorStyle.AsT);

        if (IsCompleted.AsT)
            return context.CurrentStyle.With(SuccessStyle.AsT);

        return context.CurrentStyle.With(Spinner.FrameStyle.AsT);
    }

    private string GetDisplayText() {
        if (IsFailed.AsT && ErrorText.AsT is { } err)
            return err;

        if (IsCompleted.AsT && SuccessText.AsT is { } succ)
            return succ;

        return Text.AsT ?? string.Empty;
    }
}