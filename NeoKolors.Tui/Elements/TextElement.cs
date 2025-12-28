//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Styles.Properties;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a text-based elementOld in the NeoKolors TUI framework, inheriting from <see cref="UniversalElement"/>.
/// This class provides properties for styling text, such as font and color settings.
/// </summary>
public abstract class TextElement : UniversalElement {
    
    public virtual IFont Font {
        get => _style.Get(new FontProperty(DefaultFont)).Value;
        set => _style.Set(new FontProperty(value));
    }

    public virtual IFont DefaultFont => IFont.Default;

    public virtual NKColor Color {
        get => _style.Get(new TextColorProperty(DefaultColor)).Value;
        set => _style.Set(new TextColorProperty(value));
    }

    public virtual NKColor DefaultColor => new();
    
    public virtual Align TextAlign {
        get => _style.Get(new TextAlignProperty(DefaultTextAlign)).Value;
        set => _style.Set(new TextAlignProperty(value));
    }

    public virtual Align DefaultTextAlign => new(HorizontalAlign.LEFT, VerticalAlign.TOP);
}