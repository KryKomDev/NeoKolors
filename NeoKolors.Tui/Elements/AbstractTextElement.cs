//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a text-based elementOld in the NeoKolors TUI framework, inheriting from <see cref="AbstractElement{T}"/>.
/// This class provides properties for styling text, such as font and color settings.
/// </summary>
public abstract class AbstractTextElement : AbstractElement<AnsiString> {
    
    public static StyleCollection DefaultStyles { get; } = new() {
        Font            = IFont.Default,
        TextColor       = NKColor.Default,
        TextAlign       = new Align(HorizontalAlign.LEFT, VerticalAlign.TOP),
        BackgroundColor = NKColor.Inherit,
        TextStyle       = TextStyles.NONE,
        
        ReadOnly = true
    };

    protected AbstractTextElement(StyleCollection defaultStyle)
        : base(new StyleCollection().Apply(DefaultStyles).Apply(defaultStyle)) { }
    
    protected AbstractTextElement() { }
}