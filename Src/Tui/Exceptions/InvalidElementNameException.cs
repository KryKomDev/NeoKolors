//
// NeoKolors
// Copyright (c) 2026 KryKom
//

namespace NeoKolors.Tui.Exceptions;

public class InvalidElementNameException : Exception {
    private InvalidElementNameException(string message) : base(message) { }
    
    public static InvalidElementNameException Duplicate(string name) =>
        new($"Element with name '{name}' already exists."); 
    
    public static InvalidElementNameException NotFound(string name) =>
        new($"Element with name '{name}' not found.");
}