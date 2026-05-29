// NeoKolors
// Copyright (c) krystof 2026

using System.Runtime.CompilerServices;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Extensions;
using NeoKolors.Tui.Core;
using static NeoKolors.Tui.Fonts.NKFont.TokenType;
using static NeoKolors.Tui.Fonts.FontProportionsInfo.ProportionType;
using Numeric = NeoKolors.Extensions.Numeric;

namespace NeoKolors.Tui.Fonts;

public class NKFont : IExtendedAsciiFont {
    
    // ============================ private fields ============================ //

    #region Private Fields
    
    // ----- static ----- //
    
    #region Static

    /// <summary>
    /// Represents the default glyph used when an unknown symbol is encountered.
    /// This glyph is typically a fallback representation, defaulting to a single
    /// character '?' displayed within the font system.
    /// </summary>
    private static readonly NKGlyph DEFAULT_UNKNOWN_SYMBOL_GLYPH = new(new[,] { { GlyphCell.Char('?') } }, 0);

    /// <summary>
    /// Represents a static logger instance used to log diagnostic and runtime information
    /// related to the <see cref="NKFont"/> class. Provides functionalities for various logging
    /// levels, including critical and error logs, to aid in debugging and application monitoring.
    /// </summary>
    private static readonly NKLogger LOGGER = NKDebug.GetLogger<NKFont>();
    
    #endregion
    
    /// <summary>
    /// Holds the font information and settings for the current font instance.
    /// Provides details such as font name, ligature support, leading, and letter spacing.
    /// </summary>
    private readonly NKFontInfo _fontInfo;

    /// <summary>
    /// Stores a collection of glyphs mapped by their corresponding symbols.
    /// Used to represent the visual representations of characters, ligatures, or symbols
    /// within the context of the font's design.
    /// </summary>
    private readonly Dictionary<NKGlyphSymbol, NKGlyph> _glyphs;

    /// <summary>
    /// Defines the maximum allowed length for ligatures in the font.
    /// Ligatures are sequences of characters that are combined into a single glyph
    /// for rendering purposes, and this value limits their length to ensure efficient processing.
    /// </summary>
    private readonly int _maxLigatureLength;

    /// <summary>
    /// Stores a collection of available ligature strings in the font.
    /// Used for efficient ligature detection during string tokenization.
    /// </summary>
    private readonly HashSet<string> _ligatureStrings;

    private record EffectGlyphInfo {
        public NKGlyph Glyph              { get; }
        public bool    RenderAboveLetters { get; }

        public EffectGlyphInfo(NKGlyph glyph, bool renderAboveLetters) {
            Glyph              = glyph;
            RenderAboveLetters = renderAboveLetters;
        }
    }

    /// <summary>
    /// Stores the glyph representation for strikethrough styling in the font.
    /// This glyph is used to render strikethrough effects on text elements.
    /// </summary>
    private readonly EffectGlyphInfo? _strikethrough;

    /// <summary>
    /// Stores the glyph representation for underlining text in the font.
    /// This glyph defines how underlines are rendered within the text-based
    /// user interface, adhering to the font's design principles and style.
    /// </summary>
    private readonly EffectGlyphInfo? _underline;

    /// <summary>
    /// Stores the glyph that should be used in place of unknown or unsupported symbols.
    /// Provides a fallback representation for symbols that cannot be rendered by the font.
    /// </summary>
    private readonly NKGlyph _unknownSymbolGlyph;

    private readonly int _fontBaseline;
    
    #endregion

    // ============================ public fields ============================ //

    #region Public Fields

    /// <summary>
    /// Provides access to the font information associated with the current font instance.
    /// Encapsulates details such as the font name, ligature support, leading, and letter spacing.
    /// </summary>
    public NKFontInfo Info {
        get  => _fontInfo;
        init => _fontInfo = value;
    }

    /// <summary>
    /// Gets the name of the font as defined in the associated font information.
    /// Provides a textual identifier for the font instance.
    /// </summary>
    public string Name => _fontInfo.Name;

    public Dictionary<NKGlyphSymbol, NKGlyph> Glyphs => _glyphs;
    public NKGlyph UnknownSymbolGlyph => _unknownSymbolGlyph;
    public NKGlyph? StrikethroughGlyph => _strikethrough?.Glyph;
    public bool RenderStrikethroughAboveLetters => _strikethrough?.RenderAboveLetters ?? true;
    public NKGlyph? UnderlineGlyph => _underline?.Glyph;
    public bool RenderUnderlineAboveLetters => _underline?.RenderAboveLetters ?? true;

    #endregion


    // ============================ constructors ============================ //
    
    #region Constructors
    
    /// <summary>
    /// Represents a font used in the NeoKolors text-based UI framework.
    /// Provides functionality for rendering strings and glyphs onto a canvas,
    /// as well as for calculating text dimensions and handling font-specific behaviors.
    /// </summary>
    public NKFont(
        NKFontInfo                         info, 
        Dictionary<NKGlyphSymbol, NKGlyph> glyphs, 
        NKGlyph?                           unknownSymbolGlyph              = null,
        NKGlyph?                           strikethroughGlyph              = null,
        bool                               renderStrikethroughAboveLetters = true,
        NKGlyph?                           underlineGlyph                  = null,
        bool                               renderUnderlineAboveLetters     = true) 
    {
        _fontInfo           = info;
        _glyphs             = glyphs;
        _unknownSymbolGlyph = unknownSymbolGlyph ?? DEFAULT_UNKNOWN_SYMBOL_GLYPH;
        
        _strikethrough = strikethroughGlyph is not null
            ? new EffectGlyphInfo(strikethroughGlyph, renderStrikethroughAboveLetters)
            : null;
        
        _underline = underlineGlyph is not null
            ? new EffectGlyphInfo(underlineGlyph, renderUnderlineAboveLetters)
            : null;
        
        _ligatureStrings = [];
        var maxLigatureLength = -1;
        
        foreach (var g in glyphs.Keys.Where(g => g.IsLigature)) {
            maxLigatureLength = Math.Max(maxLigatureLength, g.LigatureLength);
            _ligatureStrings.Add(g.LigatureSymbol);
        }
        
        _maxLigatureLength = maxLigatureLength;
        var standardGlyphs = glyphs.Where(g => g.Key.IsSimple && g.Key.Styles.Styles == TextStyles.NONE && 
            ((g.Key.SimpleSymbol >= 'A' && g.Key.SimpleSymbol <= 'Z') || (g.Key.SimpleSymbol >= 'a' && g.Key.SimpleSymbol <= 'z')));
        _fontBaseline = standardGlyphs.Any() 
            ? standardGlyphs.Max(g => g.Value.BaselineOffset + g.Value.Height) 
            : (glyphs.Count > 0 ? glyphs.Values.Max(g => g.BaselineOffset + g.Height) : 0);
    }
    
    #endregion
    
    // ============================ rendering impl ============================ //
    
    #region Rendering Implementation
    
    public void PlaceString(string str, ICharCanvas canvas) => 
        PlaceString(str, canvas, int.MaxValue);

    public void PlaceString(string str, ICharCanvas canvas, int maxWidth) =>
        PlaceString(str, canvas, new Area2D(Point2D.Zero, new Point2D(maxWidth, int.MaxValue)));

    public void PlaceString(
        string str,
        ICharCanvas canvas,
        Area2D bounds,
        NKStyle style,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign verticalAlign = VerticalAlign.TOP,
        bool overflow = false)
    {
        PlaceString(new AnsiString(str, style), canvas, bounds, horizontalAlign, verticalAlign, overflow);
    }
    
    public void PlaceString(AnsiString str, ICharCanvas canvas) => 
        PlaceString(str, canvas, int.MaxValue);
    
    public void PlaceString(AnsiString str, ICharCanvas canvas, int maxWidth) =>
        PlaceString(str, canvas, new Area2D(Point2D.Zero, new Point2D(maxWidth, int.MaxValue)));
    
    public void PlaceString(
        AnsiString str,
        ICharCanvas canvas,
        Area2D bounds,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign verticalAlign = VerticalAlign.TOP,
        bool overflow = false) 
    {
        var options = new TextRenderingOptions(horizontalAlign, verticalAlign, overflow);
        PlaceString(str, canvas, bounds, options);
    }
    
    public void PlaceString(string str, ICharCanvas canvas, Area2D bounds, TextRenderingOptions options) {
        PlaceString(new AnsiString(str), canvas, bounds, options);
    }
    
    public void PlaceString(AnsiString str, ICharCanvas canvas, Area2D bounds, TextRenderingOptions options) {
        var tokens = Tokenize(str);
        if (tokens.Length == 0) return;
        
        var wordWrap = options.WordWrap;
        var wrapWidth = bounds.Size.X;
        
        var lines = ComputeLines(tokens, wrapWidth, wordWrap);
        if (lines.Length == 0) return;
        
        var totalHeight = lines.Length * _fontInfo.Leading;
        
        var yOffset = options.VerticalAlign switch {
            VerticalAlign.TOP => 0,
            VerticalAlign.CENTER => Math.Max(0, (bounds.Size.Y - totalHeight) / 2),
            VerticalAlign.BOTTOM => Math.Max(0, bounds.Size.Y - totalHeight),
            _ => throw new ArgumentOutOfRangeException()
        } + bounds.LowerY;
        
        Func<int, int, int> computeXOffset = options.HorizontalAlign switch {
            HorizontalAlign.LEFT   => static (_, _)          => 0,
            HorizontalAlign.CENTER => static (width, bounds) => Math.Max(0, (bounds - width) / 2),
            HorizontalAlign.RIGHT  => static (width, bounds) => Math.Max(0, bounds - width),
            _ => throw new ArgumentOutOfRangeException(nameof(options.HorizontalAlign), options.HorizontalAlign, null)
        };
        
        for (int i = 0; i < lines.Length; i++) {
            var line = lines[i];
            var xOffset = computeXOffset(line.Width, bounds.Size.X) + bounds.LowerX;
            var lineBaseline = yOffset + i * _fontInfo.Leading + _fontBaseline - 1;
            
            int x = xOffset;
            
            for (int j = 0; j < line.Tokens.Length; j++) {
                var token = line.Tokens[j];
                
                if (token.Type == SPACE) {
                    continue;
                }
                
                var glyph = GetGlyphFromToken(token);
                
                if (line.Offsets != null && j < line.Offsets.Length) {
                    x += line.Offsets[j];
                }
                
                bool shouldRender = !token.FullStyle.Styles.HasFlag(TextStyles.INVISIBLE);

                if (token.FullStyle.Styles.HasFlag(TextStyles.BLINK)) {
                    if (DateTime.Now.Second % 2 != 0) {
                        shouldRender = false;
                    }
                }
                
                var glyphStyles = token.Symbol?.Styles.Styles ?? TextStyles.NONE;
                
                // 1. Underline before
                if (_underline != null && !_underline.RenderAboveLetters && token.FullStyle.Styles.HasFlag(TextStyles.UNDERLINE) && !glyphStyles.HasFlag(TextStyles.UNDERLINE)) {
                    RenderEffect(canvas, _underline.Glyph!, x, glyph.Width, lineBaseline, token.FullStyle, bounds, options.Overflow);
                }
                
                // 2. Strikethrough before
                if (_strikethrough != null && !_strikethrough.RenderAboveLetters && token.FullStyle.Styles.HasFlag(TextStyles.STRIKETHROUGH) && !glyphStyles.HasFlag(TextStyles.STRIKETHROUGH)) {
                    RenderEffect(canvas, _strikethrough.Glyph!, x, glyph.Width, lineBaseline, token.FullStyle, bounds, options.Overflow);
                }
                
                // 3. Render the letter itself
                if (shouldRender) {
                    var yp = lineBaseline - glyph.BaselineOffset - glyph.Height + 1;
                    var finalStyle = token.FullStyle;
                    if (glyphStyles.HasFlag(TextStyles.NEGATIVE)) {
                        finalStyle = finalStyle with { Styles = finalStyle.Styles & ~TextStyles.NEGATIVE };
                    }
                    
                    for (int gx = 0; gx < glyph.Width; gx++) {
                        int cx = x + gx;
                        
                        for (int gy = 0; gy < glyph.Height; gy++) {
                            int cy = yp + gy;
                            
                            if (!options.Overflow) {
                                if (cx < bounds.LowerX || cx > bounds.HigherX || cy < bounds.LowerY || cy > bounds.HigherY) {
                                    continue;
                                }
                            }
                            
                            if (cx < 0 || cx >= canvas.Width || cy < 0 || cy >= canvas.Height) {
                                continue;
                            }
                            
                            var cell = glyph.Glyph[gx, gy];
                            if (cell.Type == GlyphCellType.BACKGROUND) {
                                continue;
                            }
                            
                            char? cellChar = cell.Type switch {
                                GlyphCellType.FOREGROUND => ' ',
                                GlyphCellType.CHARACTER => cell.Character,
                                _ => null
                            };
                            
                            var canvasCell = canvas[cx, cy];
                            canvasCell.Char = cellChar;
                            canvasCell.Style = canvasCell.Style.Override(finalStyle);
                        }
                    }
                }
                
                // 4. Underline after
                if (_underline != null && _underline.RenderAboveLetters && token.FullStyle.Styles.HasFlag(TextStyles.UNDERLINE) && !glyphStyles.HasFlag(TextStyles.UNDERLINE)) {
                    RenderEffect(canvas, _underline.Glyph!, x, glyph.Width, lineBaseline, token.FullStyle, bounds, options.Overflow);
                }
                
                // 5. Strikethrough after
                if (_strikethrough != null && _strikethrough.RenderAboveLetters && token.FullStyle.Styles.HasFlag(TextStyles.STRIKETHROUGH) && !glyphStyles.HasFlag(TextStyles.STRIKETHROUGH)) {
                    RenderEffect(canvas, _strikethrough.Glyph!, x, glyph.Width, lineBaseline, token.FullStyle, bounds, options.Overflow);
                }
            }
        }
    }
    
    private void RenderEffect(
        ICharCanvas canvas, 
        NKGlyph effectGlyph, 
        int startX, 
        int width, 
        int lineBaseline, 
        NKStyle style, 
        Area2D bounds, 
        bool overflow) 
    {
        var yp = lineBaseline - effectGlyph.BaselineOffset - effectGlyph.Height + 1;
        
        for (int dx = 0; dx < width; dx++) {
            int cx = startX + dx;
            int col = dx % effectGlyph.Width;
            
            for (int gy = 0; gy < effectGlyph.Height; gy++) {
                int cy = yp + gy;
                
                if (!overflow) {
                    if (cx < bounds.LowerX || cx > bounds.HigherX || cy < bounds.LowerY || cy > bounds.HigherY) {
                        continue;
                    }
                }
                
                if (cx < 0 || cx >= canvas.Width || cy < 0 || cy >= canvas.Height) {
                    continue;
                }
                
                var cell = effectGlyph.Glyph[col, gy];
                if (cell.Type == GlyphCellType.BACKGROUND) {
                    continue;
                }
                
                var cellChar = cell.Type == GlyphCellType.CHARACTER ? cell.Character : ' ';
                
                var canvasCell = canvas[cx, cy];
                canvasCell.Char = cellChar;
                canvasCell.Style = canvasCell.Style.Override(style);
            }
        }
    }
    
    #endregion
    
    // ============================ measuring impl ============================ //

    #region Measuring Implementation

    public Size2D GetMinSize(string str) => GetMinSize(new AnsiString(str));

    public Size2D GetSize(string str) => GetSize(str, int.MaxValue);

    public Size2D GetSize(string str, int maxWidth) => GetSize(new AnsiString(str), maxWidth);

    public Size2D GetMinSize(AnsiString str) {
        var tokens = Tokenize(str);
        var words  = tokens.Split(t => t.Type is NEWLINE or SPACE);
        
        var maxWordWidth = 0;

        for (int i = 0; i < words.Length; i++) {
            maxWordWidth = Math.Max(maxWordWidth, ComputeLineWidth(words[i]));
        }

        return GetSize(tokens, maxWordWidth);
    }

    public Size2D GetSize(AnsiString str) => GetSize(str, int.MaxValue);

    public Size2D GetSize(AnsiString str, int maxWidth) {
        return GetSize(Tokenize(str), maxWidth);
    }

    private Size2D GetSize(Token[] tokens, int maxWidth) {
        var lines = ComputeLines(tokens, maxWidth);

        var w = 0;

        for (int i = 0; i < lines.Length; i++) {
            var l = lines[i];
            
            w = Math.Max(w, l.Width);
        }
        
        return new Size2D(w, _fontInfo.Leading * lines.Length);
    }

    private int ComputeLineWidth(Token[] tokens) {
        return ComputeLineWidth(
            tokens, 
            _fontInfo.ProportionType switch {
                MONOSPACED          => GetGlyphOffset_Mono,
                VARIABLE | ~KERNING => GetGlyphOffset_Variable,
                VARIABLE |  KERNING => GetGlyphOffset_Kerning,
                _                   => throw new ArgumentOutOfRangeException(nameof(_fontInfo.ProportionType))
            }
        );
    }

    private int ComputeLineWidth(Token[] tokens, Func<NKGlyph, NKGlyph, int, int> getOffset) {
        var      co = 0;
        var      cw = 0;
        NKGlyph? lg = null;
        var      sp = _fontInfo.LetterSpacing;
        
        for (int i = 0; i < tokens.Length; i++) {
            var t = tokens[i];

            switch (t.Type) {
                case LIGATURE or LETTER or INVALID: {
                    var g = GetGlyphFromToken(t);
                    
                    if (lg is null) {
                        cw = g.Width;
                        lg = g;
                        break;
                    }

                    co += getOffset(lg, g, sp);
                    cw  = co + g.Width;
                    lg  = g;
                    sp  = _fontInfo.LetterSpacing;
                } break;
                case SPACE: {
                    sp = t.Length * _fontInfo.WordSpacing;
                } break;
                case NEWLINE: {
                    // ignore?  
                } break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return cw;
    }
    
    private RenderedLineInfo[] ComputeLines(Token[] tokens, int maxWidth, WordWrapType wordWrap = WordWrapType.WORD) {
        return _fontInfo.ProportionType switch {
            MONOSPACED          => ComputeLines_Mono    (tokens, maxWidth, wordWrap),
            VARIABLE | ~KERNING => ComputeLines_Variable(tokens, maxWidth, wordWrap),
            VARIABLE |  KERNING => ComputeLines_Kerning (tokens, maxWidth, wordWrap),
            _                   => throw new ArgumentOutOfRangeException()
        };
    }
    
    private RenderedLineInfo[] ComputeLines(Token[] tokens, int maxWidth, Func<NKGlyph, NKGlyph, int, int> getOffset, WordWrapType wordWrap) {
        
        // return empty if there are no tokens
        if (tokens.Length == 0)
            return [];
        
        int[] offsets = new int[tokens.Length];
        int[] widths  = new int[tokens.Length];

        // set values for the first token
        offsets[0] = 0;
        var first = tokens[0];

        widths[0] = first.Type switch {
            INVALID  => _unknownSymbolGlyph.Width,
            LETTER   => _glyphs[first.Symbol!.Value].Width,
            LIGATURE => _glyphs[first.Symbol!.Value].Width,
            SPACE    => 0,
            NEWLINE  => 0,
            _        => throw new ArgumentOutOfRangeException()
        };

        // last set glyph
        var lg = first.Type switch {
            LETTER or LIGATURE => _glyphs[first.Symbol!.Value],
            INVALID                      => _unknownSymbolGlyph,
            _                                      => null
        };
        
        int cs      = _fontInfo.LetterSpacing;
        int spacing = first.Type == SPACE ? first.Length * _fontInfo.WordSpacing : 0;
        
        // set values for the other tokens
        for (int i = 1; i < tokens.Length; i++) {
            var ct = tokens[i];
            
            switch (ct.Type) {
                case INVALID:
                case LETTER:
                case LIGATURE: {
                    var cg = GetGlyphFromToken(ct);
                    
                    offsets[i] = lg == null ? spacing : getOffset(lg, cg, spacing == 0 ? cs : spacing);
                    widths[i]  = cg.Width;
                    spacing    = 0;
                    lg         = cg;
                } break;
                case SPACE: {
                    spacing += ct.Length * _fontInfo.WordSpacing;
                } break;
                case NEWLINE: {
                    lg      = null;
                    spacing = 0;
                } break;
                default: {
                    LOGGER.Crit($"Unexpected token type '{(int)ct.Type}'. What the hell bro?");
                    throw new ArgumentOutOfRangeException(nameof(ct.Type));
                }
            }    
        }
        
        // compute lines
        List<RenderedLineInfo> lines = [];
        
        int lineStart = 0;
        int? lastSpaceIndex = null;
        int lastSpaceWidth = 0;
        int lastWidth = 0;
        int currentWidth = 0;
        int currentOffset = 0;
        
        for (int i = 0; i < tokens.Length; i++) {
            var t = tokens[i];
            switch (t.Type) {
                case INVALID or LETTER or LIGATURE: {
                    currentWidth = currentOffset + widths[i];
                    currentOffset += offsets[i];
                } break;
                case SPACE: {
                    lastSpaceIndex = i;
                    lastSpaceWidth = currentWidth;
                } break;
                case NEWLINE: {
                    lines.Add(new RenderedLineInfo(
                        currentWidth, 
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

            if (wordWrap != WordWrapType.NONE && currentWidth > maxWidth) {
                if (wordWrap == WordWrapType.WORD && lastSpaceIndex.HasValue) {
                    var j = lastSpaceIndex.Value;
                    
                    lines.Add(new RenderedLineInfo(
                        lastSpaceWidth,
                        tokens .InRange(lineStart, j).ToArray(),
                        offsets.InRange(lineStart, j).ToArray()
                    ));

                    i = lastSpaceIndex.Value;
                    lineStart = lastSpaceIndex.Value + 1;
                }
                else if (lineStart == i) {
                    lines.Add(new RenderedLineInfo(
                        currentWidth,
                        [tokens [i]],
                        [offsets[i]]
                    ));

                    lineStart++;
                }
                else {
                    lines.Add(new RenderedLineInfo(
                        lastWidth,
                        tokens .InRange(lineStart, i).ToArray(),
                        offsets.InRange(lineStart, i).ToArray()
                    ));
                    
                    lineStart = i;
                    i -= 1;
                }

                lastSpaceIndex = null;
                lastSpaceWidth = 0;
                currentOffset  = 0;
                currentWidth   = 0;
            }

            lastWidth = currentWidth;
        }

        // add remaining tokens
        if (lineStart < tokens.Length) {
            lines.Add(new RenderedLineInfo(
                currentOffset + widths[tokens.Length - 1],
                tokens .Skip(lineStart).ToArray(),
                offsets.Skip(lineStart).ToArray()
            ));
        }
        
        return lines.ToArray();
    }
    
    // ----- Variable - With Kerning ----- //
    
    #region Kerning 
    
    private RenderedLineInfo[] ComputeLines_Kerning(Token[] tokens, int maxWidth, WordWrapType wordWrap) {
        return ComputeLines(tokens, maxWidth, GetGlyphOffset_Kerning, wordWrap);
    }

    private static int GetGlyphOffset_Kerning(NKGlyph left, NKGlyph right, int spacing) {
        int closest  = int.MaxValue;
        int closestY = -1;

        var startY = Math.Max(left.BaselineOffset,               right.BaselineOffset);
        var endY   = Math.Min(left.BaselineOffset + left.Height, right.BaselineOffset + right.Height);
        
        for (int y = startY; y < endY; y++) { // y is baseline offset
            int firstGap = 0;
            int secondGap = 0;

            for (int x = left.Width - 1; x >= 0; x--) {
                if (left.Glyph[x, CalcY(left, y)].Type == GlyphCellType.BACKGROUND) 
                    continue;
                
                firstGap = left.Width - x - 1;
                break;
            }

            for (int x = 0; x < right.Width; x++) {
                if (right.Glyph[x, CalcY(right, y)].Type == GlyphCellType.BACKGROUND)
                    continue;
                
                secondGap = x;
                break;
            }
            
            int gap = firstGap + secondGap;

            if (gap >= closest) 
                continue;
            
            closest = gap;
            closestY = y;
        }
        
        if (closest == int.MaxValue) return left.Width;
        
        for (int x = left.Width - 1; x >= 0; x--) {
            if (left.Glyph[x, CalcY(left, closestY)].Type == GlyphCellType.BACKGROUND)
                continue;
            
            return x + 1 - GetSecondOffset() + spacing;
        }
        
        return left.Width;

        // ----- local methods ----- //
        
        int GetSecondOffset() {
            for (int x = 0; x < right.Width; x++) {
                if (right.Glyph[x, CalcY(right, closestY)].Type == GlyphCellType.BACKGROUND)
                    continue;
                
                return x;
            }
            
            return 0;
        }

        static int CalcY(NKGlyph g, int y) {
            return g.Height - (y - g.BaselineOffset) - 1;
        }
    }

    #endregion

    // ----- Variable - No Kerning ----- //
    
    #region Variable

    private RenderedLineInfo[] ComputeLines_Variable(Token[] tokens, int maxWidth, WordWrapType wordWrap) {
        return ComputeLines(tokens, maxWidth, GetGlyphOffset_Variable, wordWrap);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetGlyphOffset_Variable(NKGlyph left, NKGlyph _, int spacing) {
        return left.Width + spacing + 1;
    }

    #endregion

    // ----- Mono ----- //
    
    #region Mono

    private RenderedLineInfo[] ComputeLines_Mono(Token[] tokens, int maxWidth, WordWrapType wordWrap) {
        return ComputeLines(tokens, maxWidth, GetGlyphOffset_Mono, wordWrap);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetGlyphOffset_Mono(NKGlyph _0, NKGlyph _1, int spacing) {
        return _fontInfo.FontPropoInfo.AsMono.GlyphWidth + spacing + 1;
    }

    #endregion

    public readonly record struct RenderedLineInfo {
        public int     Width   { get; }
        public Token[] Tokens  { get; }
        public int[]?  Offsets { get; }

        public RenderedLineInfo(int width, Token[] tokens, int[] offsets) {
            Width   = width;
            Tokens  = tokens;
            Offsets = offsets;
        }

        public RenderedLineInfo(int width, Token[] tokens) {
            Width   = width;
            Tokens  = tokens;
            Offsets = null;
        }
    }
    
    private NKGlyph GetGlyphFromToken(Token token) {
        return token.Type switch {
            INVALID            => _unknownSymbolGlyph,
            LETTER or LIGATURE => _glyphs[token.Symbol!.Value],
            _                  => throw new ArgumentOutOfRangeException()
        };
    }

    #endregion
    
    // ============================ helper methods ============================ //

    #region Helper Methods

    /// <summary>
    /// Resolves a text string to a glyph symbol, prioritizing an exact style match
    /// and falling back to the closest matching style if an exact match is not found.
    /// </summary>
    /// <param name="text">The text value of the symbol to resolve.</param>
    /// <param name="style">The desired style for the symbol.</param>
    /// <param name="isLigature">Specifies whether the symbol is a ligature.</param>
    /// <returns>The resolved <see cref="NKGlyphSymbol"/>, or <c>null</c> if no matching symbol is found.</returns>
    private NKGlyphSymbol? ResolveSymbol(string text, NKStyle style, bool isLigature) {
        
        // 1. Exact match
        var exactSymbol = isLigature 
            ? NKGlyphSymbol.Ligature(text, style) 
            : NKGlyphSymbol.Simple(text[0], style);
        
        if (_glyphs.ContainsKey(exactSymbol))
            return exactSymbol;
        
        // 2. Closest match
        NKGlyphSymbol? bestSymbol = null;
        int bestScore = -1;
        
        foreach (var symbol in _glyphs.Keys) {
            if (isLigature) {
                if (!symbol.IsLigature || symbol.LigatureSymbol != text) 
                    continue;
            }
            else {
                if (!symbol.IsSimple || symbol.SimpleSymbol != text[0]) 
                    continue;
            }
            
            int score = CalculateStyleMatchScore(style, symbol.Styles);

            if (score <= bestScore)
                continue;

            bestScore  = score;
            bestSymbol = symbol;
        }
        
        return bestSymbol;
    }

    /// <summary>
    /// Calculates a similarity score between two styles to determine the best match.
    /// </summary>
    private static int CalculateStyleMatchScore(NKStyle target, NKStyle candidate) {
        var xor = ~(target.Styles ^ candidate.Styles);
        return Numeric.PopCount((byte)xor);
    }
    
    #endregion
    
    // ============================ tokenization impl ============================ //

    #region Tokenization Implementation

    public Token[] Tokenize(AnsiString text) {
        List<Token> tokens = [];
        int i = 0;
        
        while (i < text.Length) {
            var c = text[i];

            // 1. Newlines
            if (c == '\n') {
                tokens.Add(Token.Newline);
                i++;
                continue;
            }
            if (c == '\r' && i + 1 < text.Length && text[i+1] == '\n') {
                tokens.Add(Token.Newline);
                i += 2;
                continue;
            }

            // 2. Spaces
            if (c == ' ') {
                var style = c.Style;
                int count = 0;
                
                while (i < text.Length && text[i] == ' ' && text[i].Style == style) {
                    count++;
                    i++;
                }
                
                tokens.Add(Token.Space(count, style));
                
                continue;
            }

            // 3. Ligatures
            bool foundLigature = false;
            if (_maxLigatureLength >= 2) {
                for (int len = Math.Min(_maxLigatureLength, text.Length - i); len >= 2; len--) {
                    var style = text[i].Style;
                    
                    // Ligatures must have a uniform style
                    bool styleMatch = true;
                    for (int j = 1; j < len; j++) {
                        if (text[i + j].Style == style)
                            continue;

                        styleMatch = false;
                        break;
                    }
                    
                    if (!styleMatch)
                        continue;

                    string ligatureStr = text.Plain.Substring(i, len);
                    
                    // Check if the font even supports this ligature string
                    if (!_ligatureStrings.Contains(ligatureStr)) 
                        continue;
                    
                    var symbol = ResolveSymbol(ligatureStr, style, true);

                    if (!symbol.HasValue)
                        continue;

                    tokens.Add(Token.Ligature(symbol.Value, style));
                    i += len;
                    foundLigature = true;
                    
                    break;
                }
            }
            if (foundLigature)
                continue;

            // 4. Letters
            var charSymbol = ResolveSymbol(c.Char.ToString(), c.Style, false);
            
            tokens.Add(charSymbol.HasValue 
                ? Token.Letter(charSymbol.Value, c.Style) 
                : Token.Invalid
            );
            
            i++;
        }

        return tokens.ToArray();
    }
    
    public readonly record struct Token {
        public TokenType      Type      { get; }
        public NKGlyphSymbol? Symbol    { get; }
        public NKStyle        FullStyle { get; }
        public int            Length    { get; }

        private Token(TokenType type, NKGlyphSymbol? symbol, NKStyle fullStyle, int length) {
            Type      = type;
            Symbol    = symbol;
            FullStyle = fullStyle;
            Length    = length;
        }
        
        public static Token Invalid { get; } = new(INVALID, null, default, 0);
        public static Token Newline { get; } = new(NEWLINE, null, default, 0);
        
        public static Token Letter(NKGlyphSymbol symbol, NKStyle style) => 
            new(LETTER, symbol, style, 1);
        
        public static Token Ligature(NKGlyphSymbol symbol, NKStyle style) =>
            new(LIGATURE, symbol, style, symbol.LigatureLength);

        public static Token Space(int length, NKStyle style) => 
            new(SPACE, null, style, length);
    }

    public enum TokenType {
        INVALID,
        LETTER,
        LIGATURE,
        SPACE,
        NEWLINE
    }

    #endregion
}