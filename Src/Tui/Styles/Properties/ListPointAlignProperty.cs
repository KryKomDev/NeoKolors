// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Styles.Properties;

public struct ListPointAlignProperty : IStyleProperty<HorizontalAlign, ListPointAlignProperty> {
    public HorizontalAlign Value { get; }

    public ListPointAlignProperty(HorizontalAlign value) {
        Value = value;
    }

    public ListPointAlignProperty(ListPointAlignProperty source) {
        Value = source.Value;
    }

    public ListPointAlignProperty() {
        Value = HorizontalAlign.RIGHT;
    }
}