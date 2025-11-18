// NeoKolors
// Copyright (c) 2025 KryKom

using System.Runtime.CompilerServices;
using NeoKolors.Tui.Fonts.Exceptions;
using NeoKolors.Tui.Styles;
using static NeoKolors.Tui.Fonts.NKFontStringTokenizer;
using static NeoKolors.Tui.Fonts.NKFontStringTokenizer.TokenType;

namespace NeoKolors.Tui.Fonts;

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
    /// The <c>_invalidTokenGlyph</c> is used to substitute characters that cannot be matched to a valid glyph
    /// in the font. This ensures that rendering can proceed even when certain characters are absent or unsupported.
    /// </remarks>
    private readonly NKComponentGlyph _invalidTokenGlyph;

    /// <summary>
    /// The default glyph representation for invalid or unrecognized characters in the font.
    /// </summary>
    /// <remarks>
    /// The <c>DEFAULT_INVALID_TOKEN_GLYPH</c> is a fallback glyph used when a character or glyph
    /// cannot be rendered or resolved in the font. It commonly represents a placeholder symbol,
    /// such as <c>'?'</c>, to indicate an unrecognizable or unsupported character.
    /// </remarks>
    private static readonly NKComponentGlyph DEFAULT_INVALID_TOKEN_GLYPH = new(new char?[,] {{'?'}}, 0, []);

    /// <summary>
    /// Stores the cached maximum length of a ligature.
    /// </summary>
    private int? _maxLigatureLength = null;
    
    private readonly NKFontInfo _info;
    
    public IFontInfo Info => _info;

    public NKFont(NKFontInfo info, GlyphInfo[] glyphs, NKComponentGlyph? invalidTokenGlyph = null) {
        _info = info;
        (_simpleGlyphs, _ligatureGlyphs, _autoCompoundGlyphs) = SplitGlyphs(glyphs);
        _invalidTokenGlyph = invalidTokenGlyph ?? DEFAULT_INVALID_TOKEN_GLYPH;
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PlaceString(string str, ICharCanvas canvas) 
        => PlaceString(str, canvas, int.MaxValue);

    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PlaceString(string str, ICharCanvas canvas, int maxWidth) 
        => PlaceString(str, canvas, new Size(maxWidth, int.MaxValue));

    
    public void PlaceString(
        string str, ICharCanvas canvas, Size bounds,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign verticalAlign = VerticalAlign.TOP, bool overflow = false) 
    {
        var tokens = Tokenize(str, this);

        if (_info.SpacingInfo.IsMonospace)
            PlaceMonoWithAlign(tokens, canvas, bounds, horizontalAlign, verticalAlign, overflow);
        else 
            if (_info.SpacingInfo.AsVariable.Kerning)
                if (_info.SpacingInfo.AsVariable.LineKerning)
                    PlaceVariableKerningWithAlignWithLineKerning(tokens, canvas, bounds, horizontalAlign, verticalAlign, overflow);
                else
                    PlaceVariableKerningWithAlignNoLineKerning(tokens, canvas, bounds, horizontalAlign, verticalAlign, overflow);
            else 
                PlaceVariableNoKerningWithAlign(tokens, canvas, bounds, horizontalAlign, verticalAlign, overflow);
    }

    // todo: support overflow
    
    private void PlaceMonoWithAlign(
        Token[] tokens,
        ICharCanvas canvas,
        Size bounds,
        HorizontalAlign horizontalAlign,
        VerticalAlign verticalAlign,
        bool overflow) 
    {
        var lines = SplitLinesMono(tokens, bounds.Width);
        var lineSize = _info.SpacingInfo.AsMonospace.CharacterHeight + _info.LineSpacing;
        var charSize = _info.SpacingInfo.AsMonospace.CharacterWidth + _info.CharSpacing;
        
        var yOffset = verticalAlign switch {
            VerticalAlign.TOP => 0,
            VerticalAlign.CENTER => Math.Max(0, (bounds.Height - lineSize * lines.Length) / 2),
            VerticalAlign.BOTTOM => Math.Max(0, bounds.Height - lineSize * lines.Length),
            _ => throw new ArgumentOutOfRangeException()
        };

        Func<int, int, int> computeXOffset = horizontalAlign switch {
            HorizontalAlign.LEFT   => static (_, _)          => 0,
            HorizontalAlign.CENTER => static (width, bounds) => Math.Max(0, (bounds - width) / 2),
            HorizontalAlign.RIGHT  => static (width, bounds) => Math.Max(0, bounds - width),
            _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlign), horizontalAlign, null)
        };

        int y = yOffset;
        
        for (int i = 0; i < lines.Length; i++) {
            var lineData = lines[i];
            var lineTokens = lineData.Tokens;

            var xOffset = computeXOffset(lineData.Width, bounds.Width);
            
            int x = xOffset;
            
            for (int j = 0; j < lineTokens.Length; j++) {
                var token = lineTokens[j];

                if (token.Type == SPACE) {
                    x += charSize;
                    continue;
                }
                
                var tokenData = token.Data;
                
                var glyph = token.Type switch {
                    INVALID => 
                        _invalidTokenGlyph,
                    SIMPLE => 
                        _simpleGlyphs[tokenData!.Value.AsT0.Character].Glyph,
                    LIGATURE => 
                        _ligatureGlyphs[tokenData!.Value.AsT1.Ligature].Glyph,
                    AUTO_COMPOUND => 
                        _autoCompoundGlyphs[tokenData!.Value.AsT2.Second]
                            .GetGlyph(_simpleGlyphs[tokenData.Value.AsT0.Character].Glyph),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                canvas.PlaceGlyph(x, y, glyph);
                
                x += charSize;
            }
            
            y += lineSize;
        }
    }

    private void PlaceVariableNoKerningWithAlign(
        Token[] tokens,
        ICharCanvas canvas,
        Size bounds,
        HorizontalAlign horizontalAlign,
        VerticalAlign verticalAlign,
        bool overflow) 
    {
        var lines = SplitLinesVariableNoKerning(tokens, bounds.Width);
        var wordSpacing = _info.SpacingInfo.AsVariable.WordSpacing;
        var lineSpacing = _info.LineSpacing;
        var charSpacing = _info.CharSpacing;
        
        var totalHeight = lines.Sum(l => l.Height) + lineSpacing * (lines.Length - 1);
        
        var yOffset = verticalAlign switch {
            VerticalAlign.TOP => 0,
            VerticalAlign.CENTER => Math.Max(0, (bounds.Height - totalHeight) / 2),
            VerticalAlign.BOTTOM => Math.Max(0, bounds.Height - totalHeight),
            _ => throw new ArgumentOutOfRangeException()
        }; 

        Func<int, int, int> computeXOffset = horizontalAlign switch {
            HorizontalAlign.LEFT   => static (_, _)          => 0,
            HorizontalAlign.CENTER => static (width, bounds) => Math.Max(0, (bounds - width) / 2),
            HorizontalAlign.RIGHT  => static (width, bounds) => Math.Max(0, bounds - width),
            _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlign), horizontalAlign, null)
        };
        
        int y = yOffset;
        
        for (int i = 0; i < lines.Length; i++) {
            var lineData = lines[i];
            var lineTokens = lineData.Tokens;

            var xOffset = computeXOffset(lineData.Width, bounds.Width);
            
            int x = xOffset;
            
            for (int j = 0; j < lineTokens.Length; j++) {
                var token = lineTokens[j];

                if (token.Type == SPACE) {
                    x += wordSpacing * token.Data!.Value.AsT3.Width;
                    continue;
                }
                
                var tokenData = token.Data;
                
                var glyph = token.Type switch {
                    INVALID => 
                        _invalidTokenGlyph,
                    SIMPLE => 
                        _simpleGlyphs[tokenData!.Value.AsT0.Character].Glyph,
                    LIGATURE => 
                        _ligatureGlyphs[tokenData!.Value.AsT1.Ligature].Glyph,
                    AUTO_COMPOUND => 
                        _autoCompoundGlyphs[tokenData!.Value.AsT2.Second]
                            .GetGlyph(_simpleGlyphs[tokenData.Value.AsT0.Character].Glyph),
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                canvas.PlaceGlyph(x, y + glyph.BaselineOffset, glyph);
                x += glyph.Width + charSpacing;
            }
            
            y += lineData.Height + lineSpacing;
        }
    } 

    private void PlaceVariableKerningWithAlignNoLineKerning(
        Token[] tokens,
        ICharCanvas canvas,
        Size bounds,
        HorizontalAlign horizontalAlign,
        VerticalAlign verticalAlign,
        bool overflow) 
    {
        var lines = SplitLinesVariableKerning(tokens, bounds.Width);
        var lineSpacing = _info.LineSpacing;

        var totalHeight = lines.Sum(l => l.Height) + lineSpacing * (lines.Length - 1);
        
        var yOffset = verticalAlign switch {
            VerticalAlign.TOP    => 0,
            VerticalAlign.CENTER => Math.Max(0, (bounds.Height - totalHeight) / 2),
            VerticalAlign.BOTTOM => Math.Max(0, bounds.Height - totalHeight),
            _ => throw new ArgumentOutOfRangeException()
        }; 

        Func<int, int, int> computeXOffset = horizontalAlign switch {
            HorizontalAlign.LEFT   => static (_, _)          => 0,
            HorizontalAlign.CENTER => static (width, bounds) => Math.Max(0, (bounds - width) / 2),
            HorizontalAlign.RIGHT  => static (width, bounds) => Math.Max(0, bounds - width),
            _ => throw new ArgumentOutOfRangeException(nameof(horizontalAlign), horizontalAlign, null)
        };
        
        int y = yOffset;
        
        for (int i = 0; i < lines.Length; i++) {
            var lineData = lines[i];
            var lineTokens = lineData.Tokens;

            var xOffset = computeXOffset(lineData.Width, bounds.Width);
            
            int x = xOffset;
            
            for (int j = 0; j < lineTokens.Length; j++) {
                var token = lineTokens[j];

                if (token.Type == SPACE) {
                    continue;
                }
                
                var tokenData = token.Data;
                
                var glyph = token.Type switch {
                    INVALID 
                        => _invalidTokenGlyph,
                    SIMPLE
                        => _simpleGlyphs[tokenData!.Value.AsT0.Character].Glyph,
                    LIGATURE 
                        => _ligatureGlyphs[tokenData!.Value.AsT1.Ligature].Glyph,
                    AUTO_COMPOUND 
                        => _autoCompoundGlyphs[tokenData!.Value.AsT2.Second]
                            .GetGlyph(_simpleGlyphs[tokenData.Value.AsT0.Character].Glyph),
                    _ => throw new ArgumentOutOfRangeException()
                };

                x += lineData.Offsets[j];
                canvas.PlaceGlyph(x, y - glyph.BaselineOffset - glyph.Height, glyph);
            }
            
            y += lineData.Height + lineSpacing;
        }
    }
    
    // todo: line kerning
    
    private void PlaceVariableKerningWithAlignWithLineKerning(
        Token[] tokens,
        ICharCanvas canvas,
        Size bounds,
        HorizontalAlign horizontalAlign,
        VerticalAlign verticalAlign,
        bool overflow) 
    {
        throw new NotImplementedException();
    }

    #endregion RENDERING

    #region OFFSETS CALCULATION

    /// <summary>
    /// Calculates the horizontal offset required to place two glyphs next to each other,
    /// considering their shapes, spacing, and baseline alignment.
    /// </summary>
    /// <param name="g1">The first glyph to be placed.</param>
    /// <param name="g2">The second glyph to be placed next to the first.</param>
    /// <param name="spacing">The initial spacing value to consider between the two glyphs.</param>
    /// <returns>
    /// An integer representing the horizontal offset needed to place the two glyphs
    /// with the correct spacing while avoiding overlaps.
    /// </returns>
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

    /// <summary>
    /// Calculates the line offset between two character canvases, factoring in their respective heights
    /// and the provided line spacing. The method determines the optimal alignment of the two canvases
    /// by minimizing the vertical gap between their content.
    /// </summary>
    /// <param name="l1">The first character canvas, representing the previous line.</param>
    /// <param name="l2">The second character canvas, representing the current line.</param>
    /// <param name="spacing">The additional spacing to be added between the two lines.</param>
    /// <returns>The calculated vertical offset needed to place the second line relative to the first.</returns>
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
    
    #region SPLIT LINES

    /// <summary>
    /// Splits a list of tokens into separate lines based on the specified maximum width.
    /// The splitting behavior adapts according to the font's monospaced or kerning characteristics.
    /// </summary>
    /// <param name="tokens">The list of tokens to be split into multiple lines.</param>
    /// <param name="maxWidth">The maximum allowed width for a single line of tokens.</param>
    /// <returns>
    /// An array of <c>LineData</c> objects where each element represents a line of tokens
    /// that fits within the specified maximum width.
    /// </returns>
    private LineData[] SplitLines(Token[] tokens, int maxWidth) {
        return _info.SpacingInfo.IsMonospace
            ? SplitLinesMono(tokens, maxWidth)
            : _info.SpacingInfo.AsVariable.Kerning
                ? SplitLinesVariableKerning(tokens, maxWidth)
                : SplitLinesVariableNoKerning(tokens, maxWidth);
    }

    private LineData[] SplitLinesMono(Token[] tokens, int maxWidth) {
        List<LineData> lines = [];
        int? lastSpaceIndex = null;
        int widthAtLastSpace = 0;
        int currentWidth = 0;
        int cursor = 0;
        int lineStartIndex = 0;
        int charWidth = _info.SpacingInfo.AsMonospace.CharacterWidth;
        int spacing = _info.CharSpacing;

        for (int i = 0; i < tokens.Length; i++) {
            bool skip = false;
            
            switch (tokens[i].Type) {
                case NEWLINE:
                    lines.Add(new LineData(
                        lastSpaceIndex is null ? currentWidth : widthAtLastSpace,
                        tokens.Skip(lineStartIndex).Take(i - lineStartIndex).ToArray()
                    ));
                    
                    lastSpaceIndex = null;
                    widthAtLastSpace = 0;
                    lineStartIndex = i + 1;
                    currentWidth = 0;
                    cursor = 0;

                    skip = true;
                    break;
                case SPACE:
                    // // this should not in theory be necessary, but I keep it here for now 
                    // if (currentWidth + singleCharWidth > maxWidth) {
                    //     lastSpaceIndex = null;
                    //     lines.Add(currentLine.ToArray());
                    //     currentLine.Clear();
                    //     skip = true;
                    //     break;
                    // }

                    int sw = tokens[i].Data!.Value.AsT3.Width;
                    
                    widthAtLastSpace = currentWidth;
                    
                    lastSpaceIndex = i;
                    currentWidth = cursor + charWidth * sw + spacing * (sw - 1);
                    cursor = currentWidth + spacing;
                    
                    break;
                case INVALID or SIMPLE or AUTO_COMPOUND:
                    currentWidth = cursor + charWidth;
                    cursor = currentWidth + spacing;
                    break;
                case LIGATURE:
                    int lw = tokens[i].Data!.Value.AsT1.Ligature.Length;
                    currentWidth = cursor + charWidth * lw + spacing * (lw - 1);
                    cursor = currentWidth + spacing;
                    break;
            }
            
            if (skip) continue;
            
            if (currentWidth > maxWidth) {
                i = lastSpaceIndex ?? i - 1;
                
                lines.Add(new LineData(
                    lastSpaceIndex is null ? currentWidth : widthAtLastSpace, 
                    tokens.Skip(lineStartIndex).Take(i - lineStartIndex).ToArray()
                ));
                
                lineStartIndex = i + 1;
                lastSpaceIndex = null;
                widthAtLastSpace = 0;
                currentWidth = 0;
                cursor = 0;
            }
        }

        if (lineStartIndex < tokens.Length) {
            lines.Add(new LineData(
                currentWidth,
                tokens.Skip(lineStartIndex).ToArray()
            ));
        }
        
        return lines.ToArray();
    }

    private LineData[] SplitLinesVariableNoKerning(Token[] tokens, int maxWidth) {
        List<LineData> lines = [];
        int? lastSpaceIndex = null;
        int widthAtLastSpace = 0;
        int currentWidth = 0;
        int lineStartIndex = 0;
        int wordSpacing = _info.SpacingInfo.AsVariable.WordSpacing;
        int emptyLineHeight = _info.SpacingInfo.AsVariable.EmptyLineHeight;
        int charSpacing = _info.CharSpacing;
        int maxY = int.MinValue;
        int minY = int.MaxValue;

        for (int i = 0; i < tokens.Length; i++) {
            bool skip = false;
            int newWidth = 0;

            switch (tokens[i].Type) {
                case NEWLINE:
                    lastSpaceIndex = null;
                    lineStartIndex = i + 1;

                    lines.Add(new LineData(
                        currentWidth,
                        Math.Max(maxY - minY, emptyLineHeight),
                        tokens.Skip(lineStartIndex).Take(i - lineStartIndex - 1).ToArray()
                    ));
                    
                    maxY = int.MinValue;
                    minY = int.MaxValue;

                    skip = true;
                    break;
                case SPACE:
                    widthAtLastSpace = currentWidth;
                    newWidth = currentWidth + wordSpacing * tokens[i].Data!.Value.AsT3.Width;
                    lastSpaceIndex = i;

                    if (newWidth > maxWidth) {
                        lines.Add(new LineData(
                            currentWidth,
                            maxY - minY, 
                            tokens.Skip(lineStartIndex).Take(i - lineStartIndex - 1).ToArray()
                        ));
                        
                        maxY = int.MinValue;
                        minY = int.MaxValue;
                        skip = true;
                        lastSpaceIndex = null;
                        lineStartIndex = i + 1;
                        currentWidth = 0;
                    }

                    break;
                case INVALID or SIMPLE or AUTO_COMPOUND or LIGATURE: {
                    var t = tokens[i].Data!.Value;
                    var type = tokens[i].Type;

                    var g = type switch {
                        SIMPLE
                            => _simpleGlyphs[t.AsT0.Character].Glyph,
                        LIGATURE
                            => _ligatureGlyphs[t.AsT1.Ligature].Glyph,
                        AUTO_COMPOUND
                            => _autoCompoundGlyphs[t.AsT2.Second].GetGlyph(_simpleGlyphs[t.AsT2.Main].Glyph),
                        INVALID
                            => _invalidTokenGlyph,
                        _
                            => throw new InvalidOperationException("Invalid token type.")
                    };

                    maxY = Math.Max(maxY, g.Height + g.BaselineOffset);
                    minY = Math.Min(minY, g.BaselineOffset);
                    
                    newWidth = currentWidth + g.Width + charSpacing; 
                    break;
                }
            }

            if (skip) continue;
            
            if (newWidth > maxWidth) {
                lines.Add(new LineData(
                    lastSpaceIndex == null ? currentWidth : widthAtLastSpace, 
                    maxY - minY, 
                    tokens.Skip(lineStartIndex).Take(((lastSpaceIndex) ?? (i - 2)) - lineStartIndex).ToArray()
                ));
                
                i = lastSpaceIndex ?? (i - 2);
                lastSpaceIndex = null;

                lineStartIndex = i + 1;
                currentWidth = 0;
                maxY = int.MinValue;
                minY = int.MaxValue;
                continue;
            }
            
            currentWidth = newWidth;
        }

        if (lineStartIndex < tokens.Length) {
            lines.Add(new LineData(
                currentWidth,
                maxY - minY,
                tokens.Skip(lineStartIndex).ToArray()
            ));
        }
        
        return lines.ToArray();
    }

    private LineData[] SplitLinesVariableKerning(Token[] tokens, int maxWidth) { 
        
        // SPOILER:
        // DON'T THE FUCK TOUCH THIS UNLESS YOU REALLY KNOW WHAT YOU'RE DOING
        // I did not, and it was a great mistake.
        // Every time you edit this method and fail to understand what it does,
        // add one to this counter: 2
        
        List<LineData> lines = [];
        int? lastSpaceIndex = null;
        int widthAtLastSpace = 0;
        int heightAtLastSpace = 0;
        int currentWidth = 0;
        int lineStartIndex = 0;
        int wordSpacing = _info.SpacingInfo.AsVariable.WordSpacing;
        int emptyLineHeight = _info.SpacingInfo.AsVariable.EmptyLineHeight;
        int charSpacing = _info.CharSpacing;
        int maxY = int.MinValue;
        int minY = int.MaxValue;
        int spacing = charSpacing;
        IGlyph? lastGlyph = null;
        List<int> offsets = [];

        for (int i = 0; i < tokens.Length; i++) {
            bool skip = false;
            int newWidth = 0;

            switch (tokens[i].Type) {
                case NEWLINE:
                    lastSpaceIndex = null;
                    lineStartIndex = i + 1;
                    
                    lines.Add(new LineData(
                        currentWidth,
                        Math.Max(maxY - minY, emptyLineHeight),
                        tokens.Skip(lineStartIndex).Take(i - lineStartIndex - 1).ToArray(),
                        offsets.ToArray()
                    ));
                    
                    maxY = int.MinValue;
                    minY = int.MaxValue;
                    lastGlyph = null; 
                    offsets = [];
                    
                    skip = true;
                    break;
                case SPACE:
                    
                    widthAtLastSpace = currentWidth + (lastGlyph?.Width ?? 0);
                    heightAtLastSpace = maxY - minY;
                    spacing = wordSpacing * tokens[i].Data!.Value.AsT3.Width;
                    
                    newWidth = widthAtLastSpace + spacing; 
                    lastSpaceIndex = i;

                    if (newWidth > maxWidth) {
                        lines.Add(new LineData(
                            currentWidth + (lastGlyph?.Width ?? 0), 
                            maxY - minY, 
                            tokens.Skip(lineStartIndex).Take(i - lineStartIndex).ToArray(),
                            offsets.ToArray()
                        ));
                        
                        maxY = int.MinValue;
                        minY = int.MaxValue;
                        skip = true;
                        lastSpaceIndex = null;
                        lineStartIndex = i + 1;
                        currentWidth = 0;
                        lastGlyph = null;
                        offsets = [];
                        spacing = charSpacing;
                        break;
                    }
                    
                    offsets.Add(-1);

                    break;
                case INVALID or SIMPLE or AUTO_COMPOUND or LIGATURE: {
                    var type = tokens[i].Type;
                    var t = tokens[i].Data;

                    var g = type switch {
                        SIMPLE
                            => _simpleGlyphs[t!.Value.AsT0.Character].Glyph,
                       LIGATURE
                            => _ligatureGlyphs[t!.Value.AsT1.Ligature].Glyph,
                        AUTO_COMPOUND
                            => _autoCompoundGlyphs[t!.Value.AsT2.Second]
                                .GetGlyph(_simpleGlyphs[t.Value.AsT2.Main].Glyph),
                        INVALID
                            => _invalidTokenGlyph,
                        _
                            => throw new InvalidOperationException("Invalid token type.")
                    };

                    maxY = Math.Max(maxY, g.Height + g.BaselineOffset);
                    minY = Math.Min(minY, g.BaselineOffset);

                    int offset = lastGlyph != null ? GetGlyphOffset(lastGlyph, g, spacing) : 0;
                    
                    newWidth = currentWidth + offset + g.Width;
                    offsets.Add(offset);
                    spacing = charSpacing;
                    lastGlyph = g;
                    break;
                }
            }

            if (skip) continue;
            
            if (newWidth > maxWidth) {
                offsets.RemoveAt(offsets.Count - 1); 
                
                // sorry for the naming but i have absolutely no idea what the fuck these numbers mean...
                var a = lastSpaceIndex ?? i - 1; 
                int b = a - lineStartIndex;
                
                lines.Add(new LineData(
                    lastSpaceIndex == null ? currentWidth + (lastGlyph?.Width ?? 0) : widthAtLastSpace, 
                    lastSpaceIndex == null ? maxY - minY : heightAtLastSpace, 
                    tokens.Skip(lineStartIndex).Take(b).ToArray(),
                    offsets.Take(b).ToArray()
                ));
                
                i = a;
                lastSpaceIndex = null;

                lineStartIndex = i + 1;
                currentWidth = 0;
                maxY = int.MinValue;
                minY = int.MaxValue;
                offsets = [];
                lastGlyph = null;
                spacing = charSpacing;
                continue;
            }
            
            
            if (tokens[i].Type != SPACE) {
                currentWidth = newWidth - (lastGlyph?.Width ?? 0); 
            }
        }

        if (lineStartIndex < tokens.Length) {
            lines.Add(new LineData(
                currentWidth + (lastGlyph?.Width ?? 0), 
                maxY - minY,
                tokens.Skip(lineStartIndex).ToArray(),
                offsets.ToArray()
            ));
        }
        
        return lines.ToArray();
    }

    private readonly struct LineData {
        public int Width { get; }
        public int Height { get; } = 0;
        public Token[] Tokens { get; }
        public int[] Offsets { get; } = [];
        
        public LineData(int width, Token[] tokens) {
            Width = width;
            Tokens = tokens;
        }
        
        public LineData(int width, int height, Token[] tokens) {
            Width = width;
            Height = height;
            Tokens = tokens;
        }
        
        public LineData(int width, int height, Token[] tokens, int[] offsets) {
            Width = width;
            Height = height;
            Tokens = tokens;
            Offsets = offsets;
        }

        public override string ToString() {
            return $"LineData(Width: {Width}, Height: {Height}, Tokens: {Tokens.Length}, string: {
                Tokens.Select(t => t.Type switch {
                    AUTO_COMPOUND => t.Data!.Value.AsT2.Main.ToString() + t.Data.Value.AsT2.Second,
                    INVALID => "Invalid", 
                    SIMPLE => t.Data!.Value.AsT0.Character.ToString(),
                    LIGATURE => t.Data!.Value.AsT1.Ligature, 
                    SPACE => " ", 
                    NEWLINE => "\n", 
                    _ => throw new ArgumentOutOfRangeException()
                }).Aggregate((a, b) => a + b)
            })";
        }
    }
    
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
        _maxLigatureLength ??= _ligatureGlyphs.Select(kvp => kvp.Value.Ligature.Length).Prepend(0).Max();
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