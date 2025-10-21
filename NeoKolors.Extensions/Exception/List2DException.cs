namespace NeoKolors.Extensions.Exception;

public class List2DException : System.Exception {
    private List2DException(string message) : base(message) { }

    public static List2DException AddInvalidRowSize(int expected, int actual) =>
        new($"Could not add a new row. Row had a different length. Expected {expected}, got {actual}.");
    
    public static List2DException AddInvalidColSize(int expected, int actual) =>
        new($"Could not add a new column. Column had a different length. Expected {expected}, got {actual}.");
}