namespace NeoKolors.Tui.Fonts;

public interface IGlyph {
    
    /// <summary>
    /// maximal allowed width of a single glyph
    /// </summary>
    public const int MAX_WIDTH = 10;
    
    /// <summary>
    /// maximal allowed height of a single glyph
    /// </summary>
    public const int MAX_HEIGHT = 10;
    
    
    /// <summary>
    /// the character the glyph is supposed to represent
    /// </summary>
    public char Character { get; }
    
    /// <summary>
    /// the width of the glyph
    /// </summary>
    public byte Width { get; }
    
    /// <summary>
    /// the height of the glyph
    /// </summary>
    public byte Height { get; }

    /// <summary>
    /// returns the x-offset of the character, (+) -> right, (-) -> left
    /// </summary>
    public sbyte XOffset { get; }
    
    /// <summary>
    /// returns the y-offset of the character, (+) -> down, (-) -> up
    /// </summary>
    public sbyte YOffset { get; }
    
    /// <summary>
    /// returns the individual characters of the glyph,
    /// the first index represents left / right and the second up / down,
    /// [0, 0] is top left corner,
    /// if a field is <c>\0</c> (NUL), the character is not rendered 
    /// </summary>
    public char[,] Chars { get; }
}