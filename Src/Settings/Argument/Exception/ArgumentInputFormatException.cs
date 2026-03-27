//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Argument.Exception;

public class ArgumentInputFormatException : FormatException {
    public ArgumentInputFormatException(Type targetType, string sourceValue, string message) 
        : base($"Could not parse source value '{sourceValue}' to {targetType}. {message}") { }
}