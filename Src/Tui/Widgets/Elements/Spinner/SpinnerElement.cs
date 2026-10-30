// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Elements;

namespace NeoKolors.Tui.Widgets.Elements;

/// <summary>
/// A declarative UI element that renders an animated terminal spinner.
/// </summary>
public class SpinnerElement : IElement, ITickableElement {
    private TimeSpan _accumulatedTime;

    /// <summary>
    /// Gets or sets the spinner animation style preset.
    /// Default is <see cref="SpinnerStyle.Dots"/>.
    /// </summary>
    public Bindable<SpinnerStyle> Style { get; set; } = SpinnerStyle.Dots;

    /// <summary>
    /// Gets or sets the styling applied to each spinner frame.
    /// </summary>
    public Bindable<NKStyle> FrameStyle { get; set; } = NKStyle.Default;

    /// <summary>
    /// Gets or sets the index of the currently active animation frame.
    /// </summary>
    public int FrameIndex { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="SpinnerElement"/>.
    /// </summary>
    public SpinnerElement(
        Bindable<SpinnerStyle> style      = default,
        Bindable<NKStyle>      frameStyle = default
    ) {
        if (style.Value is not null)
            Style = style;

        if (frameStyle.Value is not null)
            FrameStyle = frameStyle;
    }

    /// <summary>
    /// Advances the spinner by one frame manually.
    /// </summary>
    public void Step() {
        var frames = Style.AsT.Frames;

        if (frames.Count > 0)
            FrameIndex = (FrameIndex + 1) % frames.Count;
    }

    /// <inheritdoc/>
    public void Tick(TimeSpan delta) {
        var style = Style.AsT;

        if (style.Frames.Count == 0 || style.Interval <= TimeSpan.Zero)
            return;

        _accumulatedTime += delta;

        if (_accumulatedTime >= style.Interval) {
            var steps = (int)(_accumulatedTime.Ticks / style.Interval.Ticks);
            FrameIndex       =  (FrameIndex + steps) % style.Frames.Count;
            _accumulatedTime -= TimeSpan.FromTicks(steps * style.Interval.Ticks);
        }
    }

    /// <inheritdoc/>
    public Size2D Measure(LayoutContext context) {
        var frames = Style.AsT.Frames;

        if (frames.Count == 0)
            return Size2D.Zero;

        var frame = frames[Math.Clamp(FrameIndex, 0, frames.Count - 1)];

        return new Size2D(frame.Length, 1);
    }

    /// <inheritdoc/>
    public void Render(RenderContext context, Size2D parent) {
        var frames = Style.AsT.Frames;

        if (frames.Count == 0)
            return;

        var frame = frames[Math.Clamp(FrameIndex, 0, frames.Count - 1)];
        var style = context.CurrentStyle.With(FrameStyle.AsT);
        context.Draw(new AnsiString(frame, style), 0, 0, 0);
    }
}