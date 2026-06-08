// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Events;
using NeoKolors.Console.Input;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A lightweight control designed specifically for displaying read-only or styled ANSI text.
/// Fully replaces legacy Text, Paragraph, and Heading elements.
/// </summary>
public class TextBlock : Control<AnsiString>, IMouseInteractableElement<AnsiString>, INotifyOnRender {
    
    private AnsiString _text = new("");

    public event Action<MouseButton> OnClick = delegate { };
    public event Action<MouseButton> OnRelease = delegate { };
    public event Action OnHover = delegate { };
    public event Action OnHoverOut = delegate { };
    public event OnRenderEventHandler OnRender = delegate { };

    public virtual AnsiString Content {
        get => _text.Clone();
        set {
            if (_text == value) 
                return;
            
            _text = value;
            InvokeElementUpdated();
        }
    }

    public static StyleCollection DefaultStyles { get; } = new(AbstractTextElement.DefaultStyles) {
        ReadOnly = true
    };

    public TextBlock(string text) : base(DefaultStyles) {
        _text = new AnsiString(text);
    }

    public TextBlock(AnsiString text) : base(DefaultStyles) {
        _text = text;
    }

    public TextBlock() : base(DefaultStyles) { }

    protected override Size MeasureOverride(Size availableSize) {
        return _style.Font.GetSize(Content);
    }

    protected override Size ArrangeOverride(Size finalSize) {
        return finalSize;
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var sp = _style.Position;
        var pos = new Point2D(
            sp.AbsoluteX ? sp.X.ToScalar(RenderBounds.Width) : RenderBounds.LowerX + sp.X.ToScalar(RenderBounds.Width),
            sp.AbsoluteY ? sp.Y.ToScalar(RenderBounds.Height) : RenderBounds.LowerY + sp.Y.ToScalar(RenderBounds.Height)
        );
        
        var align = _style.TextAlign;
        
        var styledContent = new AnsiString(Content.Select(c => new AnsiChar(
            c.Char, 
            new NKStyle(
                _style.TextColor.IsInherit || _style.TextColor.IsDefault ? c.Style.FColor : _style.TextColor,
                _style.BackgroundColor,
                c.Style.Styles | _style.TextStyle
            )
        )));
        
        canvas.Fill(RenderLayout.Content + new Point(pos.X, pos.Y), ' ');
        
        _style.Font.PlaceString(
            styledContent,
            canvas,
            RenderLayout.Content + new Point(pos.X, pos.Y),
            align.Horizontal,
            align.Vertical,
            _style.Overflow
        );
        
        OnRender();
    }

    public override ElementInfo Info { get; } = new();

    public override AnsiString GetChildNode() => _text;

    public override void SetChildNode(AnsiString childNode) {
        Content = childNode;
    }
}
