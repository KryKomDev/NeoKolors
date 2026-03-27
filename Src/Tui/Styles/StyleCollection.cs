// NeoKolors
// Copyright (c) 2026 KryKom

using System.Collections;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Styles.Properties;

namespace NeoKolors.Tui.Styles;

public class StyleCollection : IEnumerable<IStyleProperty> {
    
    private readonly Dictionary<Type, IStyleProperty> _styles;
    private readonly StyleCollection? _defaultValues;

    /// An event that is triggered whenever a style property within the StyleCollection is changed.
    /// The event provides the updated style property as its parameter.
    /// This event allows subscribers to respond to changes in the StyleCollection,
    /// which can be used for updating UI elements, applying new styles, or handling other
    /// interactions dependent on style updates.
    /// The delegate type for this event is `StyleChangedHandler`, which accepts a single parameter
    /// of type `IStyleProperty`.
    public event StyleChangedHandler StyleChanged;

    /// Specifies whether the style collection is read-only.
    /// When set to true, the collection cannot be modified,
    /// preventing changes to its style properties. This property
    /// is useful for defining immutable style templates that can
    /// be shared across multiple elements without the risk of alteration.
    public bool ReadOnly { get; init; }
    
    public StyleCollection(StyleCollection? defaultValues) {
        _styles        = new Dictionary<Type, IStyleProperty>();
        StyleChanged   = _ => { };
        _defaultValues = defaultValues;
    }

    public StyleCollection() {
        _styles        = new Dictionary<Type, IStyleProperty>();
        StyleChanged   = _ => { };
        _defaultValues = null;
    }

    /// <summary>
    /// Gets or sets the underlying style found by the specified type. If this instance does not contain
    /// the style, a style of the same type will be queried within the defaults.
    /// </summary>
    /// <param name="type">The type of the style to be retrieved or set.</param>
    /// <exception cref="ArgumentException">The set style type is different from the specified type.</exception>
    public IStyleProperty this[Type type] {
        get {
            if (IStyleProperty.TryIsStyle(type, out var s))
                IStyleProperty.ThrowNotStyle(type);

            if (IStyleProperty.TryIsPartial(s!, out var partial))
                return partial.Extract(this[partial.BaseType]);
            
            return _styles.TryGetValue(type, out var style) ? style : GetDefault(type);
        }
        set {
            if (ReadOnly) return;
         
            if (value.GetType() != type)
                throw new ArgumentException(
                    $"Type '{type}' is not the same as the stored expected type '{value.GetType()}'.");
            
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

    /// <summary>
    /// Gets or sets the underlying style found by the specified style name.
    /// </summary>
    /// <param name="name">The name of the style.</param>
    /// <exception cref="ArgumentException">A style with the specified name does not exist.</exception>
    public IStyleProperty this[string name] {
        get {
            var style = IStyleProperty.GetByName(name);
            
            return style is null ? throw new ArgumentException($"No style with name '{name}' found.") : this[style];
        }
        set {
            if (ReadOnly) return;
            
            var style = IStyleProperty.GetByName(name);
            
            if (style is null) throw new ArgumentException($"No style with name '{name}' found.");
            
            this[style] = value;
        }
    }

    /// <summary>
    /// Retrieves a style property of the specified type from the style collection.
    /// If the property does not exist, a default value is returned.
    /// </summary>
    /// <typeparam name="T">The type of the style property to retrieve.</typeparam>
    /// <param name="coalesce">The default value to return if the property is not found.</param>
    /// <returns>The retrieved style property of the specified type, or the default value if not found.</returns>
    public T Get<T>(T coalesce) where T : IStyleProperty, new() {
        if (!_styles.ContainsKey(typeof(T))) {
            if (_defaultValues is null || 
                !_defaultValues._styles.TryGetValue(typeof(T), out var style) || 
                style is not T d) 
            {
                return coalesce;
            }

            return d;
        }

        var s = _styles[typeof(T)];
        if (s is T t) 
            return t;

        return coalesce;
    }

    /// <summary>
    /// Retrieves an instance of the specified style property type from the style collection.
    /// If the property is not present in the collection, a new default instance is created and returned.
    /// </summary>
    /// <typeparam name="T">The type of the style property to retrieve. Must implement <see cref="IStyleProperty"/>.</typeparam>
    /// <returns>The requested style property of the specified type, or a new default instance if not found.</returns>
    public T Get<T>() where T : IStyleProperty, new() => Get(new T());

    /// <summary>
    /// Sets the inputted style within the style collection.
    /// </summary>
    /// <param name="style">The style to be set.</param>
    public void Set(IStyleProperty style) {
        if (ReadOnly) return;
       
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

    /// <summary>
    /// Applies the styles from the provided <see cref="StyleCollection"/> to the current collection.
    /// </summary>
    /// <param name="other">The <see cref="StyleCollection"/> containing the styles to be applied.</param>
    /// <returns>The current <see cref="StyleCollection"/> with the applied styles.</returns>
    public StyleCollection Apply(StyleCollection other) {
        if (ReadOnly) return this;

        foreach (var style in other) {
            Set(style);
        }

        return this;
    }
    
    public IEnumerator<IStyleProperty> GetEnumerator() => _styles.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private IStyleProperty GetDefault(Type type) 
        => _defaultValues is null ? IStyleProperty.Create(type) : _defaultValues[type];
}