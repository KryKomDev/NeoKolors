//
// NeoKolors
// Copyright (c) 2024 KryKom
//

namespace NeoKolors.ConsoleGraphics.Settings.Exceptions;

public class TypeNotEnumException : Exception {
    public TypeNotEnumException(Type type) : base($"Type '{type.Name}' is not a valid enum type.") { }    
}