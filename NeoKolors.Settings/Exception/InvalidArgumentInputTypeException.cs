//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Exception;

public class InvalidArgumentInputTypeException : System.Exception {
    public InvalidArgumentInputTypeException(Type expected, Type actual) 
        : base($"Tried to set argument of type '{expected.FullName}' with input of type '{actual.FullName}'") { }
}