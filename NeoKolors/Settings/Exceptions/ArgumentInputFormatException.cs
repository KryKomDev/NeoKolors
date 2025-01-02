//
// NeoKolors
// Copyright (c) 2024 KryKom
//

namespace NeoKolors.Settings.Exceptions;

public class ArgumentInputFormatException : FormatException {
    public ArgumentInputFormatException(Type targetType, string sourceValue, string message) 
        : base($"Could not parse source value '{sourceValue}' to {targetType}. {message}") { }
}