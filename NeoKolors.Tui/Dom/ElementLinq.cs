// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Elements;
using NeoKolors.Tui.Styles.Properties;

namespace NeoKolors.Tui.Dom;

public static class ElementLinq {
    
    extension(IEnumerable<IElement> elements) {
        
        public void Apply(params IStyleProperty[] styles) {
            foreach (var element in elements) {
                foreach (var styleProperty in styles) {
                    element.Style.Set(styleProperty);
                }
            }
        }

        public IEnumerable<IElement> OfClass(string className) {
            foreach (var element in elements) {
                if (element.Info.Classes.Contains(className))
                    yield return element;
            }
        }

        public IEnumerable<IElement> OfType<T>() where T : IElement => elements.OfType(typeof(T));
        
        public IEnumerable<IElement> OfType(Type type) {
            foreach (var element in elements) {
                if (element.Info.GetType() == type)
                    yield return element;
            }
        }
    }
}