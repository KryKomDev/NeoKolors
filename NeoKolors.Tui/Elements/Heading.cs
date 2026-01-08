// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Fonts;

namespace NeoKolors.Tui.Elements;

public class Heading : Paragraph {
    protected override IFont DefaultFont => FontAtlas.Get("Future");

    public Heading(string text) : base(text) { }
}