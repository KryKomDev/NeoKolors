// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

public class Paragraph : Text {
    
    protected new static StyleCollection DefaultStyles { get; } = new(Text.DefaultStyles) {
        Width = Dimension.Stretch,

        ReadOnly = true,
    };
    
    public Paragraph(string text) : base(text, DefaultStyles) { }
    public Paragraph(string text, StyleCollection defaultStyles) : base(text, defaultStyles) { }
}