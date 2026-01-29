// NeoKolors
// Copyright (c) 2025 KryKom

using System.Collections;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Styles.Properties;

namespace NeoKolors.Tui.Styles;

public class StyleCollection : IEnumerable<IStyleProperty> {
    
    private readonly Dictionary<Type, IStyleProperty> _styles = new();
    public event StyleChangedHandler StyleChanged = _ => {};

    public IStyleProperty this[Type type] {
        get {
            if (IStyleProperty.IsStyle(type))
                IStyleProperty.ThrowNotStyle(type);
            
            return _styles.TryGetValue(type, out var style) ? style : IStyleProperty.Create(type);
        }
        set {
            if (value.GetType() != type)
                throw new ArgumentException($"Type '{type}' is not the same as the stored expected type '{value.GetType()}'.");
            
            if (IStyleProperty.IsStyle(type))
                IStyleProperty.ThrowNotStyle(type);

            if (IStyleProperty.TryIsPartial(value, out var partial)) {
                _styles[type] = partial.Combine(_styles[type]);
                return;
            }
            
            _styles[type] = value;
            StyleChanged.Invoke(value);
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
        if (!_styles.ContainsKey(typeof(T))) 
            return coalesce;
        
        var s = _styles[typeof(T)];
        if (s is T t) 
            return t;

        return coalesce;
    }
    
    public T Get<T>() where T : IStyleProperty, new() => Get(new T());

    public void Set(IStyleProperty style) {
        if (IStyleProperty.TryIsPartial(style, out var partial)) {
            if (_styles.TryGetValue(partial.BaseType, out var value)) {
                _styles[partial.BaseType] = partial.Combine(value);
            }
            else {
                var created = IStyleProperty.Create(partial.BaseType);
                _styles[partial.BaseType] = partial.Combine(created);
            }
            
            return;
        }
        
        if (_styles.ContainsKey(style.GetType()))
            _styles[style.GetType()] = style;
        else 
            _styles.TryAdd(style.GetType(), style);
        
        StyleChanged.Invoke(style);
    }

    public void Set(StyleCollection other) {
        foreach (var style in other) {
            Set(style);
        }
    }
    
    public IEnumerator<IStyleProperty> GetEnumerator() => _styles.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}