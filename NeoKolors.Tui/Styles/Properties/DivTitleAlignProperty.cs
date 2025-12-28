// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

public struct DivTitleAlignProperty : IStyleProperty<HorizontalAlign, DivTitleAlignProperty> {
    public HorizontalAlign Value { get; }
    public DivTitleAlignProperty(HorizontalAlign value) => Value = value;
    public DivTitleAlignProperty() => Value = HorizontalAlign.LEFT;
}