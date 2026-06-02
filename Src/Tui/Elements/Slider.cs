// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A slider control that allows the user to select from a range of values by moving a slider thumb.
/// Renders like: ═══╡█╞══════════ 30%
/// </summary>
public class Slider : RangeBase {
    
    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Width = Dimension.Chars(20),
        Height = Dimension.Chars(1),
        Border = BorderStyle.Borderless,
        ReadOnly = true
    };

    public Slider() : base(DefaultStyles) { }

    protected override Size MeasureOverride(Size availableSize) {
        return new Size(20, 1);
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;
        var contentPos = pos + RenderLayout.Content.Lower;
        var contentWidth = RenderLayout.Content.Width;

        if (contentWidth <= 6) {
            canvas.Place($"{Value:0}%", contentPos, contentWidth, HorizontalAlign.LEFT);
            return;
        }

        var barWidth = contentWidth - 5;
        double percentage = (Maximum - Minimum) > 0 ? (Value - Minimum) / (Maximum - Minimum) : 0;
        int thumbPos = (int)Math.Round(percentage * (barWidth - 3)); // thumb has size 3: "╡█╞"

        thumbPos = Math.Clamp(thumbPos, 0, barWidth - 3);

        string leftBar = new('═', thumbPos);
        string rightBar = new('═', barWidth - 3 - thumbPos);
        string sliderBar = leftBar + "╡█╞" + rightBar;
        string pctLabel = $" {Value,3:0}%";

        canvas.Place(sliderBar + pctLabel, contentPos, contentWidth, HorizontalAlign.LEFT);
    }

    public override ElementInfo Info { get; } = new();
}
