// NeoKolors
// Copyright (c) 2025 KryKom

using System.Diagnostics.CodeAnalysis;
using Implyzer;
using NeoKolors.Tui.Global;
using BindingFlags = System.Reflection.BindingFlags;

namespace NeoKolors.Tui.Styles.Properties;

public interface IStyleProperty<out TValue, in TSelf> : IStyleProperty where 
    TSelf  : IStyleProperty<TValue, TSelf>, new() where 
    TValue : notnull 
{
    public new TValue Value { get; }
    object IStyleProperty.Value => Value;

    Type IStyleProperty.ValueType => typeof(TValue);

    public static TValue ToTValue(TSelf property) => property.Value;
    
    #if NET8_0_OR_GREATER
    public static virtual implicit operator TValue(TSelf property) => ToTValue(property);
    #endif
}

[ImplType(ImplKind.ValueType)]
public interface IStyleProperty {
    
    public object Value { get; }
    public Type ValueType { get; }

    
    // ---------------------------------- STATIC METHODS ---------------------------------- //
    
    /// <summary>
    /// Creates an instance of an <see cref="IStyleProperty"/> type if the specified type is valid.
    /// </summary>
    /// <param name="style">The type of the style property to create. The type must be a valid, non-abstract,
    /// non-interface type that implements <see cref="IStyleProperty"/> and has a default constructor.</param>
    /// <returns>An instance of the specified <see cref="IStyleProperty"/> type.</returns>
    /// <exception cref="ArgumentException">Thrown when the specified type is not a valid style property or does
    /// not have a default constructor.</exception>
    public static IStyleProperty Create(Type style) {
        if (!IsStyle(style))
            ThrowNotStyle(style);

        var c = style.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
        
        if (c == null) 
            throw new ArgumentException("No default constructor found!");
        
        return (IStyleProperty)c.Invoke(null);
    }

    /// <summary>
    /// Determines whether the specified type is a valid style property type.
    /// </summary>
    /// <param name="t">The type to check. The type must be non-abstract, non-interface, and implement
    /// <see cref="IStyleProperty"/>.</param>
    /// <returns><see langword="true"/> if the specified type is a valid style property type; otherwise,
    /// <see langword="false"/>.</returns>
    public static bool IsStyle(Type t) =>
        t is { IsInterface: false, IsAbstract: false } && typeof(IStyleProperty).IsAssignableFrom(t);

    /// <summary>
    /// Determines whether the specified type is a valid partial style property type.
    /// </summary>
    /// <param name="t">The type to check. The type must be non-abstract, non-interface, implement
    /// <see cref="IStyleProperty"/>, and implement <see cref="IPartialStyleProperty"/>.</param>
    /// <returns><see langword="true"/> if the specified type is a valid partial style property type; otherwise,
    /// <see langword="false"/>.</returns>
    public static bool IsPartial(Type t) =>
        IsStyle(t) && typeof(IPartialStyleProperty).IsAssignableFrom(t);

    /// <summary>
    /// Determines whether the given <see cref="IStyleProperty"/> is a partial style property and, if so,
    /// retrieves it as an <see cref="IPartialStyleProperty"/>.
    /// </summary>
    /// <param name="property">The <see cref="IStyleProperty"/> to check.</param>
    /// <param name="partialProperty">When this method returns, contains the <see cref="IPartialStyleProperty"/>
    /// instance if the given property is partial; otherwise, null.</param>
    /// <returns>True if the given property is a partial style property; otherwise, false.</returns>
    public static bool TryIsPartial(
        IStyleProperty property,
        [NotNullWhen(returnValue: true)] out IPartialStyleProperty? partialProperty) 
    {
        partialProperty = property as IPartialStyleProperty;
        return partialProperty != null;
    }
    
    /// <summary>
    /// Throws an exception if the provided type is not a valid style property.
    /// </summary>
    /// <param name="t">The type to validate. The type must be a non-abstract, non-interface type that implements
    /// <see cref="IStyleProperty"/>.</param>
    /// <exception cref="ArgumentException">Thrown when the specified type does not meet the requirements for being
    /// a valid style property.</exception>
    public static void ThrowNotStyle(Type t) =>
        throw new ArgumentException($"Type {t} is not a style property!");

    /// <summary>
    /// Retrieves the simplified name of a style based on the provided type.
    /// </summary>
    /// <param name="t">The type of the style property. The type must be a valid, non-abstract,
    /// non-interface type that implements <see cref="IStyleProperty"/>.</param>
    /// <returns>The name of the style property with the "Property" suffix removed, if present.</returns>
    /// <exception cref="ArgumentException">Thrown when the specified type is not a valid style property.</exception>
    public static string GetName(Type t) {
        if (!IsStyle(t))
            ThrowNotStyle(t);
        
        string name = t.Name;
        
        if (name.EndsWith("Property")) 
            name = name[..^8];
        
        return name;
    }

    /// <summary>
    /// Retrieves the simplified name of a style based on the provided type.
    /// </summary>
    /// <typeparam name="T">The type of the style property. The type must be a valid, non-abstract,
    /// non-interface type that implements <see cref="IStyleProperty"/>.</typeparam>
    /// <returns>The name of the style property with the "Property" suffix removed, if present.</returns>
    /// <exception cref="ArgumentException">Thrown when the specified type is not a valid style property.</exception>
    public static string GetName<T>() => GetName(typeof(T));

    public static Type? GetByName(string name) {
        return StyleManager.GetTypeByName(name);
    }

    public static bool TryGetByName(string name, [NotNullWhen(returnValue: true)] out Type? type) {
        type = GetByName(name);
        return type != null;
    }
}