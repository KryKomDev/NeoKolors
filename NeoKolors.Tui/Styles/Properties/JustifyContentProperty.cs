// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Values;
using JC = NeoKolors.Tui.Styles.Values.JustifyContent;

namespace NeoKolors.Tui.Styles.Properties;

public struct JustifyContentProperty : IStyleProperty<JustifyContent, JustifyContentProperty> {
    public JC Value { get; }
    
    public JustifyContentProperty(JC value) {
        Value = value;
    }

    public JustifyContentProperty() {
        Value = JC.START;
    }
}