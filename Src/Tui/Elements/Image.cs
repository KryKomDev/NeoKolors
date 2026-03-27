// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Global;
using NeoKolors.Tui.Rendering;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Properties;
using NeoKolors.Tui.Styles.Values;
using SkiaSharp;

namespace NeoKolors.Tui.Elements;

public class Image : AbstractElement<SKBitmap> {

    private SKBitmap _image;
    
    public override ElementInfo Info { get; }

    private void OnStyleChanged(IStyleProperty prop) {
        if (prop is ImageSourceProperty isp) {
            UpdateImage(isp.Value);
        }
    }
    
    private void UpdateImage(string path) {
        if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
            var bmp = SKBitmap.Decode(path);
            if (bmp != null) {
                _image = bmp;
            }
        }
    }

    public Image(SKBitmap image, ElementInfo info) {
        _image = image;
        Info   = info;
        _style.StyleChanged += OnStyleChanged;
    }
    
    public Image() : this(new SKBitmap(), new ElementInfo()) { }
    
    public Image(SKBitmap image) : this(image, ElementInfo.Default) {}
    
    public override void Render(ICharCanvas canvas, Rectangle rect) {
        if (!_style.Visible) return;
        
        var layout = ComputeRenderLayout(rect);
        
        var pos = new Point(
            _style.Position.AbsoluteX ? _style.Position.X.ToScalar(rect.Width) : rect.LowerX + _style.Position.X.ToScalar(rect.Width), 
            _style.Position.AbsoluteY ? _style.Position.Y.ToScalar(rect.Height) : rect.LowerY + _style.Position.Y.ToScalar(rect.Height)
        );
        
        if (!_style.BackgroundColor.IsInherit) {
            canvas.Fill(layout.Border - Size.Two + pos + Point.One, new AnsiChar(' ', new NKStyle()));
        }
        
        if (!_style.Border.IsBorderless) {
            if (!_style.BackgroundColor.IsInherit)
                canvas.StyleBackground(layout.Border - Size.Two + pos + Point.One, _style.BackgroundColor);
            canvas.PlaceRectangle(layout.Border + pos, _style.Border);
        }
        else {
            if (!_style.BackgroundColor.IsInherit)
                canvas.StyleBackground(layout.Border + pos, _style.BackgroundColor);
        }

        var checker = _style.Get<CheckerBckgProperty>().Value;

        if (checker.Enabled) {
            canvas.StyleCheckerBckg(layout.Content + pos, checker);
        }

        var imgSize = ComputeImageSize(new Size(_image.Width, _image.Height), layout.Content, rect.Size);

        var xo = _style.ImageAlign.Horizontal switch {
            HorizontalAlign.LEFT   => 0,
            HorizontalAlign.CENTER => (layout.Content.Width - imgSize.Ch.Width) / 2,
            HorizontalAlign.RIGHT  =>  layout.Content.Width - imgSize.Ch.Width,
            _ => throw new ArgumentOutOfRangeException()
        };

        var yo = _style.ImageAlign.Vertical switch {
            VerticalAlign.TOP    => 0,
            VerticalAlign.CENTER => (layout.Content.Height - imgSize.Ch.Height) / 2,
            VerticalAlign.BOTTOM => layout.Content.Height - imgSize.Ch.Height,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        canvas.PlaceSixel(
            _image,
            pos + layout.Content.Lower + new Point(xo, yo), 
            imgSize.Px,
            imgSize.Ch,
            _style.ZIndex
        );
    }

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

    /// <summary>
    /// Computes the appropriate size (in chars) for an image based on the content bounds
    /// and the specified image display type.
    /// </summary>
    /// <param name="image">The current size of the image.</param>
    /// <param name="content">The dimensions of the content area available for rendering the image.</param>
    /// <param name="parent">The parent element size.</param>
    /// <returns>The computed size of the image that fits within the content area based on the specified display type.</returns>
    private (Size Px, Size Ch) ComputeImageSize(Size image, Size content, Size parent) {
        var img = PixelsToChars(image);
        
        var size = _style.ImageDisplayType switch {
            ImageDisplayType.FIT     => ComputeImageSize_Fit(img, content),
            ImageDisplayType.STRETCH => ComputeImageSize_Stretch(content),
            _ => throw new ArgumentOutOfRangeException(nameof(ImageDisplayType),_style. ImageDisplayType, null)
        };

        if (_style.ImageSize.Horizontal.IsNumber) size = size with { Width  = _style.ImageSize.Horizontal.ToScalar(parent.Width)  };
        if (_style.ImageSize.Vertical  .IsNumber) size = size with { Height = _style.ImageSize.Vertical  .ToScalar(parent.Height) };

        return (CharsToPixels(size), size);
    }

    private static Size ComputeImageSize_Fit(SizeF image, Size content) {
        if (image.Width <= 0 || image.Height <= 0) return Size.Zero;
        var f = MathF.Min(content.Width / image.Width, content.Height / image.Height);
        return new Size((int)(f * image.Width), (int)(f * image.Height));
    }

    private static Size ComputeImageSize_Stretch(Size content) => content;

    public override Size GetMinSize(Size parent) {
        #if NK_ENABLE_CACHING
        
        // todo: implement caching
        
        #else
        
        return ComputeMinLayout(parent).Margin;
        
        #endif
    }

    protected ElementLayout ComputeMinLayout(Size parent) {
        var content = IElement.ComputeLayoutFromBounds(
            parent, 
            _style.Margin,    _style.Border, _style.Padding, 
            _style.Width,     _style.Height, 
            _style.MinWidth,  _style.MaxWidth, 
            _style.MinHeight, _style.MaxHeight
        );
        
        return IElement.ComputeLayoutFromContent(
            ComputeImageSize(new Size(_image.Width, _image.Height), content.Content, parent).Ch,
            parent,
            _style.Margin,    _style.Border, _style.Padding, 
            _style.Width,     _style.Height, 
            _style.MinWidth,  _style.MaxWidth, 
            _style.MinHeight, _style.MaxHeight
        );
    }

    public override Size GetMaxSize(Size parent) {
        #if NK_ENABLE_CACHING
        
        // todo: implement caching
        
        #else
        
        return ComputeMaxLayout(parent).Margin;
        
        #endif
    }
    
    protected ElementLayout ComputeMaxLayout(Size parent) {
        var content = IElement.ComputeLayoutFromBounds(
            parent, 
            _style.Margin,    _style.Border, _style.Padding, 
            _style.Width,     _style.Height, 
            _style.MinWidth,  _style.MaxWidth, 
            _style.MinHeight, _style.MaxHeight
        );
        
        return IElement.ComputeLayoutFromContent(
            ComputeImageSize(new Size(_image.Width, _image.Height), content.Content, parent).Ch,
            parent,
            _style.Margin,    _style.Border, _style.Padding,
            _style.Width,     _style.Height,
            _style.MinWidth,  _style.MaxWidth, 
            _style.MinHeight, _style.MaxHeight
        );
    }
    
    public override Size GetRenderSize(Size parent) {
        #if NK_ENABLE_CACHING
        
        // todo: implement caching
        
        #else
        
        return ComputeRenderLayout(parent).Margin;
        
        #endif
    }
    
    protected ElementLayout ComputeRenderLayout(Size parent) {
        var content = IElement.ComputeLayoutFromBounds(
            parent, 
            _style.Margin,    _style.Border, _style.Padding, 
            _style.Width,     _style.Height, 
            _style.MinWidth,  _style.MaxWidth, 
            _style.MinHeight, _style.MaxHeight
        );
        
        return IElement.ComputeLayoutFromContent(
            ComputeImageSize(new Size(_image.Width, _image.Height), content.Content, parent).Ch,
            parent,
            _style.Margin,    _style.Border, _style.Padding,
            _style.Width,     _style.Height,
            _style.MinWidth,  _style.MaxWidth, 
            _style.MinHeight, _style.MaxHeight
        );
    }
    
    public override event Action? OnElementUpdated;
    
    public override SKBitmap GetChildNode() => _image;

    public override void SetChildNode(SKBitmap child) {
        _image = child;
        OnElementUpdated?.Invoke();
    }
}