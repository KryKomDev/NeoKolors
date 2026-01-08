// NeoKolors
// Copyright (c) 2025 KryKom

using System.Collections;
using NeoKolors.Tui.Styles.Properties;

namespace NeoKolors.Tui.Styles;

public class StyleCollection : IEnumerable<IStyleProperty> {
    
    private readonly Dictionary<Type, IStyleProperty> _styles = new();

    public IStyleProperty this[Type type] {
        get {
            if (IStyleProperty.IsStyle(type))
                IStyleProperty.ThrowNotStyle(type);
            
            return _styles.TryGetValue(type, out var style) ? style : IStyleProperty.Create(type);
        }
        set {
            if (IStyleProperty.IsStyle(type))
                IStyleProperty.ThrowNotStyle(type);
            
            _styles[type] = value;
        }
    }

    public IStyleProperty this[string name] {
        get {
            var style = IStyleProperty.GetByName(name);
            
            return style is null ? throw new ArgumentException($"No style with name '{name}' found.") : this[style];
        }
        set {
            var style = IStyleProperty.GetByName(name);
            
            if (style is null) throw new ArgumentException($"No style with name '{name}' found.");
            
            this[style] = value;
        }
    }

    public T Get<T>(T coalesce) where T : IStyleProperty, new() {
        if (_styles.ContainsKey(typeof(T))) {
            var s = _styles[typeof(T)];
            if (s is T t) return t;
        }

        return coalesce;
    }
    
    public T Get<T>() where T : IStyleProperty, new() => Get(new T());

    public void Set(IStyleProperty style) {
        if (_styles.ContainsKey(style.GetType()))
            _styles[style.GetType()] = style;
        else 
            _styles.TryAdd(style.GetType(), style);
    }

    public void Set(StyleCollection other) {
        foreach (var style in other) {
            Set(style);
        }
    }
    
    public IEnumerator<IStyleProperty> GetEnumerator() => _styles.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}