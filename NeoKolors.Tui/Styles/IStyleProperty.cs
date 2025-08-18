//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Reflection;
using System.Runtime.CompilerServices;
using NeoKolors.Common.Util;
using NeoKolors.Console;
using NeoKolors.Tui.Exceptions;
using NeoKolors.Tui.SourceManagement;

namespace NeoKolors.Tui.Styles;

/// <summary>
/// base class for style properties
/// </summary>
/// <typeparam name="TSelf">the implementing struct itself</typeparam>
/// <typeparam name="TValue">the type of the value it holds</typeparam>
public interface IStyleProperty<TSelf, TValue> : 
    IStyleProperty
    where TSelf : struct, IStyleProperty<TSelf, TValue> 
    where TValue : notnull 
{
    public new TValue Value { get; }
    object IStyleProperty.Value => Value;
}

/// <summary>
/// totally base class for style properties
/// </summary>
public interface IStyleProperty {

    private static readonly NKLogger LOGGER = NKDebug.GetLogger<IStyleProperty>();
    
    public object Value { get; }
    
    /// <summary>
    /// returns the name of the style property
    /// </summary>
    public static string GetName(IStyleProperty property) {
        var type = property.GetType();
        var attr = type.GetCustomAttribute<StylePropertyNameAttribute>(false);
        return attr is null ? type.Name.PascalToKebab() : attr.Name;
    }
    
    /// <summary>
    /// returns the name of the style property
    /// </summary>
    /// <param name="type">a style property's type</param>
    public static string GetName(Type type) {
        var attr = type.GetCustomAttribute<StylePropertyNameAttribute>(false);
        return attr is null ? type.Name.PascalToKebab() : attr.Name;
    }

    public static IStyleProperty GetDefault(string name) {
        var t = GetType(name);
        var ctor = t.GetConstructor([]);

        if (ctor == null) {
            throw new NotImplementedException($"No default constructor for style property '{name}' found.");
        }
        
        return (IStyleProperty)ctor.Invoke([]);
    }

    public static Type GetType(string name) {
        try {
            return SourceManager.GetStyle(name);
        }
        catch (InvalidStyleNameException) {
            LOGGER.Crit("Style with name '{0}' not found.", name);
            throw InvalidStyleNameException.NotFound(name);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsStyle(Type t) =>
        t is { IsInterface: false, IsAbstract: false } && typeof(IStyleProperty).IsAssignableFrom(t);
}