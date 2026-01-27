// NeoKolors
// Copyright (c) 2025 KryKom

using System.Runtime.CompilerServices;
using Metriks;
using NeoKolors.Tui.Fonts.Exceptions;
using NeoKolors.Tui.Rendering;
using NeoKolors.Tui.Styles.Values;
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
        => PlaceString(str, canvas, new Size(maxWidth, int.MaxValue), new NKStyle());

    
    public void PlaceString(
        string str, ICharCanvas canvas, Rectangle bounds, NKStyle style,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign verticalAlign = VerticalAlign.TOP, bool overflow = false) 
    {
        var tokens = Tokenize(str, this);

        if (_info.SpacingInfo.IsMonospace)
            PlaceMono(tokens, canvas, bounds, style, horizontalAlign, verticalAlign, overflow);
        else 
            if (_info.SpacingInfo.AsVariable.Kerning)
                if (_info.SpacingInfo.AsVariable.LineKerning)
                    PlaceVariableKerningWithAlignWithLineKerning(tokens, canvas, bounds, style, horizontalAlign, verticalAlign, overflow);
                else
                    PlaceVariable_CharKerning(tokens, canvas, bounds, style, horizontalAlign, verticalAlign, overflow);
            else 
                PlaceVariable_NoKerning(tokens, canvas, bounds, style, horizontalAlign, verticalAlign, overflow);
    }

    // todo: support overflow
    
    private void PlaceMono(
        Token[] tokens,
        ICharCanvas canvas,
        Rectangle bounds,
        NKStyle style,
        HorizontalAlign horizontalAlign,
        VerticalAlign verticalAlign,
        bool overflow) 
    {
        var lines = SplitLines_Mono(tokens, bounds.Width);
        var lineSize = _info.SpacingInfo.AsMonospace.CharacterHeight + _info.LineSpacing;
        var charSize = _info.SpacingInfo.AsMonospace.CharacterWidth + _info.CharSpacing;
        
        var yOffset = verticalAlign switch {
            VerticalAlign.TOP => 0,
            VerticalAlign.CENTER => Math.Max(0, (bounds.Height - lineSize * lines.Length) / 2),
            VerticalAlign.BOTTOM => Math.Max(0, bounds.Height - lineSize * lines.Length),
            _ => throw new ArgumentOutOfRangeException()
        } + bounds.LowerY;

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

            var xOffset = computeXOffset(lineData.Width, bounds.Width) + bounds.LowerX;
            
            int x = xOffset;
            
            for (int j = 0; j < lineTokens.Length; j++) {
                var token = lineTokens[j];

                if (token.Type == SPACE) {
                    x += charSize;
                    continue;
                }
                
                var glyph = GetGlyph(token);

                int yp = y - glyph.BaselineOffset;
                
                canvas.Place(glyph.Glyph, new Point2D(x, yp));
                canvas.Style(style, new Point(x, y), glyph.Glyph);
                
                x += charSize;
            }
            
            y += lineSize;
        }
    }

    private void PlaceVariable_NoKerning(Token[] tokens,
        ICharCanvas canvas,
        Rectangle bounds,
        NKStyle style,
        HorizontalAlign horizontalAlign,
        VerticalAlign verticalAlign,
        bool overflow) 
    {
        var lines = SplitLines_Variable_NoKerning(tokens, bounds.Width);
        var wordSpacing = _info.SpacingInfo.AsVariable.WordSpacing;
        var lineSpacing = _info.LineSpacing;
        var charSpacing = _info.CharSpacing;
        
        var totalHeight = lines.Sum(l => l.Height) + lineSpacing * (lines.Length - 1);
        
        var yOffset = verticalAlign switch {
            VerticalAlign.TOP => 0,
            VerticalAlign.CENTER => Math.Max(0, (bounds.Height - totalHeight) / 2),
            VerticalAlign.BOTTOM => Math.Max(0, bounds.Height - totalHeight),
            _ => throw new ArgumentOutOfRangeException()
        } + bounds.LowerY; 

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

            var xOffset = computeXOffset(lineData.Width, bounds.Width) + bounds.LowerX;

            y += lineData.Height;
            int x = xOffset;
            
            for (int j = 0; j < lineTokens.Length; j++) {
                var token = lineTokens[j];

                if (token.Type == SPACE) {
                    x += wordSpacing * token.Data!.Value.AsT3.Width;
                    continue;
                }
                
                var glyph = GetGlyph(token);

                var yp = y - glyph.BaselineOffset - glyph.Height;
                
                canvas.Place(glyph.Glyph, new Point2D(x, yp));
                canvas.Style(style, new Point(x, yp), glyph.Glyph);
                
                x += glyph.Width + charSpacing;
            }
            
            y += lineSpacing;
        }
    } 

    private void PlaceVariable_CharKerning(
        Token[]         tokens,
        ICharCanvas     canvas,
        Rectangle       bounds,
        NKStyle         style,
        HorizontalAlign horizontalAlign,
        VerticalAlign   verticalAlign,
        bool            overflow) 
    {
        var lines = SplitLines_Variable_CharKerning(tokens, bounds.Width);
        var lineSpacing = _info.LineSpacing;

        var totalHeight = lines.Sum(l => l.Height) + lineSpacing * (lines.Length - 1);
        
        var yOffset = verticalAlign switch {
            VerticalAlign.TOP    => 0,
            VerticalAlign.CENTER => Math.Max(0, (bounds.Height - totalHeight) / 2),
            VerticalAlign.BOTTOM => Math.Max(0, bounds.Height - totalHeight),
            _ => throw new ArgumentOutOfRangeException()
        } + bounds.LowerY;

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

            var xOffset = computeXOffset(lineData.Width, bounds.Width) + bounds.LowerX;

            y += lineData.Height;
            int x = xOffset;
            
            for (int j = 0; j < lineTokens.Length; j++) {
                var token = lineTokens[j];

                if (token.Type == SPACE) {
                    continue;
                }

                var glyph = GetGlyph(token);

                x += lineData.Offsets[j];
                
                var idk = y - glyph.BaselineOffset - glyph.Height;
                
                canvas.Place(glyph.Glyph, new Point2D(x, idk));
                canvas.Style(style, new Point(x, idk), glyph.Glyph);
            }
            
            y += lineSpacing;
        }
    }
    
    // todo: line kerning
    
    private void PlaceVariableKerningWithAlignWithLineKerning(
        Token[]         tokens,
        ICharCanvas     canvas,
        Rectangle       bounds,
        NKStyle         style,
        HorizontalAlign horizontalAlign,
        VerticalAlign   verticalAlign,
        bool            overflow) 
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
                if (l1[x, y].Char == null) continue;
                
                firstGap = (l1.Height - y) - 1;
                break;
            }

            for (int y = 0; y < l2.Height; y++) {
                if (l2[x, y].Char == null) continue;
                
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
            if (l1[closestX, y].Char == null) continue;
            
            return y + 1 + spacing;
        }
        
        return l1.Height + spacing;
    }
    
    #endregion
    
    
    // --------------------------- SPLIT LINES --------------------------- //
    
    #region SPLIT LINES

    private LineData[] SplitLines_Mono(Token[] tokens, int maxWidth) {
        if (tokens.Length == 0)
            return [];
        
        List<LineData> lines = [];
        
        int lineStart = 0;
        int? lastSpaceIndex = null;
        int lastWidth = 0;
        int currentWidth = 0;

        int gw = _info.SpacingInfo.AsMonospace.CharacterWidth;
        
        for (int i = 0; i < tokens.Length; i++) {
            var t = tokens[i];
            switch (t.Type) {
                case SIMPLE or AUTO_COMPOUND or INVALID: {
                    currentWidth += gw;
                } break;
                case LIGATURE: {
                    currentWidth += t.AsLigature().Length * gw;
                } break;
                case SPACE: {
                    currentWidth += t.AsSpace().Width * gw;
                    lastSpaceIndex = i;
                } break;
                case NEWLINE: {
                     lines.Add(new LineData(currentWidth, tokens.InRange(lineStart, i).ToArray()));
                     lineStart = i + 1;
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (currentWidth > maxWidth) {
                if (lastSpaceIndex.HasValue) {
                    lines.Add(new LineData(
                        currentWidth,
                        tokens.InRange(lastSpaceIndex.Value, lastSpaceIndex.Value).ToArray()
                    ));

                    i = lastSpaceIndex.Value;
                    lineStart = lastSpaceIndex.Value + 1;
                }
                else if (lineStart == i) {
                    lines.Add(new LineData(
                        currentWidth,
                        [tokens[i]]
                    ));

                    lineStart++;
                }
                else {
                    lines.Add(new LineData(lastWidth, tokens.InRange(lineStart, i).ToArray()));
                }

                currentWidth = 0;
            }

            lastWidth = currentWidth;
        }

        if (lineStart < tokens.Length) {
            lines.Add(new LineData(
                currentWidth,
                tokens.Skip(lineStart).ToArray()
            ));
        }
        
        return lines.ToArray();
    }

    private LineData[] SplitLines_Variable_NoKerning(Token[] tokens, int maxWidth) {
        if (tokens.Length == 0)
            return [];
        
        // precompute widths and positions
        int[] widths = new int[tokens.Length];
        int[] up     = new int[tokens.Length];
        int[] dn     = new int[tokens.Length];

        for (int i = 0; i < tokens.Length; i++) {
            var t = tokens[i];
            
            if (t.Type is AUTO_COMPOUND or INVALID or LIGATURE or SIMPLE) {
                var g = GetGlyph(t);
                widths[i] = g.Width + _info.CharSpacing;
                up[i]     = g.BaselineOffset + g.Height;
                dn[i]     = g.BaselineOffset;
            }
            else {
                widths[i] = t.Type is SPACE ? t.AsSpace().Width * _info.SpacingInfo.AsVariable.WordSpacing : 0;
                up[i]     = int.MinValue;
                dn[i]     = int.MaxValue;
            }
        }
        
        // compute lines
        List<LineData> lines = [];
        
        int lineStart = 0;
        int? lastSpaceIndex = null;
        int lastSpaceWidth = 0;
        int lastWidth = 0;
        int currentWidth = 0;
        
        for (int i = 0; i < tokens.Length; i++) {
            var t = tokens[i];
            switch (t.Type) {
                case SIMPLE or AUTO_COMPOUND or INVALID or LIGATURE: {
                    currentWidth += widths[i];
                } break;
                case SPACE: {
                    lastSpaceWidth = currentWidth;
                    currentWidth += widths[i];
                    lastSpaceIndex = i;
                } break;
                case NEWLINE: {
                    lines.Add(new LineData(
                        currentWidth, 
                        up.InRange(lineStart, i).Max() - dn.InRange(lineStart, i).Min(),
                        tokens.InRange(lineStart, i).ToArray()
                    ));
                    lineStart = i + 1;
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (currentWidth > maxWidth) {
                if (lastSpaceIndex.HasValue) {
                    var si = lastSpaceIndex.Value;
                    
                    lines.Add(new LineData(
                        lastSpaceWidth,
                        up.InRange(lineStart, si).Max() - dn.InRange(lineStart, si).Min(),
                        tokens.InRange(lineStart, si).ToArray()
                    ));

                    i = si;
                    lineStart = si + 1;
                    lastSpaceIndex = null;
                }
                else if (lineStart == i) {
                    lines.Add(new LineData(
                        currentWidth,
                        up[i] - dn[i],
                        [tokens[i]]
                    ));

                    lineStart++;
                }
                else {
                    lines.Add(new LineData(
                        lastWidth, 
                        up.InRange(lineStart, i).Max() - dn.InRange(lineStart, i).Min(), 
                        tokens.InRange(lineStart, i).ToArray()
                    ));
                }

                currentWidth = 0;
            }

            lastWidth = currentWidth;
        }

        if (lineStart < tokens.Length) {
            lines.Add(new LineData(
                currentWidth,
                up.Skip(lineStart).Max() - dn.Skip(lineStart).Min(),
                tokens.Skip(lineStart).ToArray()
            ));
        }
        
        return lines.ToArray();
    }

    private LineData[] SplitLines_Variable_CharKerning(Token[] tokens, int maxWidth) {
        if (tokens.Length == 0)
            return [];
        
        // precompute offsets
        int[] offsets = new int[tokens.Length];
        int[] widths  = new int[tokens.Length];
        int[] up      = new int[tokens.Length];
        int[] dn      = new int[tokens.Length];

        // fill first cell
        offsets[0] = 0;
        IGlyph? lg = null;

        if (tokens[0].Type is AUTO_COMPOUND or INVALID or LIGATURE or SIMPLE) {
            lg = GetGlyph(tokens[0]);
            widths[0] = lg.Width;
            up[0]     = lg.BaselineOffset + lg.Height;
            dn[0]     = lg.BaselineOffset;
        }
        else {
            widths[0] = 0;
            up[0]     = int.MinValue;
            dn[0]     = int.MaxValue;
        }
        
        int cs = _info.CharSpacing;
        int spacing = cs;
        
        // fill all other cells
        for (int i = 1; i < tokens.Length; i++) {
            switch (tokens[i].Type) {
                case AUTO_COMPOUND or INVALID or LIGATURE or SIMPLE: {
                    var g      = GetGlyph(tokens[i]);
                    offsets[i] = lg == null ? 0 : GetGlyphOffset(lg, g, spacing);
                    widths[i]  = g.Width;
                    up[i]      = g.BaselineOffset + g.Height;
                    dn[i]      = g.BaselineOffset;
                    spacing    = cs;
                    lg         = g;
                } break;
                case SPACE: {
                    up[i]      = int.MinValue;
                    dn[i]      = int.MaxValue;
                    spacing    = cs;
                } break;
                case NEWLINE: {
                    lg = null;
                    up[i] = int.MinValue;
                    dn[i] = int.MaxValue;
                    spacing = cs;
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        // compute lines
        List<LineData> lines = [];
        
        int lineStart = 0;
        int? lastSpaceIndex = null;
        int lastSpaceWidth = 0;
        int lastWidth = 0;
        int currentWidth = 0;
        int currentOffset = 0;
        
        for (int i = 0; i < tokens.Length; i++) {
            var t = tokens[i];
            switch (t.Type) {
                case AUTO_COMPOUND or INVALID or LIGATURE or SIMPLE: {
                    currentWidth = currentOffset + widths[i];
                    currentOffset += offsets[i];
                } break;
                case SPACE: {
                    lastSpaceIndex = i;
                    lastSpaceWidth = currentWidth;
                } break;
                case NEWLINE: {
                    lines.Add(new LineData(
                        currentWidth, 
                        up     .InRange(lineStart, i).Max() - dn.InRange(lineStart, i).Min(),
                        tokens .InRange(lineStart, i).ToArray(),
                        offsets.InRange(lineStart, i).ToArray()
                    ));
                    
                    lineStart = i + 1;
                    currentOffset = 0;
                    currentWidth  = 0;
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (currentWidth > maxWidth) {
                if (lastSpaceIndex.HasValue) {
                    var j = lastSpaceIndex.Value;
                    
                    lines.Add(new LineData(
                        lastSpaceWidth,
                        up     .InRange(lineStart, j).Max() - dn.InRange(lineStart, j).Min(),
                        tokens .InRange(lineStart, j).ToArray(),
                        offsets.InRange(lineStart, j).ToArray()
                    ));

                    i = lastSpaceIndex.Value;
                    lineStart = lastSpaceIndex.Value + 1;
                }
                else if (lineStart == i) {
                    lines.Add(new LineData(
                        currentWidth,
                        up[i] - dn[i],
                        [tokens [i]],
                        [offsets[i]]
                    ));

                    lineStart++;
                }
                else {
                    lines.Add(new LineData(
                        lastWidth,
                        up     .InRange(lineStart, i).Max() - dn.InRange(lineStart, i).Min(),
                        tokens .InRange(lineStart, i).ToArray(),
                        offsets.InRange(lineStart, i).ToArray()
                    ));
                    
                    lineStart = i;
                    i -= 1;
                }

                lastSpaceIndex = null;
                lastSpaceWidth = 0;
                currentOffset  = 0;

                currentWidth = 0;
            }

            lastWidth = currentWidth;
        }

        // add remaining tokens
        if (lineStart < tokens.Length) {
            lines.Add(new LineData(
                currentOffset + widths[tokens.Length - 1],
                up     .Skip(lineStart).Max() - dn.Skip(lineStart).Min(),
                tokens .Skip(lineStart).ToArray(),
                offsets.Skip(lineStart).ToArray()
            ));
        }
        
        return lines.ToArray();
    }
    
    #endregion SPLIT LINES NEW
    
    private record LineData {
        
        /// <summary>
        /// Represents the width of a line of text in chars.
        /// </summary>
        /// <remarks>
        /// The <c>Width</c> property defines the horizontal extent of a line, measured in units used
        /// by the layout engine. It is crucial for determining the alignment and horizontal positioning
        /// of text elements within the bounding area during rendering.
        /// </remarks>
        public int Width { get; }

        /// <summary>
        /// Represents the height of a text line in chars.
        /// </summary>
        /// <remarks>
        /// The <c>Height</c> property is used to specify the vertical size of a line. It is calculated
        /// based on the font metrics and any additional spacing applied during text layout. This
        /// value is essential for determining the total height of multiple lines of text and for
        /// aligning text within a given area.
        /// </remarks>
        public int Height { get; } = 0;

        /// <summary>
        /// A collection of tokens representing glyphs for a line of text.
        /// </summary>
        /// <remarks>
        /// The <c>Tokens</c> property provides information about the glyphs associated with the text,
        /// including simple glyphs, ligatures, and auto-compound glyphs. These tokens are used for
        /// rendering text with specified alignment, spacing, and kerning.
        /// </remarks>
        public Token[] Tokens { get; }

        /// <summary>
        /// Represents the relative horizontal offsets for tokens within a text line.
        /// </summary>
        /// <remarks>
        /// The <c>Offsets</c> property provides an array of integer values indicating the horizontal displacement
        /// for each token in a line of text. These offsets are used to adjust the positioning of tokens during rendering,
        /// ensuring precise placement of glyphs based on alignment, spacing, and other layout considerations.
        /// Each value in the array corresponds to a specific token in the <c>Tokens</c> collection.
        /// </remarks>
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
                    INVALID => "\uf059", 
                    SIMPLE => t.Data!.Value.AsT0.Character.ToString(),
                    LIGATURE => t.Data!.Value.AsT1.Ligature, 
                    SPACE => " ", 
                    NEWLINE => "\n", 
                    _ => throw new ArgumentOutOfRangeException()
                }).Aggregate((a, b) => a + b)
            })";
        }
    }
    
    
    // --------------------------- SIZE COMP --------------------------- //
    
    #region SIZE COMP
    
    public Size GetMinSize(string str) {
        var tokens = Tokenize(str, this);
        var lines = tokens.Split(t => t.Type is NEWLINE or SPACE);

        int maxWidth = 0;

        for (int i = 0; i < lines.Length; i++) {
            var canv = new NKCharCanvas();
            PlaceString(str, canv);
            maxWidth = Math.Max(maxWidth, canv.Width);
        }
        
        var canvas = new NKCharCanvas();
        PlaceString(str, canvas);
        
        return new Size(maxWidth, canvas.Height);
    }
    
    public Size GetSize(string str) {
        var canv = new NKCharCanvas();
        PlaceString(str, canv);
        return new Size(canv.Width, canv.Height);
    }
    
    public Size GetSize(string str, int maxWidth) {
        if (maxWidth < 0) throw new ArgumentOutOfRangeException(nameof(maxWidth), "maxWidth must be >= 0.");
        if (maxWidth == 0) return Size.Zero;
        
        var canv = new NKCharCanvas(0, 0, true);
        PlaceString(str, canv, maxWidth);
        return new Size(canv.Width, canv.Height);
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

    private IGlyph GetAutoCompoundGlyph(AutoCompoundToken token) 
        => _autoCompoundGlyphs[token.Second].GetGlyph(_simpleGlyphs[token.Main].Glyph);

    private IGlyph GetGlyph(Token token) {
        return token.Type switch {
            INVALID          => _invalidTokenGlyph,
            SIMPLE           => _simpleGlyphs[token.Data!.Value.AsT0.Character].Glyph,
            LIGATURE         => _ligatureGlyphs[token.Data!.Value.AsT1.Ligature].Glyph,
            AUTO_COMPOUND    => GetAutoCompoundGlyph(token.AsAutoCompound()),
            SPACE or NEWLINE => throw new InvalidOperationException(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private int GetWidth(Token token) {
        return token.Type switch {
            INVALID       => _invalidTokenGlyph.Width,
            SIMPLE        => _simpleGlyphs[token.Data!.Value.AsT0.Character].Glyph.Width,
            LIGATURE      => _ligatureGlyphs[token.Data!.Value.AsT1.Ligature].Glyph.Width,
            AUTO_COMPOUND => GetAutoCompoundGlyph(token.AsAutoCompound()).Width,
            NEWLINE       => 0,
            SPACE         => token.AsSpace().Width * (_info.SpacingInfo.IsVariable 
                ? _info.SpacingInfo.AsVariable.WordSpacing 
                : _info.SpacingInfo.AsMonospace.CharacterWidth),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    #if DEBUG || WIP
    
    public IGlyph GetSimple(char c) => _simpleGlyphs[c].Glyph;
    public IGlyph GetLigature(string ligature) => _ligatureGlyphs[ligature].Glyph;
    public IGlyph GetAutoCompound(char main, char secondary) => 
        _autoCompoundGlyphs[main].GetGlyph(_simpleGlyphs[secondary].Glyph);
    
    #endif
}