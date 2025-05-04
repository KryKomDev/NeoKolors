//
// NeoKolors 
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Tui.Exceptions;

public class InvalidStyleNameException : Exception {
    private InvalidStyleNameException(string message) : 
        base(message) { }
    
    public static InvalidStyleNameException Duplicate(string name) =>
        new($"Style with name '{name}' already exists."); 
    
    public static InvalidStyleNameException NotFound(string name) => 
        new($"Style with name '{name}' not found.");
}