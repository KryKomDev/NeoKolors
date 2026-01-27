// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui.Styles.Properties;

internal struct CheckerBckgProperty : IStyleProperty<CheckerBckg, CheckerBckgProperty> {
    public CheckerBckg Value { get; }
    
    public CheckerBckgProperty(CheckerBckg value) {
        Value = value;
    }
    
    public CheckerBckgProperty() {
        Value = new CheckerBckg(NKColor.Default, NKColor.Default, false);
    }
}