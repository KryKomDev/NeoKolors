// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Fonts;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

public class Heading : Paragraph {
    
    protected new static StyleCollection DefaultStyles { get; } = new(Paragraph.DefaultStyles) {
        Font = FontAtlas.Get("Future"),

        ReadOnly = true,
    };
    

    public Heading(string text) : base(text, DefaultStyles) { }
    public Heading(string text, StyleCollection defaultStyles) : base(text, defaultStyles) { }
}