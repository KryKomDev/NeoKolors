//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Collections;
using System.Runtime.CompilerServices;

namespace NeoKolors.Tui.Styles;

/// <summary>
/// Represents a collection of style properties.
/// </summary>
public class StyleCollection : IEnumerable<IStyleProperty> {
    
    private readonly IDictionary<string, IStyleProperty> _styles;
    
    public IStyleProperty this[string name] {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Get(name);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Set(value);
    }

    public StyleCollection() {
        _styles = new Dictionary<string, IStyleProperty>();
    }

    public StyleCollection(params IStyleProperty[] styles) {
        _styles = styles.ToDictionary(IStyleProperty.GetName);
    }
    
    private StyleCollection(IDictionary<string, IStyleProperty> styles) {
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

    /// <summary>
    /// Retrieves a style property from the collection,
    /// returning the specified default if the property is not set explicitly.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the style property, which must implement <see cref="IStyleProperty"/>.
    /// </typeparam>
    /// <param name="coalesce">
    /// The default style property to return if the requested property is not found in the collection.
    /// </param>
    /// <returns>
    /// The style property matching the specified type, or the provided default style property if not found.
    /// </returns>
    public T Get<T>(T coalesce) where T : IStyleProperty {
        return _styles.TryGetValue(IStyleProperty.GetName(coalesce), out var value)
            ? (T)value 
            : coalesce;
    }

    public IEnumerator<IStyleProperty> GetEnumerator() => _styles.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void OverrideWith(StyleCollection other) {
        foreach (var style in other) {
            Set(style);
        }
    }
}