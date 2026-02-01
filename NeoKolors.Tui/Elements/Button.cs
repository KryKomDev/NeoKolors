// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Properties;

namespace NeoKolors.Tui.Elements;

public class Button : Text {
    
    public Button(string text) : base(text) { }

    protected override WidthProperty DefaultWidth => new(Dimension.MinContent);
}