// NeoKolors
// Copyright (c) KryKom 2026

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Metriks;
using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.Extensions;
using NeoKolors.Tui.Core;
using static NeoKolors.Tui.Fonts.NKFont.TokenType;
using static NeoKolors.Tui.Fonts.FontProportionsInfo.ProportionType;
using ProportionType = NeoKolors.Tui.Fonts.FontProportionsInfo.ProportionType;
using Numeric = NeoKolors.Extensions.Numeric;

namespace NeoKolors.Tui.Fonts;

public class NKFont : IAsciiFont {

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
        get => _fontInfo;
        init => _fontInfo = value;
    }

    /// <summary>
    /// Gets the name of the font as defined in the associated font information.
    /// Provides a textual identifier for the font instance.
    /// </summary>
    public string Name => _fontInfo.Name;

    /// <summary>
    /// Provides a collection of glyph mappings, where each glyph is associated with a specific symbol.
    /// This property represents the immutable dictionary of symbols to their corresponding glyphs,
    /// offering a way to retrieve the graphical representation of characters in the font.
    /// </summary>
    public ImmutableDictionary<NKGlyphSymbol, NKGlyph> Glyphs => _glyphs.ToImmutableDictionary();

    /// <summary>
    /// Gets the glyph used as a fallback when an unrecognized or unsupported
    /// symbol is encountered. This glyph substitutes missing or unknown
    /// characters, ensuring continuity in rendering text.
    /// </summary>
    public NKGlyph UnknownSymbolGlyph => _unknownSymbolGlyph;

    /// <summary>
    /// Represents the glyph used for rendering strikethrough effects in text.
    /// This glyph is used to draw a horizontal line through characters,
    /// typically indicating deleted or irrelevant information. If no glyph is
    /// assigned, the strikethrough effect may not be rendered.
    /// </summary>
    public NKGlyph? StrikethroughGlyph => _strikethrough?.Glyph;

    /// <summary>
    /// Indicates whether the strikethrough effect is rendered above the letters.
    /// If no specific value is set, this property defaults to true, meaning the
    /// strikethrough will be placed above the letters by default.
    /// </summary>
    public bool RenderStrikethroughAboveLetters => _strikethrough?.RenderAboveLetters ?? true;

    /// <summary>
    /// Represents the glyph used to render an underline effect within the font system.
    /// This glyph is typically used to draw a visual line beneath text characters and
    /// may vary based on font settings or design specifications.
    /// </summary>
    public NKGlyph? UnderlineGlyph => _underline?.Glyph;

    /// <summary>
    /// Determines whether the underline is rendered above letters
    /// in the text layout when using this font.
    /// When set to true, the underline will be positioned above the glyphs,
    /// otherwise, it will appear below them.
    /// </summary>
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
        bool                               renderUnderlineAboveLetters     = true
    ) {
        _fontInfo           = info;
        _glyphs             = new Dictionary<NKGlyphSymbol, NKGlyph>(glyphs, NKGlyphSymbol.Comparer);
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

        var standardGlyphs = glyphs.Where(
            g => g.Key is {
                IsSimple     : true,
                Styles.Styles: NKTextStyles.NONE,
                SimpleSymbol : >= 'A' and <= 'Z' or >= 'a' and <= 'z'
            }
        );

        var keyValuePairs = standardGlyphs
                as KeyValuePair<NKGlyphSymbol, NKGlyph>[]
            ?? standardGlyphs.ToArray();

        _fontBaseline = keyValuePairs.Any()
            ? keyValuePairs.Max(g => g.Value.BaselineOffset + g.Value.Height)
            : glyphs.Count > 0
                ? glyphs.Values.Max(g => g.BaselineOffset + g.Height)
                : 0;
    }

    #endregion

    // ============================ rendering impl ============================ //

    #region Rendering Implementation

    public void DrawString(string str, ICharDrawer drawer, Point2D position = default, int z = 0) {
        DrawString(str, drawer, int.MaxValue, position, z);
    }

    public void DrawString(
        string      str,
        ICharDrawer drawer,
        int         maxWidth,
        Point2D     position = default,
        int         z        = 0
    ) {
        DrawString(
            str,
            drawer,
            new Rect2D(position, new Point2D(maxWidth, int.MaxValue)),
            new TextRenderingOptions(),
            z
        );
    }

    public void DrawString(
        string          str,
        ICharDrawer     drawer,
        Rect2D          bounds,
        NKStyle         style,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign   verticalAlign   = VerticalAlign.TOP,
        bool            overflow        = false,
        int             z               = 0
    ) {
        DrawString(
            new AnsiString(str, style),
            drawer,
            bounds,
            horizontalAlign,
            verticalAlign,
            overflow,
            z
        );
    }

    public void DrawString(
        AnsiString  str,
        ICharDrawer drawer,
        Point2D     position = default,
        int         z        = 0
    ) {
        DrawString(str, drawer, int.MaxValue, position, z);
    }

    public void DrawString(
        AnsiString  str,
        ICharDrawer drawer,
        int         maxWidth,
        Point2D     position = default,
        int         z        = 0
    ) {
        DrawString(
            str,
            drawer,
            new Rect2D(position, new Point2D(maxWidth, int.MaxValue)),
            new TextRenderingOptions(),
            z
        );
    }

    public void DrawString(
        string               str,
        ICharDrawer          drawer,
        Rect2D               bounds,
        TextRenderingOptions options,
        int                  z = 0
    ) {
        DrawString(new AnsiString(str), drawer, bounds, options, z);
    }

    public void DrawString(
        string               str,
        ICharDrawer          drawer,
        TextRenderingOptions options,
        int                  z = 0
    ) {
        DrawString(str, drawer, new Rect2D(Point2D.Zero, drawer.Size), options, z);
    }

    public void DrawString(
        AnsiString           str,
        ICharDrawer          drawer,
        TextRenderingOptions options,
        int                  z = 0
    ) {
        DrawString(str, drawer, new Rect2D(Point2D.Zero, drawer.Size), options, z);
    }

    public void DrawString(
        AnsiString      str,
        ICharDrawer     drawer,
        Rect2D          bounds,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign   verticalAlign   = VerticalAlign.TOP,
        bool            overflow        = false,
        int             z               = 0
    ) {
        var options = new TextRenderingOptions(horizontalAlign, verticalAlign, overflow);
        DrawString(str, drawer, bounds, options, z);
    }


    public void DrawString(
        AnsiString           str,
        ICharDrawer          drawer,
        Rect2D               bounds,
        TextRenderingOptions options,
        int                  z = 0
    ) {
        bool allowLigatures = options.Spacing?.Ligatures ?? _fontInfo.Ligatures;
        var  tokens         = Tokenize(str, allowLigatures);

        if (tokens.Length == 0)
            return;

        var wordWrap  = options.WordWrap;
        var wrapWidth = bounds.Size.X;

        int cs = options.Spacing?.LetterSpacing ?? _fontInfo.LetterSpacing;
        int ws = options.Spacing?.WordSpacing   ?? _fontInfo.WordSpacing;

        int effectiveLeading = options.Spacing is { Leading: > 0 }
            ? options.Spacing.Leading
            : _fontInfo.Leading;

        var lines = ComputeLines(tokens, wrapWidth, wordWrap, cs, ws, options.Proportions);

        if (lines.Length == 0)
            return;

        if (options.JustifyText && wrapWidth > 0) {
            for (int i = 0; i < lines.Length; i++) {
                if (lines[i].WasWrapped || (lines.Length == 1 && options.JustifyText)) {
                    lines[i] = JustifyLine(lines[i], wrapWidth);
                }
            }
        }

        MeasureVisualSpan(lines, out _, out _, out var minY, out var maxY, out var hasPixels, effectiveLeading);

        var logicalHeight = lines.Length * effectiveLeading;

        var actualHeight = hasPixels
            ? Math.Max(logicalHeight, maxY) - Math.Min(0, minY)
            : logicalHeight;

        var yShift = hasPixels ? Math.Min(0, minY) : 0;

        var yOffset = options.VerticalAlign switch {
            VerticalAlign.TOP    => -yShift,
            VerticalAlign.CENTER => Math.Max(0, (bounds.Size.Y - actualHeight) / 2) - yShift,
            VerticalAlign.BOTTOM => Math.Max(0, bounds.Size.Y - actualHeight)       - yShift,
            _                    => throw new ArgumentOutOfRangeException()
        } + bounds.LowerY;

        Func<int, int, int> computeXOffset = options.HorizontalAlign switch {
            HorizontalAlign.LEFT   => static (_,     _) => 0,
            HorizontalAlign.CENTER => static (width, bounds) => Math.Max(0, (bounds - width) / 2),
            HorizontalAlign.RIGHT  => static (width, bounds) => Math.Max(0, bounds - width),
            _ => throw new ArgumentOutOfRangeException(
                nameof(options.HorizontalAlign),
                options.HorizontalAlign,
                null
            )
        };

        for (var i = 0; i < lines.Length; i++) {
            var line         = lines[i];
            var xOffset      = computeXOffset(line.Width, bounds.Size.X)      + bounds.LowerX;
            var lineBaseline = yOffset + i * effectiveLeading + _fontBaseline - 1;

            var yStart = yOffset                   + i * effectiveLeading;
            var yEnd   = yStart + effectiveLeading - 1;

            var x = xOffset;

            var lastX          = -1;
            var lastWidth      = 0;
            var lastTokenIndex = -1;

            for (var j = 0; j < line.Tokens.Length; j++) {
                var token = line.Tokens[j];

                if (token.Type == SPACE)
                    continue;

                if (line.Offsets != null && j < line.Offsets.Length)
                    x += line.Offsets[j];

                var glyph      = GetGlyphFromToken(token);
                var tokenWidth = glyph.Width;

                if (lastX != -1) {
                    var gapWidth = x - (lastX + lastWidth);

                    if (gapWidth > 0) {
                        var gapToken = line.Tokens[lastTokenIndex + 1];
                        ProcessColumns(lastX + lastWidth, gapWidth, gapToken, null);
                    }
                }

                ProcessColumns(x, tokenWidth, token, glyph);

                lastX          = x;
                lastWidth      = tokenWidth;
                lastTokenIndex = j;
            }

            continue;

            void ProcessColumns(int startCol, int colWidth, Token t, NKGlyph? g) {
                var activeStyle = t.FullStyle;
                var glyphStyles = t.Symbol?.Styles.Styles ?? NKTextStyles.NONE;

                var shouldRender = !activeStyle.Styles.GetIsInvisible();

                if (activeStyle.Styles.GetIsBlink())
                    if (DateTime.Now.Second % 2 != 0)
                        shouldRender = false;

                var hasUnderline     = _underline     != null && activeStyle.Styles.GetIsUnderline()     && !glyphStyles.GetIsUnderline();
                var hasStrikethrough = _strikethrough != null && activeStyle.Styles.GetIsStrikethrough() && !glyphStyles.GetIsStrikethrough();

                var ypu = _underline     != null ? lineBaseline - _underline.Glyph.BaselineOffset     - _underline.Glyph.Height     + 1 : 0;
                var yps = _strikethrough != null ? lineBaseline - _strikethrough.Glyph.BaselineOffset - _strikethrough.Glyph.Height + 1 : 0;
                var yp  = g              != null ? lineBaseline - g.BaselineOffset                    - g.Height                    + 1 : 0;

                for (var dx = 0; dx < colWidth; dx++) {
                    var cx = startCol + dx;

                    for (var cy = yStart; cy <= yEnd; cy++) {
                        if (!options.Overflow)
                            if (cx < bounds.LowerX  ||
                                cx > bounds.HigherX ||
                                cy < bounds.LowerY  ||
                                cy > bounds.HigherY)
                                continue;

                        if (cx < 0 || cx >= drawer.Width || cy < 0 || cy >= drawer.Height)
                            continue;

                        char? cellChar  = null;
                        var   cellStyle = activeStyle;

                        if (hasUnderline && _underline != null && cy >= ypu && cy < ypu + _underline.Glyph.Height) {
                            var col  = cx % _underline.Glyph.Width;
                            var row  = cy - ypu;
                            var cell = _underline.Glyph.Glyph[col, row];

                            if (cell.Type != GlyphCellType.BACKGROUND)
                                cellChar = cell.Type == GlyphCellType.CHARACTER ? cell.Character : ' ';
                        }

                        if (hasStrikethrough && _strikethrough != null && cy >= yps && cy < yps + _strikethrough.Glyph.Height) {
                            var col  = cx % _strikethrough.Glyph.Width;
                            var row  = cy - yps;
                            var cell = _strikethrough.Glyph.Glyph[col, row];

                            if (cell.Type != GlyphCellType.BACKGROUND)
                                cellChar = cell.Type == GlyphCellType.CHARACTER ? cell.Character : ' ';
                        }

                        if (shouldRender && g != null && cy >= yp && cy < yp + g.Height) {
                            var col  = cx - startCol;
                            var row  = cy - yp;
                            var cell = g.Glyph[col, row];

                            if (cell.Type != GlyphCellType.BACKGROUND) {
                                cellChar = cell.Type == GlyphCellType.CHARACTER ? cell.Character : ' ';

                                if (glyphStyles.GetIsNegative())
                                    cellStyle = cellStyle with { Styles = cellStyle.Styles & ~NKTextStyles.NEGATIVE };
                            }
                        }

                        if (_underline != null && cellStyle.Styles.GetIsUnderline())
                            cellStyle = cellStyle with {
                                Styles = cellStyle.Styles & ~NKTextStyles.UNDERLINE
                            };

                        if (_strikethrough != null && cellStyle.Styles.GetIsStrikethrough())
                            cellStyle = cellStyle with {
                                Styles = cellStyle.Styles & ~NKTextStyles.STRIKETHROUGH
                            };

                        char charToDraw;

                        if (cellChar != null) {
                            charToDraw = cellChar.Value;
                        }
                        else if (drawer is ICharCanvas canvas && canvas.Data[cx, cy].Char is var existing && existing != ' ' && existing != '\0') {
                            charToDraw = existing;
                        }
                        else {
                            charToDraw = ' ';
                        }

                        if (options.Coloring != null) {
                            var relX = Math.Clamp(cx - bounds.LowerX, 0, Math.Max(0, bounds.Size.X - 1));
                            var relY = Math.Clamp(cy - bounds.LowerY, 0, Math.Max(0, bounds.Size.Y - 1));
                            cellStyle = cellStyle.WithFColor(options.Coloring.GetColor(relX, relY, bounds.Size.X, bounds.Size.Y));
                        }

                        drawer.Draw(new AnsiChar(charToDraw, cellStyle), cx, cy, z);
                    }
                }
            }
        }
    }

    private static void RenderEffect(
        ICharDrawer drawer,
        NKGlyph     effectGlyph,
        int         startX,
        int         width,
        int         lineBaseline,
        NKStyle     style,
        Rect2D      bounds,
        bool        overflow,
        int         z
    ) {
        var yp = lineBaseline - effectGlyph.BaselineOffset - effectGlyph.Height + 1;

        var finalStyle = style;

        if (finalStyle.Styles.GetIsUnderline())
            finalStyle = finalStyle with {
                Styles = finalStyle.Styles & ~NKTextStyles.UNDERLINE
            };

        if (finalStyle.Styles.GetIsStrikethrough())
            finalStyle = finalStyle with {
                Styles = finalStyle.Styles & ~NKTextStyles.STRIKETHROUGH
            };

        for (var dx = 0; dx < width; dx++) {
            var cx  = startX + dx;
            var col = dx % effectGlyph.Width;

            for (var gy = 0; gy < effectGlyph.Height; gy++) {
                var cy = yp + gy;

                if (!overflow)
                    if (cx < bounds.LowerX || cx > bounds.HigherX || cy < bounds.LowerY || cy > bounds.HigherY)
                        continue;

                if (cx < 0 || cx >= drawer.Width || cy < 0 || cy >= drawer.Height)
                    continue;

                var cell = effectGlyph.Glyph[col, gy];

                if (cell.Type == GlyphCellType.BACKGROUND)
                    continue;

                var cellChar = cell.Type == GlyphCellType.CHARACTER ? cell.Character : ' ';

                drawer.Draw(new AnsiChar(cellChar, finalStyle), cx, cy, z);
            }
        }
    }

    #endregion

    // ============================ measuring impl ============================ //

    #region Measuring Implementation

    public Size2D GetMinSize(string str) {
        return GetMinSize(new AnsiString(str));
    }

    public Size2D GetSize(string str) {
        return GetSize(str, int.MaxValue);
    }

    public Size2D GetSize(string str, int maxWidth) {
        return GetSize(new AnsiString(str), maxWidth);
    }

    public Size2D GetMinSize(AnsiString str) {
        var tokens = Tokenize(str);
        var words  = tokens.Split(t => t.Type is NEWLINE or SPACE);

        var maxWordWidth = 0;

        for (var i = 0; i < words.Length; i++)
            maxWordWidth = Math.Max(maxWordWidth, ComputeLineWidth(words[i]));

        return GetSize(tokens, maxWordWidth);
    }

    public Size2D GetSize(AnsiString str) {
        return GetSize(str, int.MaxValue);
    }

    public Size2D GetSize(AnsiString str, int maxWidth) {
        return GetSize(Tokenize(str), maxWidth);
    }

    public Size2D GetSize(AnsiString str, TextRenderingOptions options, int? maxWidth = null) {
        bool allowLigatures = options.Spacing?.Ligatures ?? _fontInfo.Ligatures;
        var  tokens         = Tokenize(str, allowLigatures);

        if (tokens.Length == 0)
            return Size2D.Zero;

        int cs = options.Spacing?.LetterSpacing ?? _fontInfo.LetterSpacing;
        int ws = options.Spacing?.WordSpacing   ?? _fontInfo.WordSpacing;

        int effectiveLeading = options.Spacing is { Leading: > 0 }
            ? options.Spacing.Leading
            : _fontInfo.Leading;

        int effectiveMaxWidth = maxWidth ?? int.MaxValue;
        var lines             = ComputeLines(tokens, effectiveMaxWidth, options.WordWrap, cs, ws, options.Proportions);

        if (lines.Length == 0)
            return Size2D.Zero;

        if (options.JustifyText && effectiveMaxWidth < int.MaxValue) {
            for (int i = 0; i < lines.Length; i++) {
                if (lines[i].WasWrapped || (lines.Length == 1 && options.JustifyText)) {
                    lines[i] = JustifyLine(lines[i], effectiveMaxWidth);
                }
            }
        }

        var maxLineWidth = 0;

        for (var i = 0; i < lines.Length; i++)
            maxLineWidth = Math.Max(maxLineWidth, lines[i].Width);

        MeasureVisualSpan(
            lines,
            out var minX,
            out var maxX,
            out var minY,
            out var maxY,
            out var hasPixels,
            effectiveLeading
        );

        if (!hasPixels)
            return new Size2D(maxLineWidth, effectiveLeading * lines.Length);

        var finalWidth  = Math.Max(maxLineWidth,                    maxX) - Math.Min(0, minX);
        var finalHeight = Math.Max(effectiveLeading * lines.Length, maxY) - Math.Min(0, minY);

        return new Size2D(finalWidth, finalHeight);
    }

    private Size2D GetSize(Token[] tokens, int maxWidth) {
        var lines = ComputeLines(tokens, maxWidth);

        if (lines.Length == 0)
            return Size2D.Zero;

        var maxLineWidth = 0;

        for (var i = 0; i < lines.Length; i++)
            maxLineWidth = Math.Max(maxLineWidth, lines[i].Width);

        MeasureVisualSpan(
            lines,
            out var minX,
            out var maxX,
            out var minY,
            out var maxY,
            out var hasPixels
        );

        if (!hasPixels)
            return new Size2D(maxLineWidth, _fontInfo.Leading * lines.Length);

        var finalWidth  = Math.Max(maxLineWidth,                     maxX) - Math.Min(0, minX);
        var finalHeight = Math.Max(_fontInfo.Leading * lines.Length, maxY) - Math.Min(0, minY);

        return new Size2D(finalWidth, finalHeight);
    }

    private void MeasureVisualSpan(
        RenderedLineInfo[] lines,
        out int            minX,
        out int            maxX,
        out int            minY,
        out int            maxY,
        out bool           hasPixels,
        int?               leading = null
    ) {
        int effLeading     = leading ?? _fontInfo.Leading;
        var localMinX      = int.MaxValue;
        var localMaxX      = int.MinValue;
        var localMinY      = int.MaxValue;
        var localMaxY      = int.MinValue;
        var localHasPixels = false;

        for (var i = 0; i < lines.Length; i++) {
            var line         = lines[i];
            var lineBaseline = i * effLeading + _fontBaseline - 1;
            var x            = 0;

            for (var j = 0; j < line.Tokens.Length; j++) {
                x = RenderGlyphsWithStyles(line, j, x, lineBaseline);
            }
        }

        minX      = localMinX;
        maxX      = localMaxX;
        minY      = localMinY;
        maxY      = localMaxY;
        hasPixels = localHasPixels;

        return;

        void ProcessGlyphCells(NKGlyph g, int startX, int startY, bool isEffect = false, int effectWidth = 0) {
            if (isEffect) {
                for (var dx = 0; dx < effectWidth; dx++) {
                    var cx  = startX + dx;
                    var col = dx % g.Width;

                    for (var gy = 0; gy < g.Height; gy++) {
                        var cy   = startY + gy;
                        var cell = g.Glyph[col, gy];

                        if (cell.Type == GlyphCellType.BACKGROUND)
                            continue;

                        localMinX      = Math.Min(localMinX, cx);
                        localMaxX      = Math.Max(localMaxX, cx + 1);
                        localMinY      = Math.Min(localMinY, cy);
                        localMaxY      = Math.Max(localMaxY, cy + 1);
                        localHasPixels = true;
                    }
                }
            }
            else {
                for (var gx = 0; gx < g.Width; gx++) {
                    var cx = startX + gx;

                    for (var gy = 0; gy < g.Height; gy++) {
                        var cy   = startY + gy;
                        var cell = g.Glyph[gx, gy];

                        if (cell.Type == GlyphCellType.BACKGROUND)
                            continue;

                        localMinX      = Math.Min(localMinX, cx);
                        localMaxX      = Math.Max(localMaxX, cx + 1);
                        localMinY      = Math.Min(localMinY, cy);
                        localMaxY      = Math.Max(localMaxY, cy + 1);
                        localHasPixels = true;
                    }
                }
            }
        }

        int RenderGlyphsWithStyles(RenderedLineInfo line, int j, int x, int lineBaseline) {
            var token = line.Tokens[j];

            if (token.Type == SPACE)
                return x;

            var glyph = GetGlyphFromToken(token);

            if (line.Offsets != null && j < line.Offsets.Length)
                x += line.Offsets[j];

            var shouldRender = !token.FullStyle.Styles.GetIsInvisible();
            var glyphStyles  = token.Symbol?.Styles.Styles ?? NKTextStyles.NONE;

            // Process underline before
            if (_underline?.RenderAboveLetters == false &&
                token.FullStyle.Styles.GetIsUnderline() &&
                !glyphStyles.GetIsUnderline()
            ) {
                var ypu = lineBaseline - _underline.Glyph.BaselineOffset - _underline.Glyph.Height + 1;
                ProcessGlyphCells(_underline.Glyph, x, ypu, true, glyph.Width);
            }

            // Process strikethrough before
            if (_strikethrough?.RenderAboveLetters == false &&
                token.FullStyle.Styles.GetIsStrikethrough() &&
                !glyphStyles.GetIsStrikethrough()
            ) {
                var yps = lineBaseline - _strikethrough.Glyph.BaselineOffset - _strikethrough.Glyph.Height + 1;
                ProcessGlyphCells(_strikethrough.Glyph, x, yps, true, glyph.Width);
            }

            // Process the letter itself
            if (shouldRender) {
                var yp = lineBaseline - glyph.BaselineOffset - glyph.Height + 1;
                ProcessGlyphCells(glyph, x, yp);
            }

            // Process underline after
            if (_underline?.RenderAboveLetters == true  &&
                token.FullStyle.Styles.GetIsUnderline() &&
                !glyphStyles.GetIsUnderline()
            ) {
                var ypu = lineBaseline - _underline.Glyph.BaselineOffset - _underline.Glyph.Height + 1;
                ProcessGlyphCells(_underline.Glyph, x, ypu, true, glyph.Width);
            }

            // Process strikethrough after
            if (_strikethrough?.RenderAboveLetters == true  &&
                token.FullStyle.Styles.GetIsStrikethrough() &&
                !glyphStyles.GetIsStrikethrough()
            ) {
                var yps = lineBaseline - _strikethrough.Glyph.BaselineOffset - _strikethrough.Glyph.Height + 1;
                ProcessGlyphCells(_strikethrough.Glyph, x, yps, true, glyph.Width);
            }

            return x;
        }
    }

    private int ComputeLineWidth(Token[] tokens) {
        return ComputeLineWidth(
            tokens,
            _fontInfo.ProportionType switch {
                MONOSPACED         => GetGlyphOffset_Mono,
                VARIABLE           => GetGlyphOffset_Variable,
                VARIABLE | KERNING => GetGlyphOffset_Kerning,
                _                  => throw new ArgumentOutOfRangeException(nameof(_fontInfo.ProportionType))
            }
        );
    }

    private int ComputeLineWidth(Token[] tokens, Func<NKGlyph, NKGlyph, int, int> getOffset) {
        var      co           = 0;
        var      cw           = 0;
        NKGlyph? lg           = null;
        var      cs           = _fontInfo.LetterSpacing;
        var      sp           = cs;
        var      leadingSpace = 0;

        for (var i = 0; i < tokens.Length; i++) {
            var t = tokens[i];

            switch (t.Type) {
                case LIGATURE or LETTER or INVALID: {
                    var g = GetGlyphFromToken(t);

                    if (lg is null) {
                        co = leadingSpace;
                        cw = co + g.Width;
                        lg = g;
                        sp = cs;

                        break;
                    }

                    co += getOffset(lg, g, sp);
                    cw =  co + g.Width;
                    lg =  g;
                    sp =  cs;
                }

                    break;
                case SPACE: {
                    if (lg is null) {
                        leadingSpace += t.Length * _fontInfo.WordSpacing;
                    }
                    else {
                        sp = _fontInfo.ProportionType == MONOSPACED
                            ? cs + t.Length * _fontInfo.WordSpacing
                            : t.Length * _fontInfo.WordSpacing;
                    }
                }

                    break;
                case NEWLINE: {
                    // ignore?  

                    break;
                }
                default: throw new ArgumentOutOfRangeException();
            }
        }

        return cw;
    }

    private RenderedLineInfo[] ComputeLines(
        Token[]              tokens,
        int                  maxWidth,
        WordWrapType         wordWrap      = WordWrapType.WORD,
        int?                 letterSpacing = null,
        int?                 wordSpacing   = null,
        FontProportionsInfo? proportions   = null
    ) {
        int cs        = letterSpacing ?? _fontInfo.LetterSpacing;
        int ws        = wordSpacing   ?? _fontInfo.WordSpacing;
        var propoInfo = proportions   ?? _fontInfo.FontPropoInfo;
        var propType  = propoInfo.Type;

        if (propType.GetUsesKerning())
            return ComputeLines_Kerning(tokens, maxWidth, wordWrap, cs, ws);

        if (propType.GetIsVariable())
            return ComputeLines_Variable(tokens, maxWidth, wordWrap, cs, ws);

        return ComputeLines_Mono(tokens, maxWidth, wordWrap, cs, ws, propoInfo);
    }

    private RenderedLineInfo[] ComputeLines(
        Token[]                          tokens,
        int                              maxWidth,
        Func<NKGlyph, NKGlyph, int, int> getOffset,
        WordWrapType                     wordWrap
    ) {
        return ComputeLines(
            tokens,
            maxWidth,
            getOffset,
            wordWrap,
            _fontInfo.LetterSpacing,
            _fontInfo.WordSpacing,
            _fontInfo.ProportionType
        );
    }

    private RenderedLineInfo[] ComputeLines(
        Token[]                          tokens,
        int                              maxWidth,
        Func<NKGlyph, NKGlyph, int, int> getOffset,
        WordWrapType                     wordWrap,
        int                              cs,
        int                              ws,
        ProportionType                   propType
    ) {
        // return empty if there are no tokens
        if (tokens.Length == 0)
            return [];

        var offsets = new int[tokens.Length];
        var widths  = new int[tokens.Length];

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
            INVALID            => _unknownSymbolGlyph,
            _                  => null
        };

        var spacing = first.Type == SPACE ? first.Length * ws : 0;

        // set values for the other tokens
        for (var i = 1; i < tokens.Length; i++) {
            var ct = tokens[i];

            switch (ct.Type) {
                case INVALID:
                case LETTER:
                case LIGATURE: {
                    var cg = GetGlyphFromToken(ct);

                    var effectiveSpacing = spacing == 0
                        ? cs
                        : propType == MONOSPACED
                            ? cs + spacing
                            : spacing;

                    offsets[i] = lg == null ? spacing : getOffset(lg, cg, effectiveSpacing);
                    widths[i]  = cg.Width;
                    spacing    = 0;
                    lg         = cg;

                    break;
                }
                case SPACE: {
                    spacing += ct.Length * ws;

                    break;
                }
                case NEWLINE: {
                    lg      = null;
                    spacing = 0;

                    break;
                }
                default: {
                    LOGGER.Crit($"Unexpected token flags '{(int)ct.Type}'. What the hell bro?");

                    throw new ArgumentOutOfRangeException(nameof(ct.Type));
                }
            }
        }

        // compute lines
        List<RenderedLineInfo> lines = [];

        var  lineStart      = 0;
        int? lastSpaceIndex = null;
        var  lastSpaceWidth = 0;
        var  lastWidth      = 0;
        var  currentWidth   = 0;
        var  currentOffset  = 0;

        for (var i = 0; i < tokens.Length; i++) {
            var t = tokens[i];

            switch (t.Type) {
                case INVALID or LETTER or LIGATURE: {
                    currentOffset += offsets[i];
                    currentWidth  =  currentOffset + widths[i];
                }

                    break;
                case SPACE: {
                    lastSpaceIndex = i;
                    lastSpaceWidth = currentWidth;
                }

                    break;
                case NEWLINE: {
                    lines.Add(
                        new RenderedLineInfo(
                            currentWidth,
                            tokens.InRange(lineStart, i).ToArray(),
                            offsets.InRange(lineStart, i).ToArray(),
                            wasWrapped: false
                        )
                    );

                    lineStart = i + 1;

                    if (lineStart < tokens.Length)
                        offsets[lineStart] = 0;

                    currentOffset = 0;
                    currentWidth  = 0;
                }

                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            if (wordWrap != WordWrapType.NONE && currentWidth > maxWidth) {
                if (wordWrap == WordWrapType.WORD && lastSpaceIndex.HasValue) {
                    var j = lastSpaceIndex.Value;

                    lines.Add(
                        new RenderedLineInfo(
                            lastSpaceWidth,
                            tokens.InRange(lineStart, j).ToArray(),
                            offsets.InRange(lineStart, j).ToArray(),
                            wasWrapped: true
                        )
                    );

                    i         = lastSpaceIndex.Value;
                    lineStart = lastSpaceIndex.Value + 1;
                }
                else if (lineStart == i) {
                    lines.Add(
                        new RenderedLineInfo(
                            currentWidth,
                            [tokens[i]],
                            [offsets[i]],
                            wasWrapped: true
                        )
                    );

                    lineStart++;
                }
                else {
                    lines.Add(
                        new RenderedLineInfo(
                            lastWidth,
                            tokens.InRange(lineStart, i).ToArray(),
                            offsets.InRange(lineStart, i).ToArray(),
                            wasWrapped: true
                        )
                    );

                    lineStart =  i;
                    i         -= 1;
                }

                if (lineStart < tokens.Length)
                    offsets[lineStart] = 0;

                lastSpaceIndex = null;
                lastSpaceWidth = 0;
                currentOffset  = 0;
                currentWidth   = 0;
            }

            lastWidth = currentWidth;
        }

        // add remaining tokens
        if (lineStart < tokens.Length)
            lines.Add(
                new RenderedLineInfo(
                    currentOffset + widths[tokens.Length - 1],
                    tokens.Skip(lineStart).ToArray(),
                    offsets.Skip(lineStart).ToArray(),
                    wasWrapped: false
                )
            );

        return lines.ToArray();
    }

    // ----- Variable - With Kerning ----- //

    #region Kerning

    private RenderedLineInfo[] ComputeLines_Kerning(Token[] tokens, int maxWidth, WordWrapType wordWrap) {
        return ComputeLines_Kerning(tokens, maxWidth, wordWrap, _fontInfo.LetterSpacing, _fontInfo.WordSpacing);
    }

    private RenderedLineInfo[] ComputeLines_Kerning(Token[] tokens, int maxWidth, WordWrapType wordWrap, int cs, int ws) {
        return ComputeLines(tokens, maxWidth, GetGlyphOffset_Kerning, wordWrap, cs, ws, VARIABLE | KERNING);
    }

    private static int GetGlyphOffset_Kerning(NKGlyph left, NKGlyph right, int spacing) {
        var closest  = int.MaxValue;
        var closestY = -1;

        var startY = Math.Max(left.BaselineOffset, right.BaselineOffset);
        var endY   = Math.Min(left.BaselineOffset + left.Height, right.BaselineOffset + right.Height);

        for (var y = startY; y < endY; y++) {
            // y is baseline offset
            var firstGap  = 0;
            var secondGap = 0;

            for (var x = left.Width - 1; x >= 0; x--) {
                if (left.Glyph[x, CalcY(left, y)].Type == GlyphCellType.BACKGROUND)
                    continue;

                firstGap = left.Width - x - 1;

                break;
            }

            for (var x = 0; x < right.Width; x++) {
                if (right.Glyph[x, CalcY(right, y)].Type == GlyphCellType.BACKGROUND)
                    continue;

                secondGap = x;

                break;
            }

            var gap = firstGap + secondGap;

            if (gap >= closest)
                continue;

            closest  = gap;
            closestY = y;
        }

        if (closest == int.MaxValue)
            return left.Width;

        for (var x = left.Width - 1; x >= 0; x--) {
            if (left.Glyph[x, CalcY(left, closestY)].Type == GlyphCellType.BACKGROUND)
                continue;

            return x + 1 - GetSecondOffset() + spacing;
        }

        return left.Width;

        // ----- local methods ----- //

        int GetSecondOffset() {
            for (var x = 0; x < right.Width; x++) {
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
        return ComputeLines_Variable(tokens, maxWidth, wordWrap, _fontInfo.LetterSpacing, _fontInfo.WordSpacing);
    }

    private RenderedLineInfo[] ComputeLines_Variable(Token[] tokens, int maxWidth, WordWrapType wordWrap, int cs, int ws) {
        return ComputeLines(tokens, maxWidth, GetGlyphOffset_Variable, wordWrap, cs, ws, VARIABLE);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int GetGlyphOffset_Variable(NKGlyph left, NKGlyph _, int spacing) {
        return left.Width + spacing + 1;
    }

    #endregion

    // ----- Mono ----- //

    #region Mono

    private RenderedLineInfo[] ComputeLines_Mono(Token[] tokens, int maxWidth, WordWrapType wordWrap) {
        return ComputeLines_Mono(
            tokens,
            maxWidth,
            wordWrap,
            _fontInfo.LetterSpacing,
            _fontInfo.WordSpacing,
            _fontInfo.FontPropoInfo
        );
    }

    private RenderedLineInfo[] ComputeLines_Mono(
        Token[]              tokens,
        int                  maxWidth,
        WordWrapType         wordWrap,
        int                  cs,
        int                  ws,
        FontProportionsInfo? propoInfo = null
    ) {
        var info = propoInfo ?? _fontInfo.FontPropoInfo;

        int glyphWidth = info.Type.GetIsMonospaced() && info.AsMono.GlyphWidth > 0
            ? info.AsMono.GlyphWidth
            : _fontInfo.ProportionType.GetIsMonospaced()
                ? _fontInfo.FontPropoInfo.AsMono.GlyphWidth
                : _glyphs.Count > 0 ? _glyphs.Values.Max(g => g.Width) : 8;

        return ComputeLines(
            tokens,
            maxWidth,
            (_, _, sp) => glyphWidth + sp + 1,
            wordWrap,
            cs,
            ws,
            MONOSPACED
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetGlyphOffset_Mono(NKGlyph _0, NKGlyph _1, int spacing) {
        int glyphWidth = _fontInfo.ProportionType.GetIsMonospaced()
            ? _fontInfo.FontPropoInfo.AsMono.GlyphWidth
            : (_glyphs.Count > 0 ? _glyphs.Values.Max(g => g.Width) : 8);

        return glyphWidth + spacing + 1;
    }

    #endregion

    private static RenderedLineInfo JustifyLine(RenderedLineInfo line, int targetWidth) {
        if (line.Width >= targetWidth || line.Tokens.Length <= 1 || line.Offsets == null)
            return line;

        List<int> gapIndices = [];

        for (int j = 1; j < line.Tokens.Length; j++) {
            if (line.Tokens[j].Type != SPACE && line.Tokens[j - 1].Type == SPACE) {
                gapIndices.Add(j);
            }
        }

        if (gapIndices.Count == 0)
            return line;

        int extraSpace = targetWidth - line.Width;
        int baseExtra  = extraSpace / gapIndices.Count;
        int remainder  = extraSpace % gapIndices.Count;

        var newOffsets = (int[])line.Offsets.Clone();

        for (int k = 0; k < gapIndices.Count; k++) {
            int idx = gapIndices[k];
            int add = baseExtra + (k < remainder ? 1 : 0);
            newOffsets[idx] += add;
        }

        return new RenderedLineInfo(targetWidth, line.Tokens, newOffsets, line.WasWrapped);
    }

    private readonly record struct RenderedLineInfo {
        public int     Width      { get; }
        public Token[] Tokens     { get; }
        public int[]?  Offsets    { get; }
        public bool    WasWrapped { get; }

        public RenderedLineInfo(int width, Token[] tokens, int[]? offsets, bool wasWrapped = false) {
            Width      = width;
            Tokens     = tokens;
            Offsets    = offsets;
            WasWrapped = wasWrapped;
        }

        public RenderedLineInfo(int width, Token[] tokens) : this(width, tokens, null, false) { }
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
        var            bestScore  = -1;

        foreach (var symbol in _glyphs.Keys) {
            if (isLigature) {
                if (!symbol.IsLigature || symbol.LigatureSymbol != text)
                    continue;
            }
            else {
                if (!symbol.IsSimple || symbol.SimpleSymbol != text[0])
                    continue;
            }

            var score = CalculateStyleMatchScore(style, symbol.Styles);

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

    public Token[] Tokenize(AnsiString text) => Tokenize(text, _fontInfo.Ligatures);

    public Token[] Tokenize(AnsiString text, bool allowLigatures) {
        List<Token> tokens = [];
        var         i      = 0;

        while (i < text.Length) {
            var c = text[i];

            // 1. Newlines
            if (c == '\n') {
                tokens.Add(Token.Newline);
                i++;

                continue;
            }

            if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n') {
                tokens.Add(Token.Newline);
                i += 2;

                continue;
            }

            // 2. Spaces
            if (c == ' ') {
                var style = c.Style;
                var count = 0;

                while (i < text.Length && text[i] == ' ' && text[i].Style == style) {
                    count++;
                    i++;
                }

                tokens.Add(Token.Space(count, style));

                continue;
            }

            // 3. Ligatures
            var foundLigature = false;

            if (allowLigatures && _maxLigatureLength >= 2)
                for (var len = Math.Min(_maxLigatureLength, text.Length - i); len >= 2; len--) {
                    var style = text[i].Style;

                    // Ligatures must have a uniform style
                    var styleMatch = true;

                    for (var j = 1; j < len; j++) {
                        if (text[i + j].Style == style)
                            continue;

                        styleMatch = false;

                        break;
                    }

                    if (!styleMatch)
                        continue;

                    var ligatureStr = text.Plain.Substring(i, len);

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

            if (foundLigature)
                continue;

            // 4. Letters
            var charSymbol = ResolveSymbol(c.Char.ToString(), c.Style, false);

            tokens.Add(
                charSymbol.HasValue
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

        public static Token Letter(NKGlyphSymbol symbol, NKStyle style) {
            return new Token(LETTER, symbol, style, 1);
        }

        public static Token Ligature(NKGlyphSymbol symbol, NKStyle style) {
            return new Token(LIGATURE, symbol, style, symbol.LigatureLength);
        }

        public static Token Space(int length, NKStyle style) {
            return new Token(SPACE, null, style, length);
        }
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