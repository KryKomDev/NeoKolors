//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Exceptions;

public class StylePropertyValueOutOfRangeException : Exception {
    public StylePropertyValueOutOfRangeException(Type propertyType, object value) : 
        base($"Value '{value}' of property {(propertyType.IsAssignableTo(typeof(IStyleProperty)) 
                 ? IStyleProperty.GetName(propertyType) : propertyType.Name)} is out of range.") { }
}