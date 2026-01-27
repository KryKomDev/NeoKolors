// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Global;
using NeoKolors.Tui.Rendering;
using NeoKolors.Tui.Styles.Properties;
using NeoKolors.Tui.Styles.Values;
using SkiaSharp;

namespace NeoKolors.Tui.Elements;

public class Image : UniversalElement<SKBitmap> {

    private SKBitmap _image;
    
    public override ElementInfo Info { get; }

    private void OnStyleChanged(IStyleProperty prop) {
        if (prop is ImageSourceProperty isp) {
            UpdateImage(isp.Value);
        }
    }
    
    private void UpdateImage(string path) {
        if (!string.IsNullOrEmpty(path) && File.Exists(path)) {
            _image = SKBitmap.Decode(path);
        }
    }
    
    public ImageDisplayType ImageDisplayType {
        get => _style.Get(new ImageDisplayProperty(ImageDisplayType.FIT)).Value;
        set => _style.Set(new ImageDisplayProperty(value));
    }

    protected virtual ImageDisplayType DefaultImageDisplayType => ImageDisplayType.FIT;

    public ViewSize ImageSize {
        get => _style.Get(new ImageSizeProperty(DefaultImageSize)).Value;
        set => _style.Set(new ImageSizeProperty(value));
    }

    protected virtual ViewSize DefaultImageSize => new(Dimension.Auto, Dimension.Auto);

    public Align ImageAlign {
        get => _style.Get(new ImageAlignProperty(DefaultImageAlign)).Value;
        set => _style.Set(new ImageAlignProperty(value));
    }
    
    protected virtual Align DefaultImageAlign => Align.Center;
    
    public string ImageSource {
        get => _style.Get(new ImageSourceProperty(string.Empty)).Value;
        set => _style.Set(new ImageSourceProperty(value));
    }

    public Image(SKBitmap image, ElementInfo info) {
        _image = image;
        Info   = info;
        _style.StyleChanged += OnStyleChanged;
    }
    
    public Image() : this(new SKBitmap(), new ElementInfo()) { }
    
    public Image(SKBitmap image) : this(image, ElementInfo.Default) {}
    
    public override void Render(ICharCanvas canvas, Rectangle rect) {
        if (!Visible) return;
        
        var layout = ComputeRenderLayout(rect);
        
        var pos = new Point(
            Position.AbsoluteX ? Position.X.ToScalar(rect.Width) : rect.LowerX + Position.X.ToScalar(rect.Width), 
            Position.AbsoluteY ? Position.Y.ToScalar(rect.Width) : rect.LowerY + Position.Y.ToScalar(rect.Height)
        );
        
        if (!BackgroundColor.IsInherit) {
            canvas.Fill(layout.Border - Size.Two + pos + Point.One, ' ');
        }
        
        if (!Border.IsBorderless) {
            canvas.StyleBackground(layout.Border - Size.Two + pos + Point.One, BackgroundColor);
            canvas.PlaceRectangle(layout.Border + pos, Border);
        }
        else {
            canvas.StyleBackground(layout.Border + pos, BackgroundColor);
        }

        var checker = _style.Get<CheckerBckgProperty>().Value;

        if (checker.Enabled) {
            canvas.StyleCheckerBckg(layout.Content + pos, checker);
        }

        var imgSize = ComputeImageSize(new Size(_image.Width, _image.Height), layout.Content, rect.Size);

        var xo = ImageAlign.Horizontal switch {
            HorizontalAlign.LEFT   => 0,
            HorizontalAlign.CENTER => (layout.Content.Width - imgSize.Ch.Width) / 2,
            HorizontalAlign.RIGHT  =>  layout.Content.Width - imgSize.Ch.Width,
            _ => throw new ArgumentOutOfRangeException()
        };

        var yo = ImageAlign.Vertical switch {
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
            ZIndex
        );
    }

    private static Size CharsToPixels(Size chars) {
        var px = ScreenSizeTracker.GetScreenSizePx();
        var ch = ScreenSizeTracker.GetScreenSizeCh();
        var fx = (float)px.Width  / ch.Width;
        var fy = (float)px.Height / ch.Height;
        return new Size((int)(fx * chars.Width), (int)(fy * chars.Height));
    }

    private static SizeF PixelsToChars(Size pixels) {
        var px = ScreenSizeTracker.GetScreenSizePx();
        var ch = ScreenSizeTracker.GetScreenSizeCh();
        var fx = (float)ch.Width  / px.Width;
        var fy = (float)ch.Height / px.Height;
        return new SizeF((int)(fx * pixels.Width), (int)(fy * pixels.Height));
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
        
        var size = ImageDisplayType switch {
            ImageDisplayType.FIT     => ComputeImageSize_Fit(img, content),
            ImageDisplayType.STRETCH => ComputeImageSize_Stretch(content),
            _ => throw new ArgumentOutOfRangeException(nameof(ImageDisplayType), ImageDisplayType, null)
        };

        if (ImageSize.Horizontal.IsNumber) size = size with { Width  = ImageSize.Horizontal.ToScalar(parent.Width)  };
        if (ImageSize.Vertical  .IsNumber) size = size with { Height = ImageSize.Vertical  .ToScalar(parent.Height) };

        return (CharsToPixels(size), size);
    }

    private static Size ComputeImageSize_Fit(SizeF image, Size content) {
        var f = MathF.Min(content.Width / image.Width, content.Height / image.Height);
        return new Size((int)(f * image.Width), (int)(f * image.Height));
    }

    private static Size ComputeImageSize_Stretch(Size content) => content;

    public override Size GetMinSize(Size parent) {
        #if NK_ENABLE_CACHING
        
        // todo: implement caching
        
        #else
        
        return ComputeMinLayout(parent).ElementSize;
        
        #endif
    }

    protected ElementLayout ComputeMinLayout(Size parent) {
        var content = IElement.ComputeLayout(
            parent, Margin, Padding, Border, Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
        
        return IElement.ComputeLayout(ComputeImageSize(new Size(_image.Width, _image.Height), content.Content, parent).Ch,
            parent, Margin, Padding, Border, Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
    }

    public override Size GetMaxSize(Size parent) {
        #if NK_ENABLE_CACHING
        
        // todo: implement caching
        
        #else
        
        return ComputeMaxLayout(parent).ElementSize;
        
        #endif
    }
    
    protected ElementLayout ComputeMaxLayout(Size parent) {
        var content = IElement.ComputeLayout(
            parent, Margin, Padding, Border, Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
        
        return IElement.ComputeLayout(ComputeImageSize(new Size(_image.Width, _image.Height), content.Content, parent).Ch,
            parent, Margin, Padding, Border, Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
    }
    
    public override Size GetRenderSize(Size parent) {
        #if NK_ENABLE_CACHING
        
        // todo: implement caching
        
        #else
        
        return ComputeRenderLayout(parent).ElementSize;
        
        #endif
    }
    
    protected ElementLayout ComputeRenderLayout(Size parent) {
        var content = IElement.ComputeLayout(
            parent, Margin, Padding, Border, Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
        
        return IElement.ComputeLayout(ComputeImageSize(new Size(_image.Width, _image.Height), content.Content, parent).Ch,
            parent, Margin, Padding, Border, Width, Height, MinWidth, MaxWidth, MinHeight, MaxHeight);
    }
    
    public override event Action? OnElementUpdated;
    
    public override SKBitmap GetChildNode() => _image;

    public override void SetChildNode(SKBitmap child) {
        _image = child;
        OnElementUpdated?.Invoke();
    }
}