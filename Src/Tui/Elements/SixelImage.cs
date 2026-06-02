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
    
    private SKImage? _image;
    private SKBitmap? _bitmap;

    public SKImage? Source {
        get => _image;
        set {
            if (ReferenceEquals(_image, value)) return;
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

    private static Size CharsToPixels(Size chars) {
        var px = ScreenSizeTracker.GetScreenSizePx();
        var ch = ScreenSizeTracker.GetScreenSizeCh();
        
        // default 9x18 ratio if screen size is not available
        var fx = (ch.Width  == 0 || px.Width  == 0) ? 9.0f  : (float)px.Width  / ch.Width;
        var fy = (ch.Height == 0 || px.Height == 0) ? 18.0f : (float)px.Height / ch.Height;
        
        return new Size((int)(fx * chars.Width), (int)(fy * chars.Height));
    }

    private static SizeF PixelsToChars(Size pixels) {
        var px = ScreenSizeTracker.GetScreenSizePx();
        var ch = ScreenSizeTracker.GetScreenSizeCh();
        
        // default 9x18 ratio if screen size is not available
        var fx = (ch.Width  == 0 || px.Width  == 0) ? 1.0f / 9.0f  : (float)ch.Width  / px.Width;
        var fy = (ch.Height == 0 || px.Height == 0) ? 1.0f / 18.0f : (float)ch.Height / px.Height;
        
        return new SizeF(fx * pixels.Width, fy * pixels.Height);
    }

    protected override Size MeasureOverride(Size availableSize) {
        if (_image == null) return Size.Zero;

        var imgCh = PixelsToChars(new Size(_image.Width, _image.Height));
        return new Size((int)imgCh.Width, (int)imgCh.Height);
    }

    protected override void RenderCore(ICharCanvas canvas) {
        if (_bitmap == null) return;
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
