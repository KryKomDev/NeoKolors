//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Common.Exceptions;

public class InvalidColorCastException : InvalidCastException {
    public InvalidColorCastException(string message) : base(message) { }
    
    public static InvalidColorCastException CustomToConsoleColor() => 
        new("Cannot cast a custom color to ConsoleColor.");
    
    public static InvalidColorCastException ConsoleColorToCustom() => 
        new("Cannot cast a ConsoleColor to a custom color.");
    
    public static InvalidColorCastException DefaultOrInheritToConsoleColor() => 
        new("Cannot cast Default/Inherit color to ConsoleColor.");
    
    public static InvalidColorCastException NKToSystem(NKConsoleColor color) => 
        new($"Cannot convert the NKConsoleColor.{color} to a System.ConsoleColor.");
    
    public static InvalidColorCastException DefaultToRgb() =>
        new("Cannot convert the default color to a RGB color.");
    
    public static InvalidColorCastException InheritToRgb() =>
        new("Cannot convert the inherit color to a RGB color.");
    
    public static InvalidColorCastException DefaultToConsole() =>
        new("Cannot convert the default color to a console color.");
    
    public static InvalidColorCastException InheritToConsole() =>
        new("Cannot convert the inherit color to a console color.");
    
    public static InvalidColorCastException RgbToConsole() =>
        new("Cannot convert a RGB color to a console color.");

    public static InvalidColorCastException ConsoleToRgb() =>
        new("Cannot convert a console color to a RGB color.");
}