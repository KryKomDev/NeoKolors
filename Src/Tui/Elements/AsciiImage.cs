// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A control that renders lightweight retro text-shading arrays or multi-line ASCII Art files on the standard terminal character grid.
/// </summary>
public class AsciiImage : Control<string> {
    
    private string _asciiSource = string.Empty;

    public string? Source {
        get => _asciiSource;
        set {
            var val = value ?? string.Empty;
            if (_asciiSource == val) return;
            _asciiSource = val;
            InvokeElementUpdated();
        }
    }

    public string Palette { get; set; } = "@#*+-:. ";

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        ReadOnly = true
    };

    public AsciiImage() : base(DefaultStyles) { }

    public AsciiImage(string? asciiArt) : base(DefaultStyles) {
        _asciiSource = asciiArt ?? string.Empty;
    }

    protected override Size2D MeasureOverride(Size2D availableSize) {
        if (string.IsNullOrEmpty(_asciiSource)) return Size2D.Zero;

        var lines = _asciiSource.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
        int maxLen = 0;
        foreach (var line in lines) {
            maxLen = Math.Max(maxLen, line.Length);
        }

        return new Size2D(maxLen, lines.Length);
    }

    protected override void RenderCore(ICharCanvas canvas) {
        if (string.IsNullOrEmpty(_asciiSource)) return;
        var pos = RenderBounds.Lower;

        var lines = _asciiSource.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);
        for (int y = 0; y < Math.Min(lines.Length, RenderLayout.Content.SizeY); y++) {
            var line = lines[y];
            canvas.Place(
                line,
                pos + RenderLayout.Content.Lower + new Point2D(0, y),
                RenderLayout.Content.SizeX,
                HorizontalAlign.LEFT
            );
        }
    }

    public override ElementInfo Info => ElementInfo.Default;

    public override string GetChildNode() => _asciiSource;

    public override void SetChildNode(string childNode) {
        Source = childNode;
    }
}
