//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Argument;

public class InvalidArgumentInputException : System.Exception {
    public InvalidArgumentInputException(string cause) : base($"Tried to set argument to an invalid value. {cause}") { }
}