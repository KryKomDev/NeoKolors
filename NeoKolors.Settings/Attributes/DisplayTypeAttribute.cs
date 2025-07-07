//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Reflection;

namespace NeoKolors.Settings.Attributes;

/// <summary>
/// Specifies the display type of argument.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class DisplayTypeAttribute : Attribute {
    public string DisplayType { get; }
    
    public DisplayTypeAttribute(string displayType) {
        DisplayType = displayType;
    }

    /// <summary>
    /// Retrieves the display name associated with a specified type. If the type is marked with
    /// the <see cref="DisplayTypeAttribute"/>, its value is returned; otherwise, the type's name is returned.
    /// </summary>
    /// <param name="type">The type for which to retrieve the display name.</param>
    /// <returns>A string representing either the display name defined in the attribute, or the name of the type.</returns>
    public static string GetName(Type type) {
        var a = type.GetCustomAttribute<DisplayTypeAttribute>();
        return a == null ? type.Name : a.DisplayType;
    }

    /// <summary>
    /// Retrieves the display name associated with a specified type parameter. If the type is marked with
    /// the <see cref="DisplayTypeAttribute"/>, its value is returned; otherwise, the type's name is returned.
    /// </summary>
    /// <typeparam name="T">The type parameter for which to retrieve the display name.</typeparam>
    /// <returns>A string representing either the display name defined in the attribute, or the name of the type.</returns>
    public static string GetName<T>() {
        var a = typeof(T).GetCustomAttribute<DisplayTypeAttribute>();
        return a == null ? typeof(T).Name : a.DisplayType;
    }
}