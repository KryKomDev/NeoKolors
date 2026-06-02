// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A control that indicates the progress of an operation.
/// Shows [████░░░░] 50% or indeterminate [  ███   ].
/// </summary>
public class ProgressBar : RangeBase {
    
    private bool _isIndeterminate;
    private int _animationStep;

    public bool IsIndeterminate {
        get => _isIndeterminate;
        set {
            if (_isIndeterminate == value) return;
            _isIndeterminate = value;
            InvokeElementUpdated();
        }
    }

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Width = Dimension.Chars(20),
        Height = Dimension.Chars(1),
        Border = BorderStyle.Borderless,
        ReadOnly = true
    };

    public ProgressBar() : base(DefaultStyles) { }

    protected override Size MeasureOverride(Size availableSize) {
        return new Size(20, 1);
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;
        var contentPos = pos + RenderLayout.Content.Lower;
        var contentWidth = RenderLayout.Content.Width;

        if (contentWidth <= 5) {
            canvas.Place($"{Value:0}%", contentPos, contentWidth, HorizontalAlign.LEFT);
            return;
        }

        if (IsIndeterminate) {
            var barWidth = contentWidth - 2;
            var marqueeWidth = Math.Max(1, barWidth / 4);
            _animationStep = (_animationStep + 1) % Math.Max(1, barWidth - marqueeWidth + 1);

            string barContent = new string(' ', _animationStep) + new string('█', marqueeWidth) + new string(' ', Math.Max(0, barWidth - marqueeWidth - _animationStep));
            canvas.Place($"[{barContent}]", contentPos, contentWidth, HorizontalAlign.LEFT);
        }
        else {
            var barWidth = contentWidth - 7;
            double percentage = (Maximum - Minimum) > 0 ? (Value - Minimum) / (Maximum - Minimum) : 0;
            int filledWidth = (int)Math.Round(percentage * barWidth);
            filledWidth = Math.Clamp(filledWidth, 0, barWidth);

            string filled = new('█', filledWidth);
            string empty = new('░', barWidth - filledWidth);
            string pctLabel = $" {Value,3:0}%";

            canvas.Place($"[{filled}{empty}]{pctLabel}", contentPos, contentWidth, HorizontalAlign.LEFT);
        }
    }

    public override ElementInfo Info { get; } = new();
}
