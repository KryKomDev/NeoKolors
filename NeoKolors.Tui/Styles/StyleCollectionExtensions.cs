// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Properties;

namespace NeoKolors.Tui.Styles;

public static partial class StyleCollectionExtensions {
    extension(StyleCollection c) {
        public void Apply(params IStyleProperty[] properties) {
            foreach (var property in properties) {
                c.Set(property);
            }
        }
    }
}