//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Common;
using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Represents a text-based element in the NeoKolors TUI framework, inheriting from <see cref="UniversalElement"/>.
/// This class provides properties for styling text, such as font and color settings.
/// </summary>
public abstract class TextElement : UniversalElement {
    
    public virtual IFont Font {
        get => Style.Get(new FontProperty(IFont.Default)).Font;
        set => Style.Set(new FontProperty(value));
    }

    public virtual NKColor Color {
        get => Style.Get(new ColorProperty()).Value;
        set => Style.Set(new ColorProperty(value));
    }

    public virtual AlignDirection TextAlign {
        get => Style.Get(new TextAlignProperty()).Value;
        set => Style.Set(new TextAlignProperty(value));
    }

    public virtual OverflowType Overflow {
        get => Style.Get(new OverflowProperty()).Value;
        set => Style.Set(new OverflowProperty(value));
    }
}