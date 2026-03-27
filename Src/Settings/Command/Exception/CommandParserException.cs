//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using static NeoKolors.Settings.Command.CommandParser;

namespace NeoKolors.Settings.Command.Exception;

public class CommandParserException : System.Exception {
    private CommandParserException(string message) : base(message) { }
    
    internal static CommandParserException IsEol() => new("Reached end of line.");
    
    internal static CommandParserException UnexpectedToken(string expected, Token actual, int pos) => 
        new($"Unexpected token at {pos}. Expected '{expected}', but got '{actual.Value}'.");
    
    internal static CommandParserException InvalidNodeIdentifier(string actual, int pos) => 
        new($"Invalid node identifier at {pos}. Expected node name, but got '{actual}'.");
}