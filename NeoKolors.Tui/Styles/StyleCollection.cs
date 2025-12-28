// NeoKolors
// Copyright (c) 2025 KryKom

using System.Collections;
using NeoKolors.Tui.Styles.Properties;
using IStylePropertyV2 = NeoKolors.Tui.Styles.Properties.IStyleProperty;

namespace NeoKolors.Tui.Styles;

public class StyleCollection : IEnumerable<IStyleProperty> {
    
    private readonly Dictionary<Type, IStylePropertyV2> _styles = new();

    public IStylePropertyV2 this[Type type] {
        get {
            if (IStylePropertyV2.IsStyle(type))
                IStylePropertyV2.ThrowNotStyle(type);
            
            return _styles.TryGetValue(type, out var style) ? style : IStylePropertyV2.Create(type);
        }
        set {
            if (IStylePropertyV2.IsStyle(type))
                IStylePropertyV2.ThrowNotStyle(type);
            
            _styles[type] = value;
        }
    }

    public IStylePropertyV2 this[string name] {
        get {
            var style = IStylePropertyV2.GetByName(name);
            
            return style is null ? throw new ArgumentException($"No style with name '{name}' found.") : this[style];
        }
        set {
            var style = IStylePropertyV2.GetByName(name);
            
            if (style is null) throw new ArgumentException($"No style with name '{name}' found.");
            
            this[style] = value;
        }
    }

    public T Get<T>(T coalesce) where T : IStylePropertyV2, new() {
        if (_styles.ContainsKey(typeof(T))) {
            var s = _styles[typeof(T)];
            if (s is T t) return t;
        }

        return coalesce;
    }
    
    public T Get<T>() where T : IStylePropertyV2, new() => Get(new T());

    public void Set(IStylePropertyV2 style) {
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
    
    public IEnumerator<IStylePropertyV2> GetEnumerator() => _styles.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}