//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Command.Exception;

public class CommandException : System.Exception {
    public CommandException(string? message) : base(message) { }
    
    public static CommandException NoResultConstructor() => 
        new($"Could not return result of command. Result builder is not initialized.");
}