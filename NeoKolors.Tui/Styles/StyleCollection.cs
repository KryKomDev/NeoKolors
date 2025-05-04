//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Collections;

namespace NeoKolors.Tui.Styles;

public class StyleCollection : IEnumerable<IStyleProperty> {
    
    private readonly IDictionary<string, IStyleProperty> _styles;

    public IStyleProperty this[string name] => Get(name);

    public StyleCollection() {
        _styles = new Dictionary<string, IStyleProperty>();
    }

    public StyleCollection(params IStyleProperty[] styles) {
        _styles = styles.ToDictionary(IStyleProperty.GetName);
    }
    
    public StyleCollection(IDictionary<string, IStyleProperty> styles) {
        _styles = styles;
    }

    public void Set(IStyleProperty style) {
        if (_styles.ContainsKey(IStyleProperty.GetName(style))) {
            _styles[IStyleProperty.GetName(style)] = style;
        }
        else {
            _styles.Add(IStyleProperty.GetName(style), style);
        }
    }
    
    public IStyleProperty Get(string name) {
        return _styles.TryGetValue(name, out var value) 
            ? value 
            : IStyleProperty.GetDefault(name);
    }

    public IEnumerator<IStyleProperty> GetEnumerator() => _styles.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void OverrideWith(StyleCollection other) {
        foreach (var style in other) {
            Set(style);
        }
    }
}