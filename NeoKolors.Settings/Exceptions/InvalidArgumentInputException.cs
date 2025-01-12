//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Exceptions;

public class InvalidArgumentInputException : Exception {
    public InvalidArgumentInputException(string cause) : base($"Tried to set argument to an invalid value. {cause}") { }
}