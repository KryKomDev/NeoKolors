// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Styles.Properties;

public struct DivTitleAlignProperty : IStyleProperty<HorizontalAlign, DivTitleAlignProperty> {
    public HorizontalAlign Value { get; }
    public DivTitleAlignProperty(HorizontalAlign value) => Value = value;
    public DivTitleAlignProperty() => Value = HorizontalAlign.LEFT;
}