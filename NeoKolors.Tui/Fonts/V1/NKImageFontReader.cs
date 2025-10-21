//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.Contracts;
using SkiaSharp;

namespace NeoKolors.Tui.Fonts.V1;

public class NKImageFontReader : IFontReader {
    
    public string HeaderPath { get; }
    private string _imagePath = "";
    
    [Pure]
    public IFont ReadFont() {
        string header = File.ReadAllText(HeaderPath);
        var config = ParseHeader(header);
        string img = Path.Combine(Path.GetDirectoryName(HeaderPath)!, _imagePath);
        return ReadGlyphs(SKBitmap.Decode(img), config);
    }
    
    private IFont ReadGlyphs(SKBitmap bmp, NKImageFontConfig config) {
        var font = config.MissingGlyphMode != MissingGlyphMode.GLYPH 
            ? new NKFont(config.LetterSpacing, config.WordSpacing, config.LineSpacing, config.LineSize, config.MissingGlyphMode) 
            : new NKFont(config.LetterSpacing, config.WordSpacing, config.LineSpacing, config.LineSize, config.SubstituteGlyph);

        string chars = config.GlyphLayout.GetChars();

        float fCols = (bmp.Width + 1) / (config.GlyphWidth + 1f);
        float fRows = (bmp.Height + 1) / (config.GlyphHeight + 1f);

        const float maxDiff = 0.0001f;
        if (Math.Abs(fCols - Math.Round(fCols)) > maxDiff || Math.Abs(fRows - Math.Round(fRows)) > maxDiff) 
            throw FontReaderException.InvalidImageDimensions();

        int i = 0;
        for (int y = 0; y < bmp.Height; y += config.GlyphHeight + 1) {
            for (int x = 0; x < bmp.Width; x += config.GlyphWidth + 1) {
                if (i >= chars.Length) break;
                font.AddGlyph(ReadGlyph(
                    bmp, 
                    new Rectangle(x, y, x + config.GlyphWidth, y + config.GlyphHeight), 
                    chars[i++], 
                    config));
            }
        }
        

        return font;
    }

    protected virtual IGlyph ReadGlyph(SKBitmap bmp, Rectangle r, char c, NKImageFontConfig config) {
        char[,] chars = new char[r.Width, (int)Math.Ceiling(r.Height / 2f)];

        for (int x = 0; x < chars.GetLength(0); x++) {
            for (int y = 0; y < chars.GetLength(1); y++) {
                var c1 = SKColors.Transparent;
                var c2 = SKColors.Transparent;
                
                if (r.Contains(r.LowerX + x, r.LowerY + y * 2)) c1 = bmp.GetPixel(r.LowerX + x, r.LowerY + y * 2);
                if (r.Contains(r.LowerX + x, r.LowerY + y * 2 + 1)) c2 = bmp.GetPixel(r.LowerX + x, r.LowerY + y * 2 + 1);

                if (c1 == SKColors.Transparent && c2 == SKColors.Transparent) {
                    chars[x, y] = '\0';
                }
                else if (c1 == SKColors.White && c2 == SKColors.White) {
                    chars[x, y] = '█';
                }
                else if (c1 == SKColors.White) {
                    chars[x, y] = '▀';
                }
                else if (c2 == SKColors.White) {
                    chars[x, y] = '▄';
                }
                else if (c1 == SKColors.Black || c2 == SKColors.Black) {
                    chars[x, y] = ' ';
                }
            }
        }

        if (config.IsMonospaced) return new NKGlyph(c, chars);
        
        chars = Reduce(chars, out int xDel, out int yDel);
        return new NKGlyph(c, chars, (sbyte)xDel, (sbyte)(-yDel));
    }

    private static char[,] Reduce(char[,] chars, out int xDel, out int yDel) {
        var dyn = List2D<char>.FromArray(chars);
        yDel = 0;
        xDel = 0;

        // reduce left to right
        for (int x = 0; x < dyn.XSize; x++) {
            bool isEmpty = true;
            
            for (int y = 0; y < dyn.YSize; y++) {
                if (dyn[x, y] == '\0') continue;
                isEmpty = false;
                break;
            }

            if (!isEmpty)
                break;
            
            dyn.RemoveX(0);
            x--;
            xDel++;
        }
        
        // reduce top to bottom
        for (int y = 0; y < dyn.YSize; y++) {
            bool isEmpty = true;
            
            for (int x = 0; x < dyn.XSize; x++) {
                if (dyn[x, y] == '\0') continue;
                isEmpty = false;
                break;
            }

            if (!isEmpty)
                break;
            
            dyn.RemoveY(0);
            y--;
            yDel++;
        }

        // reduce right to left
        for (int x = dyn.XSize - 1; x >= 0; x--) {
            bool isEmpty = true;
            
            for (int y = 0; y < dyn.YSize; y++) {
                if (dyn[x, y] == '\0') continue;
                isEmpty = false;
                break;
            }

            if (!isEmpty) break;
            
            dyn.RemoveX(x);
        }

        // reduce bottom to top
        for (int y = dyn.YSize - 1; y >= 0; y--) {
            bool isEmpty = true;
            
            for (int x = 0; x < dyn.XSize; x++) {
                if (dyn[x, y] == '\0') continue;
                isEmpty = false;
                break;
            }
            
            if (!isEmpty) break;
            
            dyn.RemoveY(y);
        }

        return dyn.ToArray();
    }

    private NKImageFontConfig ParseHeader(string raw) {
        string[] lines = raw.Split('\n');
        
        if (lines[0] != "nkif 1") throw FontReaderException.InvalidFileVersion();
        _imagePath = lines[1];
        
        NKImageFontConfig config = new(0, 0, 0, 0, 0, 0);
        
        string[] ugm = lines[2].Split(' ');
        
        switch (ugm) {
            case ["default"]:
                config.MissingGlyphMode = MissingGlyphMode.CHAR;
                break;
            case ["glyph", _]:
                config.MissingGlyphMode = MissingGlyphMode.GLYPH;
                config.SubstituteGlyph = ugm[1][0];
                break;
            case ["skip"]:
                config.MissingGlyphMode = MissingGlyphMode.SKIP;
                break;
            default:
                throw FontReaderException.InvalidUnknownGlyphMode(lines[2]);
        }

        string[] spacings = lines[3].Split(' ');

        if (spacings.Length != 4) throw FontReaderException.InvalidSpacings(lines[3]);

        try {
            config.LetterSpacing = int.Parse(spacings[0]);
        }
        catch (Exception e) {
            throw FontReaderException.InvalidLetterSpacing(e);
        }
        
        try {
            config.WordSpacing = int.Parse(spacings[1]);
        }
        catch (Exception e) {
            throw FontReaderException.InvalidWordSpacing(e);
        }

        try {
            config.LineSpacing = int.Parse(spacings[2]);
        }
        catch (Exception e) {
            throw FontReaderException.InvalidLineSpacing(e);
        }

        try {
            config.LineSize = int.Parse(spacings[3]);
        }
        catch (Exception e) {
            throw FontReaderException.InvalidLineSize(e);
        }

        string[] dims = lines[4].Split(' ');
        if (dims.Length != 2) throw FontReaderException.InvalidGlyphDimensions(lines[4]);

        try {
            config.GlyphWidth = int.Parse(dims[0]);
        }
        catch (Exception e) {
            throw FontReaderException.InvalidGlyphWidth(e);
        }
        
        try {
            config.GlyphHeight = int.Parse(dims[1]);
        }
        catch (Exception e) {
            throw FontReaderException.InvalidGlyphHeight(e);
        }

        try {
            config.IsMonospaced = bool.Parse(lines[5].ToLower().CapitalizeFirst());
        }
        catch (Exception e) {
            throw FontReaderException.InvalidMonospaceMode(e);
        }

        try {
            config.BaseLine = int.Parse(lines[6]);
        }
        catch (Exception e) {
            throw FontReaderException.InvalidBaseLine(e);
        }

        config.GlyphLayout = new GlyphLayout(lines[7]);

        return config;
    }

    public NKImageFontReader(string headerPath) {
        HeaderPath = headerPath;
    }

    /// <summary>
    /// creates a blank font template with the given configuration
    /// </summary>
    /// <param name="path">path to the font</param>
    /// <param name="fontName">name of the font</param>
    /// <param name="config">the configuration of the font</param>
    /// <exception cref="FontWriterException">the path is invalid</exception>
    public static void CreateBlank(string path, string fontName, NKImageFontConfig config) {
        string headerPath = Path.Combine(path, fontName + ".nkf");
        string imagePath = Path.Combine(path, fontName + ".png");
        
        if (!File.Exists(headerPath)) throw FontWriterException.InvalidPath();

        File.Create(headerPath).Close();
        File.Create(imagePath).Close();
        File.WriteAllText(headerPath, $"nkif 1\n{fontName}.png\n{config.ToString()}");

        int cols = (int)Math.Floor(Math.Sqrt(config.GlyphLayout.GlyphCount));
        int rows = (int)Math.Ceiling(config.GlyphLayout.GlyphCount / (float)cols);
        
        var bmp = new SKBitmap(
            width: cols * (config.GlyphWidth + 1) - 1, 
            height: rows * (config.GlyphHeight + 1) - 1);

        for (int x = config.GlyphWidth; x < bmp.Width; x += config.GlyphWidth + 1) {
            for (int y = config.GlyphHeight; y < bmp.Height; y += config.GlyphHeight + 1) {
                bmp.SetPixel(x, y, SKColors.Green);
            }
        }
        
        var image = SKImage.FromBitmap(bmp);
        var d = image.Encode(SKEncodedImageFormat.Png, 100);
        File.WriteAllBytes(imagePath, d.ToArray());
    }
}