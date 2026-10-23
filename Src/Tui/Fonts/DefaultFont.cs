// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Fonts;

/// <summary>
/// Represents a font that does not convert strings to any glyph form but simply places the string.
/// </summary>
public class DefaultFont : IAsciiFont {
    public virtual string Name => "Default";

    public virtual void DrawString(string str, ICharDrawer drawer, Point2D position = default, int z = 0) {
        if (string.IsNullOrEmpty(str))
            return;

        string[] lines = str.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

        for (int y = 0; y < lines.Length; y++) {
            drawer.Draw(new AnsiString(lines[y]), position.X, position.Y + y, z);
        }
    }

    public virtual void DrawString(string str, ICharDrawer drawer, int maxWidth, Point2D position = default, int z = 0) {
        DrawString(new AnsiString(str), drawer, maxWidth, position, z);
    }

    public virtual void DrawString(
        string          str,
        ICharDrawer     drawer,
        Rect2D          bounds,
        NKStyle         style,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign   verticalAlign   = VerticalAlign.TOP,
        bool            overflow        = false,
        int             z               = 0
    ) {
        DrawString(new AnsiString(str, style), drawer, bounds, horizontalAlign, verticalAlign, overflow, z);
    }

    public virtual void DrawString(
        string               str,
        ICharDrawer          drawer,
        Rect2D               bounds,
        TextRenderingOptions options,
        int                  z = 0
    ) {
        DrawString(new AnsiString(str), drawer, bounds, options, z);
    }

    public virtual void DrawString(
        string               str,
        ICharDrawer          drawer,
        TextRenderingOptions options,
        int                  z = 0
    ) {
        DrawString(str, drawer, new Rect2D(Point2D.Zero, drawer.Size), options, z);
    }

    public virtual void DrawString(
        AnsiString           str,
        ICharDrawer          drawer,
        TextRenderingOptions options,
        int                  z = 0
    ) {
        DrawString(str, drawer, new Rect2D(Point2D.Zero, drawer.Size), options, z);
    }

    public virtual void DrawString(
        AnsiString  str,
        ICharDrawer drawer,
        Point2D     position = default,
        int         z        = 0
    ) {
        if (AnsiString.IsNullOrEmpty(str))
            return;

        var lines = str.Split('\n');

        for (int y = 0; y < lines.Length; y++) {
            var line = lines[y];

            if (line.Length > 0 && line.Plain.EndsWith('\r'))
                line = line.Substring(0, line.Length - 1);

            drawer.Draw(line, position.X, position.Y + y, z);
        }
    }

    public virtual void DrawString(AnsiString str, ICharDrawer drawer, int maxWidth, Point2D position = default, int z = 0) {
        if (maxWidth <= 0 || AnsiString.IsNullOrEmpty(str))
            return;

        var wrappedLines = WrapLines(str, maxWidth, WordWrapType.WORD);

        for (int y = 0; y < wrappedLines.Length; y++) {
            drawer.Draw(wrappedLines[y].Line, position.X, position.Y + y, z);
        }
    }

    public virtual void DrawString(
        AnsiString      str,
        ICharDrawer     drawer,
        Rect2D          bounds,
        HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
        VerticalAlign   verticalAlign   = VerticalAlign.TOP,
        bool            overflow        = false,
        int             z               = 0
    ) {
        if (bounds.SizeX <= 0 || bounds.SizeY <= 0 || AnsiString.IsNullOrEmpty(str))
            return;

        var wrappedLines = WrapLines(str, bounds.SizeX, WordWrapType.WORD);

        int y = verticalAlign switch {
            VerticalAlign.TOP    => 0,
            VerticalAlign.CENTER => Math.Max(0, bounds.SizeY - wrappedLines.Length) / 2,
            VerticalAlign.BOTTOM => Math.Max(0, bounds.SizeY - wrappedLines.Length),
            _                    => throw new ArgumentOutOfRangeException(nameof(verticalAlign), verticalAlign, null)
        } + bounds.LowerY;

        Func<int, int, int> computeXOffset = horizontalAlign switch {
            HorizontalAlign.LEFT   => static (_,  _) => 0,
            HorizontalAlign.CENTER => static (tw, lw) => Math.Max(0, tw - lw) / 2,
            HorizontalAlign.RIGHT  => static (tw, lw) => Math.Max(0, tw - lw),
            _                      => throw new ArgumentOutOfRangeException(nameof(horizontalAlign), horizontalAlign, null)
        };

        for (int l = 0; l < wrappedLines.Length; l++) {
            if (!overflow) {
                if (y < bounds.LowerY) {
                    y++;

                    continue;
                }

                if (y >= bounds.HigherY)
                    break;
            }

            var s = wrappedLines[l].Line;
            var x = computeXOffset(bounds.SizeX, s.Length) + bounds.LowerX;

            drawer.Draw(s, x, y, z);

            y++;
        }
    }

    public virtual void DrawString(
        AnsiString           str,
        ICharDrawer          drawer,
        Rect2D               bounds,
        TextRenderingOptions options,
        int                  z = 0
    ) {
        if (bounds.SizeX <= 0 || bounds.SizeY <= 0)
            return;

        int leading       = options.Spacing != null && options.Spacing.Leading > 0 ? options.Spacing.Leading : 1;
        int letterSpacing = options.Spacing != null ? Math.Max(0, options.Spacing.LetterSpacing) : 0;
        int wordSpacing   = options.Spacing != null ? Math.Max(0, options.Spacing.WordSpacing) : 0;

        var wrappedLines = WrapLines(str, bounds.SizeX, options.WordWrap, letterSpacing, wordSpacing);

        if (wrappedLines.Length == 0)
            return;

        int actualHeight = (wrappedLines.Length - 1) * leading + 1;

        int y = options.VerticalAlign switch {
            VerticalAlign.TOP    => 0,
            VerticalAlign.CENTER => Math.Max(0, bounds.SizeY - actualHeight) / 2,
            VerticalAlign.BOTTOM => Math.Max(0, bounds.SizeY - actualHeight),
            _ => throw new ArgumentOutOfRangeException(
                nameof(options.VerticalAlign),
                options.VerticalAlign,
                null
            )
        } + bounds.LowerY;

        Func<int, int, int> computeXOffset = options.HorizontalAlign switch {
            HorizontalAlign.LEFT   => static (_,  _) => 0,
            HorizontalAlign.CENTER => static (tw, lw) => Math.Max(0, tw - lw) / 2,
            HorizontalAlign.RIGHT  => static (tw, lw) => Math.Max(0, tw - lw),
            _ => throw new ArgumentOutOfRangeException(
                nameof(options.HorizontalAlign),
                options.HorizontalAlign,
                null
            )
        };

        for (int l = 0; l < wrappedLines.Length; l++) {
            int lineY = y + l * leading;

            if (!options.Overflow && (lineY < bounds.LowerY || lineY >= bounds.HigherY))
                continue;

            var (s, wasWrapped) = wrappedLines[l];
            bool shouldJustify = options.JustifyText && (wasWrapped || (wrappedLines.Length == 1 && options.JustifyText));

            if (shouldJustify && bounds.SizeX > 0) {
                s = JustifyLine(s, bounds.SizeX, letterSpacing, wordSpacing);
            }

            int lineWidth = ComputeLineWidth(s, letterSpacing, wordSpacing);
            var x         = computeXOffset(bounds.SizeX, lineWidth) + bounds.LowerX;

            int curX = x;

            for (int i = 0; i < s.Length; i++) {
                char c = s[i].Char;

                if (c is '\r' or '\n')
                    continue;

                if (c == ' ') {
                    curX += 1 + wordSpacing;

                    continue;
                }

                if (options.Overflow || (curX >= bounds.LowerX && curX < bounds.HigherX)) {
                    var charStyle = s[i].Style;

                    if (options.Coloring is not null) {
                        var relX = Math.Clamp(curX  - bounds.LowerX, 0, Math.Max(0, bounds.SizeX - 1));
                        var relY = Math.Clamp(lineY - bounds.LowerY, 0, Math.Max(0, bounds.SizeY - 1));
                        charStyle = charStyle.WithFColor(options.Coloring.GetColor(relX, relY, bounds.SizeX, bounds.SizeY));
                    }

                    drawer.Draw(new AnsiChar(c, charStyle), curX, lineY, z);
                }

                curX += 1 + letterSpacing;
            }
        }
    }

    private static (AnsiString Line, bool WasWrapped)[] WrapLines(
        AnsiString   str,
        int          maxWidth,
        WordWrapType wordWrap,
        int          letterSpacing = 0,
        int          wordSpacing   = 0
    ) {
        if (maxWidth <= 0)
            return [];

        var split  = str.Split('\n');
        var result = new List<(AnsiString Line, bool WasWrapped)>();

        foreach (var rawLine in split) {
            var line = rawLine;

            if (line.Length > 0 && line.Plain.EndsWith('\r'))
                line = line.Substring(0, line.Length - 1);

            if (line.Length == 0) {
                result.Add((AnsiString.Empty, false));

                continue;
            }

            switch (wordWrap) {
                case WordWrapType.NONE: {
                    result.Add((line, false));

                    break;
                }

                case WordWrapType.HARD: {
                    WrapLine_Hard(line);

                    break;
                }
                
                case WordWrapType.WORD:
                default: {
                    WrapLine_Word(line);

                    break;
                }
            }
        }

        return result.ToArray();

        void WrapLine_Hard(AnsiString line) {
            int start = 0;

            while (start < line.Length) {
                int count    = 0;
                int currentW = 0;

                while (start + count < line.Length) {
                    char c     = line[start + count].Char;
                    int  charW = c == ' ' ? 1 + wordSpacing : 1 + letterSpacing;

                    if (count > 0 && currentW + charW > maxWidth)
                        break;

                    currentW += charW;
                    count++;
                }

                if (count == 0)
                    count = 1;

                bool wasWrapped = start + count < line.Length;
                result.Add((line.Substring(start, count), wasWrapped));
                start += count;
            }
        }

        void WrapLine_Word(AnsiString line) {
            if (letterSpacing == 0 && wordSpacing == 0) {
                var chopped = line.Chop(maxWidth);

                if (chopped.Length == 0) {
                    result.Add((AnsiString.Empty, false));
                }
                else {
                    for (int i = 0; i < chopped.Length; i++) {
                        bool wasWrapped = i < chopped.Length - 1;
                        result.Add((chopped[i], wasWrapped));
                    }
                }
            }
            else {
                var tokens       = SplitWordsAndSpaces(line);
                var currentLine  = new List<AnsiChar>();
                int currentLineW = 0;

                for (int i = 0; i < tokens.Count; i++) {
                    var token  = tokens[i];
                    int tokenW = ComputeTokenWidth(token, letterSpacing, wordSpacing);

                    if (currentLine.Count > 0 && currentLineW + tokenW > maxWidth && !IsAllSpaces(token)) {
                        TrimTrailingSpaces(currentLine);
                        
                        result.Add((new AnsiString(currentLine), true));
                        
                        currentLine  = [];
                        currentLineW = 0;

                        if (IsAllSpaces(token))
                            continue;
                    }

                    currentLine.AddRange(token);
                    currentLineW += tokenW;
                }

                if (currentLine.Count <= 0)
                    return;

                TrimTrailingSpaces(currentLine);
                result.Add((new AnsiString(currentLine), false));
            }
        }
    }

    private static List<List<AnsiChar>> SplitWordsAndSpaces(AnsiString str) {
        var tokens = new List<List<AnsiChar>>();

        if (str.Length == 0)
            return tokens;

        var  current = new List<AnsiChar> { str[0] };
        bool inSpace = str[0].Char == ' ';

        for (int i = 1; i < str.Length; i++) {
            var  ac      = str[i];
            bool isSpace = ac.Char == ' ';

            if (isSpace == inSpace) {
                current.Add(ac);
            }
            else {
                tokens.Add(current);
                current = [ac];
                inSpace = isSpace;
            }
        }

        tokens.Add(current);

        return tokens;
    }

    private static bool IsAllSpaces(List<AnsiChar> token) => token.Count > 0 && token[0].Char == ' ';

    private static void TrimTrailingSpaces(List<AnsiChar> list) {
        while (list.Count > 0 && list[^1].Char == ' ') {
            list.RemoveAt(list.Count - 1);
        }
    }

    private static int ComputeTokenWidth(List<AnsiChar> token, int letterSpacing, int wordSpacing) {
        if (token.Count == 0)
            return 0;

        if (token[0].Char == ' ')
            return token.Count * (1 + wordSpacing);

        return token.Count + Math.Max(0, token.Count - 1) * letterSpacing;
    }

    private static int ComputeLineWidth(AnsiString str, int letterSpacing, int wordSpacing) {
        if (str.Length == 0)
            return 0;

        int w = 0;

        for (int i = 0; i < str.Length; i++) {
            char c = str[i].Char;

            if (c is '\r' or '\n')
                continue;

            w += c == ' ' ? 1 + wordSpacing : 1 + letterSpacing;
        }

        if (str.Length > 0 && str[^1].Char != ' ' && letterSpacing > 0 && w > 0) {
            w -= letterSpacing;
        }

        return w;
    }

    private static AnsiString JustifyLine(AnsiString line, int targetWidth, int letterSpacing, int wordSpacing) {
        if (targetWidth <= 0 || line.Length == 0)
            return line;

        var     words       = new List<List<AnsiChar>>();
        var     currentWord = new List<AnsiChar>();
        NKStyle spaceStyle  = NKStyle.Default;

        for (int i = 0; i < line.Length; i++) {
            var ac = line[i];

            if (ac.Char == ' ') {
                if (currentWord.Count > 0) {
                    words.Add(currentWord);
                    currentWord = [];
                }

                spaceStyle = ac.Style;
            }
            else {
                currentWord.Add(ac);
            }
        }

        if (currentWord.Count > 0) {
            words.Add(currentWord);
        }

        if (words.Count <= 1)
            return line;

        int wordsWidth = 0;

        foreach (var w in words) {
            int wLen   = w.Count;
            int wWidth = wLen + Math.Max(0, wLen - 1) * letterSpacing;
            wordsWidth += wWidth;
        }

        int availableSpace = targetWidth - wordsWidth;
        int gaps           = words.Count - 1;

        int spaceAdvance    = 1 + wordSpacing;
        int totalSpaceCount = availableSpace / spaceAdvance;

        if (totalSpaceCount < gaps)
            return line;

        int baseSpaces = totalSpaceCount / gaps;
        int remainder  = totalSpaceCount % gaps;

        var result = new List<AnsiChar>(targetWidth);

        for (int i = 0; i < words.Count; i++) {
            result.AddRange(words[i]);

            if (i < gaps) {
                int spacesToAdd = baseSpaces + (i < remainder ? 1 : 0);

                for (int s = 0; s < spacesToAdd; s++) {
                    result.Add(new AnsiChar(' ', spaceStyle));
                }
            }
        }

        return new AnsiString(result);
    }

    public Size2D GetMinSize(string str) {
        if (string.IsNullOrEmpty(str))
            return Size2D.Zero;

        var normalized = str.Replace("\r\n", "\n").Replace('\r', '\n');
        var words      = normalized.Split(' ', '\n');
        int maxLength  = 0;

        for (int i = 0; i < words.Length; i++) {
            if (words[i].Length > maxLength)
                maxLength = words[i].Length;
        }

        return maxLength <= 0
            ? Size2D.Zero
            : GetSize(normalized, maxLength);
    }

    public Size2D GetSize(string str) {
        if (string.IsNullOrEmpty(str))
            return Size2D.Zero;

        var lines = str.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        int maxW  = 0;

        for (int i = 0; i < lines.Length; i++) {
            if (lines[i].Length > maxW)
                maxW = lines[i].Length;
        }

        return new Size2D(maxW, lines.Length);
    }

    public Size2D GetSize(string str, int maxWidth) {
        return string.IsNullOrEmpty(str) || maxWidth <= 0
            ? Size2D.Zero
            : GetSize(new AnsiString(str), maxWidth);
    }

    public Size2D GetMinSize(AnsiString str) => GetMinSize(str.Plain);

    public Size2D GetSize(AnsiString str) {
        return AnsiString.IsNullOrEmpty(str)
            ? Size2D.Zero
            : GetSize(str.Plain);
    }

    public Size2D GetSize(AnsiString str, int maxWidth) {
        if (AnsiString.IsNullOrEmpty(str) || maxWidth <= 0)
            return Size2D.Zero;

        var wrapped = WrapLines(str, maxWidth, WordWrapType.WORD);

        if (wrapped.Length == 0)
            return Size2D.Zero;

        int maxW = 0;

        for (int i = 0; i < wrapped.Length; i++) {
            if (wrapped[i].Line.Length > maxW)
                maxW = wrapped[i].Line.Length;
        }

        return new Size2D(maxW, wrapped.Length);
    }

    public virtual Size2D GetSize(AnsiString str, TextRenderingOptions options, int? maxWidth = null) {
        if (AnsiString.IsNullOrEmpty(str))
            return Size2D.Zero;

        int width = maxWidth ?? int.MaxValue;

        if (width <= 0)
            return Size2D.Zero;

        int leading       = options.Spacing != null && options.Spacing.Leading > 0 ? options.Spacing.Leading : 1;
        int letterSpacing = options.Spacing != null ? Math.Max(0, options.Spacing.LetterSpacing) : 0;
        int wordSpacing   = options.Spacing != null ? Math.Max(0, options.Spacing.WordSpacing) : 0;

        var wrappedLines = WrapLines(str, width, options.WordWrap, letterSpacing, wordSpacing);

        if (wrappedLines.Length == 0)
            return Size2D.Zero;

        int totalHeight = (wrappedLines.Length - 1) * leading + 1;
        int maxW        = 0;

        for (int i = 0; i < wrappedLines.Length; i++) {
            var (line, wasWrapped) = wrappedLines[i];
            bool shouldJustify = options.JustifyText && (wasWrapped || (wrappedLines.Length == 1 && options.JustifyText));

            if (shouldJustify && width < int.MaxValue) {
                line = JustifyLine(line, width, letterSpacing, wordSpacing);
            }

            int lineW = ComputeLineWidth(line, letterSpacing, wordSpacing);
            maxW = Math.Max(maxW, lineW);
        }

        return new Size2D(maxW, totalHeight);
    }
}