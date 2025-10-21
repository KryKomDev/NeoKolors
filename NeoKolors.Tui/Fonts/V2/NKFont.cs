// NeoKolors
// Copyright (c) 2025 KryKom

using System.Runtime.CompilerServices;
using NeoKolors.Tui.Fonts.V2.Exceptions;
using static NeoKolors.Tui.Fonts.V2.NKFontStringTokenizer;

namespace NeoKolors.Tui.Fonts.V2;

using SimpleGlyphSet = Dictionary<char, SimpleGlyphInfo>;
using LigatureGlyphSet = Dictionary<string, LigatureGlyphInfo>;
using AutoCompoundGlyphSet = Dictionary<char, AutoCompoundGlyphInfo>;

using GlyphSetTriple = (
    Dictionary<char, SimpleGlyphInfo>, 
    Dictionary<string, LigatureGlyphInfo>, 
    Dictionary<char, AutoCompoundGlyphInfo>
);

public class NKFont : IFont {

    /// <summary>
    /// A collection of simple glyph information used by the font.
    /// </summary>
    /// <remarks>
    /// The <c>SimpleGlyphs</c> property represents a set of basic glyphs associated with single characters.
    /// These glyphs do not involve ligatures or composition and are used for straightforward character rendering.
    /// </remarks>
    private readonly SimpleGlyphSet _simpleGlyphs;

    /// <summary>
    /// A collection of ligature glyph information used by the font.
    /// </summary>
    /// <remarks>
    /// The <c>LigatureGlyphs</c> property represents a set of glyphs that correspond to ligatures,
    /// which are specific character combinations rendered as a single glyph. These are used to
    /// enhance the typographic appearance and improve character composition in text rendering.
    /// </remarks>
    private readonly LigatureGlyphSet _ligatureGlyphs;

    /// <summary>
    /// A collection of auto-compound glyph information used by the font.
    /// </summary>
    /// <remarks>
    /// The <c>AutoCompoundGlyphs</c> property represents a set of glyphs that are created
    /// through the automatic combination of a base glyph with another glyph, based on predefined alignment settings.
    /// These glyphs allow for the dynamic composition of characters or symbols during rendering.
    /// </remarks>
    private readonly AutoCompoundGlyphSet _autoCompoundGlyphs;

    /// <summary>
    /// Represents the glyph used as a fallback when an invalid or unrecognized glyph is encountered.
    /// </summary>
    /// <remarks>
    /// The <c>_invalidGlyphGlyph</c> is used to substitute characters that cannot be matched to a valid glyph
    /// in the font. This ensures that rendering can proceed even when certain characters are absent or unsupported.
    /// </remarks>
    private NKComponentGlyph _invalidGlyphGlyph;

    /// <summary>
    /// The default glyph representation for invalid or unrecognized characters in the font.
    /// </summary>
    /// <remarks>
    /// The <c>DEFAULT_INVALID_GLYPH_GLYPH</c> is a fallback glyph used when a character or glyph
    /// cannot be rendered or resolved in the font. It commonly represents a placeholder symbol,
    /// such as <c>'?'</c>, to indicate an unrecognizable or unsupported character.
    /// </remarks>
    private static readonly NKComponentGlyph DEFAULT_INVALID_GLYPH_GLYPH = new(new char?[,] {{'?'}}, 0, []);

    /// <summary>
    /// Stores the cached maximum length of a ligature.
    /// </summary>
    private int? _maxLigatureLength = null;
    
    public NKFontInfo Info { get; }

    public NKFont(NKFontInfo info, GlyphInfo[] glyphs, NKComponentGlyph? invalidGlyphGlyph = null) {
        Info = info;
        (_simpleGlyphs, _ligatureGlyphs, _autoCompoundGlyphs) = SplitGlyphs(glyphs);
        _invalidGlyphGlyph = invalidGlyphGlyph ?? DEFAULT_INVALID_GLYPH_GLYPH;
    }

    private static GlyphSetTriple SplitGlyphs(GlyphInfo[] glyphs) {
        var simpleGlyphs = new SimpleGlyphSet();
        var ligatureGlyphs = new LigatureGlyphSet();
        var autoCompoundGlyphs = new AutoCompoundGlyphSet();

        foreach (var g in glyphs) {
            g.Switch(
                AddSimple,
                AddAutoC, 
                AddLigature
            );
        }
        
        return (simpleGlyphs, ligatureGlyphs, autoCompoundGlyphs);
        
        void AddSimple(SimpleGlyphInfo s) {
            if (!simpleGlyphs.TryAdd(s.Character, s)) 
                throw NKFontException.DuplicateGlyphSymbols(s.Character.ToString());
        }

        void AddLigature(LigatureGlyphInfo l) {
            if (!ligatureGlyphs.TryAdd(l.Ligature, l)) 
                throw NKFontException.DuplicateGlyphSymbols(l.Ligature);
        }

        void AddAutoC(AutoCompoundGlyphInfo a) {
            if (!autoCompoundGlyphs.TryAdd(a.Character, a)) 
                throw NKFontException.DuplicateGlyphSymbols(a.Character.ToString());
        }
    }
    
    #region RENDERING
    
    public void PlaceString(string str, ICharCanvas canvas) {
        var tokens = Tokenize(str, this);

        if (Info.MonospaceInfo.IsMonospace)
            PlaceMono(tokens, canvas);
        else 
            PlaceVariable(tokens, canvas);
    }

    #region NO MAX WIDTH
    
    private void PlaceMono(Token[] tokens, ICharCanvas canvas) {
        int cursor = 0;
        int y = 0;
        int lineSpacing = Info.LineSpacing + Info.MonospaceInfo.CharacterHeight;
        int width = Info.MonospaceInfo.CharacterWidth + Info.CharSpacing;
        
        for (int i = 0; i < tokens.Length; i++) {
            var t = tokens[i];
            
            if (t.Type == TokenType.SPACE) {
                cursor += width;
                continue;
            }

            if (t.Type == TokenType.NEWLINE) {
                cursor = 0;
                y += lineSpacing;
                continue;
            }
            
            if (t.Type == TokenType.INVALID) {            
                canvas.PlaceGlyph(
                    cursor, y - _invalidGlyphGlyph.BaselineOffset - _invalidGlyphGlyph.Height, _invalidGlyphGlyph);
                continue;
            }
            
            var d = t.TokenData!.Value;
            
            IGlyph glyph;
            int added;
            int offset = 0;
            
            switch (tokens[i].Type) {
                case TokenType.SIMPLE:
                    glyph = _simpleGlyphs[d.AsT0.Character].Glyph;
                    added = width;
                    break;
                case TokenType.LIGATURE:
                    glyph = _ligatureGlyphs[d.AsT1.Ligature].Glyph;
                    added = width * d.AsT1.Ligature.Length;
                    offset = (added - glyph.Width - Info.CharSpacing) / 2;
                    break;
                case TokenType.AUTO_COMPOUND:
                    glyph = _autoCompoundGlyphs[d.AsT2.Second].GetGlyph(_simpleGlyphs[d.AsT2.Main].Glyph);
                    added = width;
                    break;
                case TokenType.INVALID:
                case TokenType.SPACE:
                case TokenType.NEWLINE:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            canvas.PlaceGlyph(cursor + offset, y - glyph.BaselineOffset - glyph.Height, glyph);
            
            cursor += added;
        }
    }

    private void PlaceVariable(Token[] tokens, ICharCanvas canvas) {
        if (Info.Kerning)
            PlaceVariableWithKerning(tokens, canvas);
        else
            PlaceVariableNoKerning(tokens, canvas);
    }

    private void PlaceVariableWithKerning(Token[] tokens, ICharCanvas canvas) {
        int cursor = 0;
        int charSpacing = Info.CharSpacing;
        int spaceWidth = Info.WordSpacing;
        int y = 0;

        var line = new NKCharCanvas();
        NKCharCanvas? previousLine = null;
        var previousType = TokenType.NEWLINE;
        IGlyph? previousGlyph = null; 
        
        for (int i = 0; i < tokens.Length; i++) {
            var t = tokens[i];

            if (t.Type == TokenType.SPACE) {
                previousType = t.Type;
                continue;
            }

            if (t.Type == TokenType.NEWLINE) {
                previousType = TokenType.NEWLINE;
                previousGlyph = null;
                
                if (previousLine != null)
                    y += GetLineOffset(previousLine, line, Info.LineSpacing);
                
                canvas.PlaceCanvas(0, y, line);

                cursor = 0;
                
                previousLine = line;
                line = new NKCharCanvas();
                
                continue;
            }

            var d = t.TokenData!.Value;

            var glyph = tokens[i].Type switch {
                TokenType.SIMPLE        => _simpleGlyphs[d.AsT0.Character].Glyph,
                TokenType.LIGATURE      => _ligatureGlyphs[d.AsT1.Ligature].Glyph,
                TokenType.AUTO_COMPOUND => _autoCompoundGlyphs[d.AsT2.Second].GetGlyph(_simpleGlyphs[d.AsT2.Main].Glyph),
                TokenType.INVALID       => _invalidGlyphGlyph,
                _                       => throw new ArgumentOutOfRangeException()
            };

            if (previousGlyph != null) {
                cursor += GetGlyphOffset(
                    previousGlyph, 
                    glyph,
                    previousType is TokenType.SPACE 
                        ? spaceWidth
                        : charSpacing);
            }
            
            line.PlaceGlyph(cursor, 0 - glyph.BaselineOffset - glyph.Height, glyph);

            previousGlyph = glyph;
            previousType = t.Type;
        }
        
        if (previousLine != null)
            y += GetLineOffset(previousLine, line, Info.LineSpacing);
        
        canvas.PlaceCanvas(0, y, line);
    }

    private void PlaceVariableNoKerning(Token[] tokens, ICharCanvas canvas) {
        int cursor = 0;
        int charSpacing = Info.CharSpacing;
        int spaceWidth = Info.WordSpacing;
        int y = 0;

        var line = new NKCharCanvas();
        
        for (int i = 0; i < tokens.Length; i++) {
            var t = tokens[i];

            if (t.Type == TokenType.SPACE) {
                cursor += spaceWidth;
                continue;
            }

            if (t.Type == TokenType.NEWLINE) {
                canvas.PlaceCanvas(0, y, line);
                y += line.Height + Info.LineSpacing;
                cursor = 0;
                line = new NKCharCanvas();
                continue;
            }

            var d = t.TokenData!.Value;

            var glyph = tokens[i].Type switch {
                TokenType.SIMPLE        => _simpleGlyphs[d.AsT0.Character].Glyph,
                TokenType.LIGATURE      => _ligatureGlyphs[d.AsT1.Ligature].Glyph,
                TokenType.AUTO_COMPOUND => _autoCompoundGlyphs[d.AsT2.Second].GetGlyph(_simpleGlyphs[d.AsT2.Main].Glyph),
                TokenType.INVALID       => _invalidGlyphGlyph,
                _                       => throw new ArgumentOutOfRangeException()
            };

            var added = glyph.Width + charSpacing;

            line.PlaceGlyph(cursor, 0 - glyph.BaselineOffset - glyph.Height, glyph);
            
            cursor += added;
        }
        
        canvas.PlaceCanvas(0, y, line);
    }
    
    #endregion
    
    
    #region WITH MAX WIDTH
    
    public void PlaceString(string str, ICharCanvas canvas, int maxWidth) {
        var tokens = Tokenize(str, this);

        if (Info.MonospaceInfo.IsMonospace)
            PlaceMonoWithMaxWidth(tokens, canvas, maxWidth);
        else 
            PlaceVariableWithMaxWidth(tokens, canvas, maxWidth);
    }

    private void PlaceMonoWithMaxWidth(Token[] tokens, ICharCanvas canvas, int maxWidth) {
        int cursor = 0;
        int y = 0;
        int lineSpacing = Info.LineSpacing + Info.MonospaceInfo.CharacterHeight;
        int width = Info.MonospaceInfo.CharacterWidth + Info.CharSpacing;
        
        var currentLine = new List<Token>();
        
        for (int i = 0; i < tokens.Length; i++) {
            var t = tokens[i];
            
            if (t.Type == TokenType.NEWLINE) {
                // Process current line
                PlaceMonoLine(currentLine, canvas, y, width);
                currentLine.Clear();
                cursor = 0;
                y += lineSpacing;
                continue;
            }
            
            // Calculate token width
            int tokenWidth = GetMonoTokenWidth(t, width);
            
            // Check if adding this token would exceed maxWidth
            if (cursor + tokenWidth > maxWidth && cursor > 0) {
                // Try to find a good break point
                var (lineTokens, remainingTokens) = SplitLineAtWordBoundary(currentLine, maxWidth, width);
                
                // Place the line
                PlaceMonoLine(lineTokens, canvas, y, width);
                
                // Start new line with remaining tokens
                currentLine = remainingTokens.ToList();
                cursor = GetLineWidth(currentLine, width);
                y += lineSpacing;
            }
            
            currentLine.Add(t);
            cursor += tokenWidth;
        }
        
        // Place the final line
        if (currentLine.Count > 0) {
            PlaceMonoLine(currentLine, canvas, y, width);
        }
    }

    private void PlaceVariableWithMaxWidth(Token[] tokens, ICharCanvas canvas, int maxWidth) {
        if (Info.Kerning)
            PlaceVariableWithKerningAndMaxWidth(tokens, canvas, maxWidth);
        else
            PlaceVariableNoKerningWithMaxWidth(tokens, canvas, maxWidth);
    }

    private void PlaceVariableWithKerningAndMaxWidth(Token[] tokens, ICharCanvas canvas, int maxWidth) {
        int y = 0;
        var currentLine = new List<Token>();
        
        for (int i = 0; i < tokens.Length; i++) {
            var t = tokens[i];
            
            if (t.Type == TokenType.NEWLINE) {
                // Process current line
                var lineCanvas = CreateVariableLineWithKerning(currentLine);
                canvas.PlaceCanvas(0, y, lineCanvas);
                currentLine.Clear();
                y += lineCanvas.Height + Info.LineSpacing;
                continue;
            }
            
            // Check if adding this token would exceed maxWidth
            var testLine = new List<Token>(currentLine) { t };
            var testCanvas = CreateVariableLineWithKerning(testLine);
            
            if (testCanvas.Width > maxWidth && currentLine.Count > 0) {
                // Try to find a good break point
                var (lineTokens, remainingTokens) = SplitLineAtWordBoundaryVariable(currentLine, maxWidth);
                
                // Place the line
                var lineCanvas = CreateVariableLineWithKerning(lineTokens);
                canvas.PlaceCanvas(0, y, lineCanvas);
                y += lineCanvas.Height + Info.LineSpacing;
                
                // Start new line with remaining tokens plus current token
                currentLine = remainingTokens.ToList();
                currentLine.Add(t);
            } else {
                currentLine.Add(t);
            }
        }
        
        // Place the final line
        if (currentLine.Count > 0) {
            var lineCanvas = CreateVariableLineWithKerning(currentLine);
            canvas.PlaceCanvas(0, y, lineCanvas);
        }
    }

    private void PlaceVariableNoKerningWithMaxWidth(Token[] tokens, ICharCanvas canvas, int maxWidth) {
        int y = 0;
        var currentLine = new List<Token>();
        
        for (int i = 0; i < tokens.Length; i++) {
            var t = tokens[i];
            
            if (t.Type == TokenType.NEWLINE) {
                // Process current line
                var lineCanvas = CreateVariableLineNoKerning(currentLine);
                canvas.PlaceCanvas(0, y, lineCanvas);
                currentLine.Clear();
                y += lineCanvas.Height + Info.LineSpacing;
                continue;
            }
            
            // Check if adding this token would exceed maxWidth
            var testLine = new List<Token>(currentLine) { t };
            var testLineWidth = GetVariableLineWidth(testLine);
            
            if (testLineWidth > maxWidth && currentLine.Count > 0) {
                // Try to find a good break point
                var (lineTokens, remainingTokens) = SplitLineAtWordBoundaryVariable(currentLine, maxWidth);
                
                // Place the line
                var lineCanvas = CreateVariableLineNoKerning(lineTokens);
                canvas.PlaceCanvas(0, y, lineCanvas);
                y += lineCanvas.Height + Info.LineSpacing;
                
                // Start new line with remaining tokens plus current token
                currentLine = remainingTokens.ToList();
            }

            currentLine.Add(t);
        }
        
        // Place the final line
        if (currentLine.Count > 0) {
            var lineCanvas = CreateVariableLineNoKerning(currentLine);
            canvas.PlaceCanvas(0, y, lineCanvas);
        }
    }

    // Helper methods for line wrapping
    private void PlaceMonoLine(List<Token> tokens, ICharCanvas canvas, int y, int charWidth) {
        int cursor = 0;
        
        foreach (var t in tokens) {
            if (t.Type == TokenType.SPACE) {
                cursor += charWidth;
                continue;
            }
            
            if (t.Type == TokenType.INVALID) {
                canvas.PlaceGlyph(cursor, y - _invalidGlyphGlyph.BaselineOffset - _invalidGlyphGlyph.Height, _invalidGlyphGlyph);
                cursor += charWidth;
                continue;
            }
            
            var d = t.TokenData!.Value;
            
            IGlyph glyph;
            int added;
            int offset = 0;
            
            switch (t.Type) {
                case TokenType.SIMPLE:
                    glyph = _simpleGlyphs[d.AsT0.Character].Glyph;
                    added = charWidth;
                    break;
                case TokenType.LIGATURE:
                    glyph = _ligatureGlyphs[d.AsT1.Ligature].Glyph;
                    added = charWidth * d.AsT1.Ligature.Length;
                    offset = (added - glyph.Width - Info.CharSpacing) / 2;
                    break;
                case TokenType.AUTO_COMPOUND:
                    glyph = _autoCompoundGlyphs[d.AsT2.Second].GetGlyph(_simpleGlyphs[d.AsT2.Main].Glyph);
                    added = charWidth;
                    break;
                default:
                    continue;
            }

            canvas.PlaceGlyph(cursor + offset, y - glyph.BaselineOffset - glyph.Height, glyph);
            cursor += added;
        }
    }

    private NKCharCanvas CreateVariableLineWithKerning(List<Token> tokens) {
        var line = new NKCharCanvas();
        int cursor = 0;
        int charSpacing = Info.CharSpacing;
        int spaceWidth = Info.WordSpacing;
        var previousType = TokenType.NEWLINE;
        IGlyph? previousGlyph = null;
        
        foreach (var t in tokens) {
            if (t.Type == TokenType.SPACE) {
                previousType = t.Type;
                continue;
            }
            
            var d = t.TokenData!.Value;
            var glyph = t.Type switch {
                TokenType.SIMPLE        => _simpleGlyphs[d.AsT0.Character].Glyph,
                TokenType.LIGATURE      => _ligatureGlyphs[d.AsT1.Ligature].Glyph,
                TokenType.AUTO_COMPOUND => _autoCompoundGlyphs[d.AsT2.Second].GetGlyph(_simpleGlyphs[d.AsT2.Main].Glyph),
                TokenType.INVALID       => _invalidGlyphGlyph,
                _                       => throw new ArgumentOutOfRangeException()
            };

            if (previousGlyph != null) {
                cursor += GetGlyphOffset(
                    previousGlyph, 
                    glyph,
                    previousType is TokenType.SPACE 
                        ? spaceWidth
                        : charSpacing);
            }
            
            line.PlaceGlyph(cursor, 0 - glyph.BaselineOffset - glyph.Height, glyph);
            previousGlyph = glyph;
            previousType = t.Type;
        }
        
        return line;
    }

    private NKCharCanvas CreateVariableLineNoKerning(List<Token> tokens) {
        var line = new NKCharCanvas();
        int cursor = 0;
        int charSpacing = Info.CharSpacing;
        int spaceWidth = Info.WordSpacing;
        
        foreach (var t in tokens) {
            if (t.Type == TokenType.SPACE) {
                cursor += spaceWidth;
                continue;
            }
            
            var d = t.TokenData!.Value;
            var glyph = t.Type switch {
                TokenType.SIMPLE        => _simpleGlyphs[d.AsT0.Character].Glyph,
                TokenType.LIGATURE      => _ligatureGlyphs[d.AsT1.Ligature].Glyph,
                TokenType.AUTO_COMPOUND => _autoCompoundGlyphs[d.AsT2.Second].GetGlyph(_simpleGlyphs[d.AsT2.Main].Glyph),
                TokenType.INVALID       => _invalidGlyphGlyph,
                _                       => throw new ArgumentOutOfRangeException()
            };

            line.PlaceGlyph(cursor, 0 - glyph.BaselineOffset - glyph.Height, glyph);
            cursor += glyph.Width + charSpacing;
        }
        
        return line;
    }

    private int GetMonoTokenWidth(Token token, int charWidth) {
        return token.Type switch {
            TokenType.SPACE => charWidth,
            TokenType.LIGATURE => charWidth * token.TokenData!.Value.AsT1.Ligature.Length,
            TokenType.SIMPLE or TokenType.AUTO_COMPOUND or TokenType.INVALID => charWidth,
            _ => 0
        };
    }

    private int GetLineWidth(List<Token> tokens, int charWidth) {
        return tokens.Sum(t => GetMonoTokenWidth(t, charWidth));
    }

    private int GetVariableLineWidth(List<Token> tokens) {
        if (!tokens.Any()) return 0;
        
        // For simplicity, create a temporary canvas to measure width
        var tempCanvas = Info.Kerning ? CreateVariableLineWithKerning(tokens) : CreateVariableLineNoKerning(tokens);
        return tempCanvas.Width;
    }

    private (List<Token> lineTokens, List<Token> remainingTokens) SplitLineAtWordBoundary(
        List<Token> tokens, int maxWidth, int charWidth) {
        
        // Find the last space before exceeding maxWidth
        int lastSpaceIndex = -1;
        int currentWidth = 0;
        
        for (int i = 0; i < tokens.Count; i++) {
            var tokenWidth = GetMonoTokenWidth(tokens[i], charWidth);
            
            if (currentWidth + tokenWidth > maxWidth) {
                break;
            }
            
            if (tokens[i].Type == TokenType.SPACE) {
                lastSpaceIndex = i;
            }
            
            currentWidth += tokenWidth;
        }
        
        // If no space found, split at the maximum possible position
        if (lastSpaceIndex == -1) {
            currentWidth = 0;
            int splitIndex = 0;
            
            for (int i = 0; i < tokens.Count; i++) {
                var tokenWidth = GetMonoTokenWidth(tokens[i], charWidth);
                if (currentWidth + tokenWidth > maxWidth) {
                    splitIndex = Math.Max(1, i); // Ensure at least one character
                    break;
                }
                currentWidth += tokenWidth;
                splitIndex = i + 1;
            }
            
            return (tokens.Take(splitIndex).ToList(), tokens.Skip(splitIndex).ToList());
        }
        
        // Split at the space, excluding the space from the next line
        var lineTokens = tokens.Take(lastSpaceIndex).ToList();
        var remainingTokens = tokens.Skip(lastSpaceIndex + 1).ToList();
        
        return (lineTokens, remainingTokens);
    }

    private (List<Token> lineTokens, List<Token> remainingTokens) SplitLineAtWordBoundaryVariable(
        List<Token> tokens, int maxWidth) {
        
        // Find the last space before exceeding maxWidth
        int lastSpaceIndex = -1;
        
        for (int i = 0; i < tokens.Count; i++) {
            var testTokens = tokens.Take(i + 1).ToList();
            var testWidth = GetVariableLineWidth(testTokens);
            
            if (testWidth > maxWidth) {
                break;
            }
            
            if (tokens[i].Type == TokenType.SPACE) {
                lastSpaceIndex = i;
            }
        }
        
        // If no space found, split at the maximum possible position
        if (lastSpaceIndex == -1) {
            int splitIndex = 1; // Ensure at least one character
            
            for (int i = 1; i < tokens.Count; i++) {
                var testTokens = tokens.Take(i + 1).ToList();
                var testWidth = GetVariableLineWidth(testTokens);
                if (testWidth > maxWidth) {
                    splitIndex = i;
                    break;
                }
                splitIndex = i + 1;
            }
            
            return (tokens.Take(splitIndex).ToList(), tokens.Skip(splitIndex).ToList());
        }
        
        // Split at the space, excluding the space from the next line
        var lineTokens = tokens.Take(lastSpaceIndex).ToList();
        var remainingTokens = tokens.Skip(lastSpaceIndex + 1).ToList();
        
        return (lineTokens, remainingTokens);
    }

    #endregion

    #region HELPER METHODS
    
    private static int GetGlyphOffset(IGlyph g1, IGlyph g2, int spacing) {
        int closest = int.MaxValue;
        int closestY = -1;

        var startY = Math.Max(g1.BaselineOffset, g2.BaselineOffset);
        var endY = Math.Min(g1.Height + g1.BaselineOffset, g2.Height + g2.BaselineOffset);
        
        for (int y = startY; y < endY; y++) { // y is baseline offset
            int firstGap = 0;
            int secondGap = 0;

            for (int x = g1.Width - 1; x >= 0; x--) {
                if (g1.Glyph[x, g1.Height - (y - g1.BaselineOffset) - 1] == null) continue;
                
                firstGap = g1.Width - x - 1;
                break;
            }

            for (int x = 0; x < g2.Width; x++) {
                if (g2.Glyph[x, g2.Height - (y - g2.BaselineOffset) - 1] == null) continue;
                
                secondGap = x;
                break;
            }
            
            int gap = firstGap + secondGap;

            if (gap >= closest) 
                continue;
            
            closest = gap;
            closestY = y;
        }
        
        if (closest == int.MaxValue) return g1.Width;
        
        for (int x = g1.Width - 1; x >= 0; x--) {
            if (g1.Glyph[x, g1.Height - (closestY - g1.BaselineOffset) - 1] == null) continue;
            return x + 1 - GetSecondOffset() + spacing;
        }
        
        return g1.Width;

        int GetSecondOffset() {
            for (int x = 0; x < g2.Width; x++) {
                if (g2.Glyph[x, g2.Height - (closestY - g2.BaselineOffset) - 1] == null) continue;
                return x;
            }
            
            return 0;
        }
    }

    private static int GetLineOffset(NKCharCanvas l1, NKCharCanvas l2, int spacing) {
        int closest = int.MaxValue;
        int closestX = -1;

        for (int x = 0; x < Math.Min(l1.Width, l2.Width); x++) {
            int firstGap = int.MaxValue;
            int secondGap = int.MaxValue;

            for (int y = l1.Height - 1; y >= 0; y--) {
                if (l1.UncoordinatedGet(x, y) == null) continue;
                
                firstGap = (l1.Height - y) - 1;
                break;
            }

            for (int y = 0; y < l2.Height; y++) {
                if (l2.UncoordinatedGet(x, y) == null) continue;
                
                secondGap = y;
                break;
            }
            
            if (firstGap == int.MaxValue || secondGap == int.MaxValue) continue;
            
            int gap = firstGap + secondGap;
            
            if (gap >= closest) 
                continue;
            
            closest = gap;
            closestX = x;
        }

        if (closest == int.MaxValue) return l1.Width + spacing;
        
        for (int y = l1.Height - 1; y >= 0; y--) {
            if (l1.UncoordinatedGet(closestX, y) == null) continue;
            
            return y + 1 + spacing;
        }
        
        return l1.Height + spacing;
    }
    
    #endregion
    
    #endregion
    
    #region TOKENIZER
    
    /// <summary>
    /// Checks if a given string represents a valid ligature in the current font.
    /// </summary>
    /// <param name="candidate">The string to check for ligature validity.</param>
    /// <returns>True if the string is a valid ligature; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasLigature(string candidate)
        => _ligatureGlyphs.ContainsKey(candidate);

    /// <summary>
    /// Determines whether a given combination of a main character and a secondary character
    /// is valid as an auto-compound glyph in the current font.
    /// </summary>
    /// <param name="main">The main character to check in the auto-compound glyph.</param>
    /// <param name="secondary">The secondary character to verify against the main character.</param>
    /// <param name="mainFirst">Whether the main character is the first of the two.</param>
    /// <returns>True if the main and secondary characters form a valid auto-compound glyph; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasAutoCompound(char main, char secondary, bool mainFirst)
        => _autoCompoundGlyphs.TryGetValue(main, out var info) &&
           info.ApplicableChars.IsApplicable(secondary) &&
           info.MainFirst == mainFirst;

    /// <summary>
    /// Checks if the given character exists as a simple glyph in the current font.
    /// </summary>
    /// <param name="candidate">The character to check for existence as a simple glyph.</param>
    /// <returns>True if the character exists as a simple glyph; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool HasSimple(char candidate)
        => _simpleGlyphs.ContainsKey(candidate);

    /// <summary>
    /// Retrieves the maximum length of any ligature in the current font.
    /// </summary>
    /// <returns>The length of the longest ligature present in the font.</returns>
    private int GetMaxLigatureLength() {
        _maxLigatureLength ??= _ligatureGlyphs.Max(kvp => kvp.Value.Ligature.Length);
        return _maxLigatureLength.Value;
    }

    public int MaxLigatureLength => GetMaxLigatureLength();

    #endregion
    
    #if DEBUG || WIP
    
    public IGlyph GetSimple(char c) => _simpleGlyphs[c].Glyph;
    public IGlyph GetLigature(string ligature) => _ligatureGlyphs[ligature].Glyph;
    public IGlyph GetAutoCompound(char main, char secondary) => 
        _autoCompoundGlyphs[main].GetGlyph(_simpleGlyphs[secondary].Glyph);
    
    #endif
}