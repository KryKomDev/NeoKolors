// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A declarative UI element that renders a configurable terminal progress bar.
/// </summary>
public class ProgressBarElement : IElement {
    private static readonly char[] SMOOTH_BLOCKS = [' ', '▏', '▎', '▍', '▌', '▋', '▊', '▉'];

    /// <summary>
    /// Gets or sets the current progress value.
    /// Default is 0.0.
    /// </summary>
    public Bindable<double> Value { get; set; } = 0.0;

    /// <summary>
    /// Gets or sets the minimum progress value.
    /// Default is 0.0.
    /// </summary>
    public Bindable<double> Min { get; set; } = 0.0;

    /// <summary>
    /// Gets or sets the maximum progress value.
    /// Default is 1.0.
    /// </summary>
    public Bindable<double> Max { get; set; } = 1.0;

    /// <summary>
    /// Gets or sets an explicit width in columns for the progress bar.
    /// If null, the progress bar fills the available parent width.
    /// </summary>
    public Bindable<int?> Width { get; set; } = default;

    /// <summary>
    /// Gets or sets the character used for completed portions of the bar.
    /// Default is <c>'█'</c>.
    /// </summary>
    public Bindable<char> FilledChar { get; set; } = '█';

    /// <summary>
    /// Gets or sets the character used for uncompleted portions of the bar.
    /// Default is <c>'░'</c>.
    /// </summary>
    public Bindable<char> EmptyChar { get; set; } = '░';

    /// <summary>
    /// Gets or sets whether to display a percentage suffix (e.g. <c>" 75%"</c>).
    /// Default is <c>true</c>.
    /// </summary>
    public Bindable<bool> ShowPercentage { get; set; } = true;

    /// <summary>
    /// Gets or sets the styling applied to the filled portion of the bar.
    /// </summary>
    public Bindable<NKStyle> FilledStyle { get; set; } = NKStyle.Default.WithFColor(NKColor.FromRgb(0, 220, 180));

    /// <summary>
    /// Gets or sets the styling applied to the uncompleted portion of the bar.
    /// </summary>
    public Bindable<NKStyle> EmptyStyle { get; set; } = NKStyle.Default.WithFColor(NKColor.FromRgb(80, 80, 80));

    /// <summary>
    /// Gets or sets the styling applied to the percentage suffix.
    /// </summary>
    public Bindable<NKStyle> PercentageStyle { get; set; } = NKStyle.Default;

    /// <summary>
    /// Gets or sets whether to use sub-cell fractional unicode block characters (1/8 increments) for ultra-smooth filling.
    /// Default is <c>false</c>.
    /// </summary>
    public Bindable<bool> UseSmoothBlocks { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of <see cref="ProgressBarElement"/>.
    /// </summary>
    public ProgressBarElement(
        Bindable<double> value          = default,
        Bindable<double> min            = default,
        Bindable<double> max            = default,
        Bindable<int?>   width          = default,
        Bindable<bool>   showPercentage = default
    ) {
        if (value.Value is not null)
            Value = value;

        if (min.Value is not null)
            Min = min;

        if (max.Value is not null)
            Max = max;

        if (width.Value is not null)
            Width = width;

        if (showPercentage.Value is not null)
            ShowPercentage = showPercentage;
    }

    /// <inheritdoc/>
    public Size2D Measure(LayoutContext context) {
        var w = Width.AsT ?? context.Parent.X;

        return new Size2D(Math.Max(1, w), 1);
    }

    /// <inheritdoc/>
    public void Render(RenderContext context, Size2D parent) {
        var totalWidth = Width.AsT ?? parent.X;

        if (totalWidth <= 0)
            return;

        var min     = Min.AsT;
        var max     = Max.AsT;
        var current = Value.AsT;
        var range   = max - min;
        var ratio   = range > 0.0 ? Math.Clamp((current - min) / range, 0.0, 1.0) : 0.0;

        var showPercent = ShowPercentage.AsT;
        var percentStr  = showPercent ? $" {(int)(ratio * 100),3}%" : string.Empty;
        var barWidth    = Math.Max(1, totalWidth - percentStr.Length);

        var filledStyle  = context.CurrentStyle.With(FilledStyle.AsT);
        var emptyStyle   = context.CurrentStyle.With(EmptyStyle.AsT);
        var percentStyle = context.CurrentStyle.With(PercentageStyle.AsT);

        var cells = new List<AnsiChar>(totalWidth);

        if (UseSmoothBlocks.AsT) {
            var exactFilled = ratio * barWidth;
            var fullBlocks  = (int)exactFilled;
            var remainder   = exactFilled - fullBlocks;
            var fracIndex   = (int)Math.Clamp(remainder * 8, 0, 7);

            for (var i = 0; i < fullBlocks && i < barWidth; i++)
                cells.Add(new AnsiChar(FilledChar.AsT, filledStyle));

            var drawn = fullBlocks;

            if (drawn < barWidth && fracIndex > 0) {
                cells.Add(new AnsiChar(SMOOTH_BLOCKS[fracIndex], filledStyle));
                drawn++;
            }

            for (var i = drawn; i < barWidth; i++)
                cells.Add(new AnsiChar(EmptyChar.AsT, emptyStyle));
        }
        else {
            var filledCount = (int)Math.Round(ratio * barWidth);

            for (var i = 0; i < filledCount && i < barWidth; i++)
                cells.Add(new AnsiChar(FilledChar.AsT, filledStyle));

            for (var i = filledCount; i < barWidth; i++)
                cells.Add(new AnsiChar(EmptyChar.AsT, emptyStyle));
        }

        if (showPercent && percentStr.Length > 0)
            for (var i = 0; i < percentStr.Length; i++)
                cells.Add(new AnsiChar(percentStr[i], percentStyle));

        context.Draw(new AnsiString(cells), 0, 0, 0);
    }
}