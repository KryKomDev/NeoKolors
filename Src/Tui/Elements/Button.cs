// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

public class Button : Text {

    public new static StyleCollection DefaultStyles { get; } = new(Text.DefaultStyles) {
        ReadOnly = true,
    };
    
    public Button(string text) : base(text, DefaultStyles) { }
    public Button(string text, StyleCollection defaultStyles) : base(text, defaultStyles) { }
}