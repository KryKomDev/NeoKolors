// NeoKolors
// Copyright (c) 2026 KryKom

using SkiaSharp;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Global;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A control that renders high-fidelity bitmap pixel data utilizing terminal Sixel hardware acceleration.
/// Replaces the legacy Image element.
/// </summary>
public class SixelImage : Control<SKImage> {
    private SKImage?  _image;
    private SKBitmap? _bitmap;

    public SKImage? Source {
        get => _image;
        set {
            if (ReferenceEquals(_image, value))
                return;

            _image = value;
            _bitmap?.Dispose();
            _bitmap = _image != null ? SKBitmap.FromImage(_image) : null;
            InvokeElementUpdated();
        }
    }

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        ReadOnly = true
    };

    public SixelImage(SKImage image) : base(DefaultStyles) {
        Source = image;
    }

    public SixelImage() : base(DefaultStyles) { }

    private static Size2D CharsToPixels(Size2D chars) {
        var px = ScreenSizeTracker.GetScreenSizePx();
        var ch = ScreenSizeTracker.GetScreenSizeCh();

        // default 9x18 ratio if screen size is not available
        var fx = (ch.X == 0 || px.X == 0) ? 9.0f : (float)px.X  / ch.X;
        var fy = (ch.Y == 0 || px.Y == 0) ? 18.0f : (float)px.Y / ch.Y;

        return new Size2D((int)(fx * chars.X), (int)(fy * chars.Y));
    }

    private static SizeF PixelsToChars(Size2D pixels) {
        var px = ScreenSizeTracker.GetScreenSizePx();
        var ch = ScreenSizeTracker.GetScreenSizeCh();

        // default 9x18 ratio if screen size is not available
        var fx = (ch.X  == 0 || px.X  == 0) ? 1.0f / 9.0f : (float)ch.X   / px.X;
        var fy = (ch.Y == 0 || px.Y == 0) ? 1.0f / 18.0f : (float)ch.Y / px.Y;

        return new SizeF(fx * pixels.X, fy * pixels.Y);
    }

    protected override Size2D MeasureOverride(Size2D availableSize) {
        if (_image == null)
            return Size2D.Zero;

        var imgCh = PixelsToChars(new Size2D(_image.Width, _image.Height));

        return new Size2D((int)imgCh.Width, (int)imgCh.Height);
    }

    protected override void RenderCore(ICharCanvas canvas) {
        if (_bitmap == null)
            return;

        var pos = RenderBounds.Lower;

        var pixelSize = CharsToPixels(RenderLayout.Content.Size);

        canvas.PlaceSixel(
            _bitmap,
            pos + RenderLayout.Content.Lower,
            pixelSize,
            RenderLayout.Content.Size,
            _style.ZIndex
        );
    }

    public override ElementInfo Info => ElementInfo.Default;

    public override SKImage GetChildNode() => _image ?? SKImage.Create(new SKImageInfo(1, 1));

    public override void SetChildNode(SKImage child) {
        Source = child;
    }
}