namespace NeoKolors.Tui.Exceptions;

public class FontWriterException : Exception {
    private FontWriterException(string message) : base("Could not write font. " + message) { }

    public static FontWriterException InvalidPath() => 
        new("Path is invalid.");
    
    
}