//
// NeoKolors
// Copyright (c) 2024 KryKom
//

namespace NeoKolors.Settings.Exceptions;

public class InvalidArgumentInputTypeException : Exception {
    public InvalidArgumentInputTypeException(Type source, Type input) 
        : base($"Tried to set argument of type '{source.FullName}' with input of type '{input.FullName}'") { }
}