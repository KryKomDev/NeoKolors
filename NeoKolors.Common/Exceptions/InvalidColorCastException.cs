namespace NeoKolors.Common.Exceptions;

public class InvalidColorCastException : InvalidCastException {
    private InvalidColorCastException(string message) : base(message) { }
    
    public static InvalidColorCastException CustomToConsoleColor() => new("Cannot cast a custom color to ConsoleColor.");
    public static InvalidColorCastException ConsoleColorToCustom() => new("Cannot cast a ConsoleColor to a custom color.");
}