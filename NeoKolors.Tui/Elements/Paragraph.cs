// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Properties;

namespace NeoKolors.Tui.Elements;

public class Paragraph : Text {
    public Paragraph(string text) : base(text) { }

    protected override WidthProperty DefaultWidth => new(Dimension.Percent(100));
}