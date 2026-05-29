// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Tui.Fonts.Serialization.Xml.V3;
using OneOf;
using OneOf.Types;
using static NeoKolors.Tui.Fonts.Serialization.Xml.V3.XmlGlyphAlignPointReplaceType;

namespace NeoKolors.Tui.Fonts.Serialization;

internal static class NKFontSerializerHelper {
    
    /// <summary>
    /// Creates a 2D array of <see cref="GlyphCell"/> objects representing the character data from the provided
    /// lines of text. Each character in the input is mapped to a <see cref="GlyphCell"/> with the corresponding
    /// character. Any empty space in the array is filled with <see cref="GlyphCell.Background"/>.
    /// </summary>
    /// <param name="lines">
    /// An array of strings, where each string represents a line of text to be converted into
    /// <see cref="GlyphCell"/> data.
    /// </param>
    /// <returns>
    /// A two-dimensional array of <see cref="GlyphCell"/> objects representing the input character data.
    /// </returns>
    internal static GlyphCell[,] CreateFromLines(string[] lines) {
        int max = 0;

        for (int y = 0; y < lines.Length; y++) {
            max = Math.Max(max, lines[y].Length);
        }

        var glyph = new GlyphCell[max, lines.Length];

        for (int y = 0; y < lines.Length; y++) {
            for (int x = 0; x < max; x++) {
                glyph[x, y] = x < lines[y].Length ? GlyphCell.Char(lines[y][x]) : GlyphCell.Background;
            }
        }
        
        return glyph;
    }

    /// <summary>
    /// Detects alignment points within a grid of <see cref="GlyphCell"/> objects based on specified marker characters
    /// and replacement rules. Align points are identified by their associated marker characters and positions in the grid,
    /// while unprocessed or failed alignments are collected separately.
    /// </summary>
    /// <param name="glyph">
    /// A two-dimensional array of <see cref="GlyphCell"/> objects representing the grid to be processed for alignment points.
    /// </param>
    /// <param name="markers">
    /// A set of characters that serve as markers to identify potential alignment points within the grid.
    /// </param>
    /// <param name="defaultReplace">
    /// Specifies the default behavior for replacing alignment points when their associated marker characters
    /// are not found in replacement rules or foreground/background character sets.
    /// </param>
    /// <param name="forg">
    /// A set of characters designated as foreground. These characters are used to replace alignment point markers if applicable.
    /// </param>
    /// <param name="bckg">
    /// A set of characters designated as background. These characters are used to replace alignment point markers if applicable.
    /// </param>
    /// <param name="pairs">
    /// A dictionary mapping marker characters to their replacement characters. This is used to replace alignment point markers
    /// with custom defined values.
    /// </param>
    /// <returns>
    /// A tuple containing two elements:
    /// - <see cref="HashSet{T}"/> of <see cref="AlignPoint"/> representing the successfully identified alignment points.
    /// - An array of <see cref="AlignPoint"/> representing alignment points that failed to process or could not be added.
    /// </returns>
    internal static (HashSet<AlignPoint> AlignPoints, AlignPoint[] Failed) DetectAlignPoints(
        GlyphCell[,] glyph,
        HashSet<char> markers,
        XmlGlyphAlignPointReplaceType defaultReplace,
        HashSet<char> forg,
        HashSet<char>                 bckg,
        Dictionary<char, char>        pairs) 
    {
        var alignPoints = new HashSet<AlignPoint>();
        var failed      = new List   <AlignPoint>();
        
        for (int x = 0; x < glyph.Len0; x++) {
            for (int y = 0; y < glyph.Len1; y++) {
                if (glyph[x, y].Type != GlyphCellType.CHARACTER) 
                    continue;
                
                var c = glyph[x, y].Character;
                
                if (!markers.Contains(c)) 
                    continue;

                var ap = new AlignPoint(c, new Point2D(x, y));

                if (!alignPoints.Add(ap)) {
                    failed.Add(ap);
                    
                    continue;
                }

                glyph[x, y] = pairs.TryGetValue(c, out var r)
                    ? GlyphCell.Char(r)
                    : forg.Contains(c)
                        ? GlyphCell.Foreground
                        : bckg.Contains(c)
                            ? GlyphCell.Background
                            : defaultReplace switch {
                                FORG        => GlyphCell.Foreground,
                                BCKG        => GlyphCell.Background,
                                _           => GlyphCell.Char(c),
                            };
            }
        }
        
        return (alignPoints, failed.ToArray());
    }

    /// <summary>
    /// Applies masking to the specified 2D array of <see cref="GlyphCell"/> objects based on the provided
    /// foreground and background character sets, while interpreting spaces according to the space mask configuration.
    /// </summary>
    /// <param name="chars">
    /// A two-dimensional array of <see cref="GlyphCell"/> objects to be processed for masking.
    /// </param>
    /// <param name="spaceConf">
    /// The space masking configuration.
    /// </param>
    /// <param name="forg">
    /// An array of characters that should be treated as foreground.
    /// </param>
    /// <param name="bckg">
    /// An array of characters that should be treated as the background.
    /// </param>
    internal static void Mask(GlyphCell[,] chars, XmlSpaceMask spaceConf, char[] forg, char[] bckg) {
        var fh = forg.ToHashSet();
        var bh = bckg.ToHashSet();

        for (int x = 0; x < chars.Len0; x++) {
            for (int y = 0; y < chars.Len1; y++) {
                var cell = chars[x, y];

                if (cell.Type != GlyphCellType.CHARACTER) continue;

                var c = cell.Character;
                
                chars[x, y] = c is ' '
                    ? spaceConf switch {
                        XmlSpaceMask.FOREGROUND => GlyphCell.Foreground,
                        XmlSpaceMask.SPACE_CHAR => cell,
                        _                       => GlyphCell.Background
                    }
                    : bh.Contains(c)               // if it's not a space,
                        ? GlyphCell.Background     // try to mask it as background
                        : fh.Contains(c)           // and if it cannot be masked as background 
                            ? GlyphCell.Foreground // try to mask it as foreground
                            : cell;                // and if it cannot be masked as foreground, let it be
            }
        }
    }

    /// <summary>
    /// Reduces the size of the given glyph by trimming any rows and columns
    /// that only contain background cells. The resulting glyph will contain
    /// the smallest bounding area with non-background cells.
    /// </summary>
    /// <param name="glyph">
    /// A two-dimensional array of <see cref="GlyphCell"/> objects representing the original glyph.
    /// </param>
    /// <returns>
    /// A <see cref="OneOf"/> value containing either a <see cref="Success{T}"/> with the reduced glyph
    /// and its offset as a tuple, or an <see cref="Error{T}"/> with an error message if the input glyph
    /// is invalid or contains no non-background cells.
    /// </returns>
    internal static OneOf<Success<(GlyphCell[,] Glyph, Point2D Offset)>, Error<string>> Reduce(GlyphCell[,] glyph) {
        
        // define the indices for left-most (rlx), right-most (rrx),
        // top-most (rty) and bottom-most (rby) columns/lines that do
        // contain at least one non-background cell 
        // if the -1 value remains after the search, the index has not been found
        int rlx = -1;
        int rrx = -1;
        int rty = -1;
        int rby = -1;
        
        // left-most
        for (int x = 0; x < glyph.Len0; x++) {
            bool nb = false;

            for (int y = 0; y < glyph.Len1; y++) {
                if (glyph[x, y].Type == GlyphCellType.BACKGROUND) 
                    continue;

                nb = true;
                break;
            }

            if (!nb) continue;

            rlx = x;

            break;
        }
        
        // right-most
        for (int x = glyph.Len0 - 1; x >= 0; x--) {
            bool nb = false;

            for (int y = 0; y < glyph.Len1; y++) {
                if (glyph[x, y].Type == GlyphCellType.BACKGROUND) 
                    continue;

                nb = true;
                break;
            }

            if (!nb) continue;

            rrx = x;

            break;
        }
        
        // top-most
        for (int y = 0; y < glyph.Len1; y++) {
            bool nb = false;

            for (int x = 0; x < glyph.Len0; x++) {
                if (glyph[x, y].Type == GlyphCellType.BACKGROUND) 
                    continue;

                nb = true;
                break;
            }
            
            if (!nb) continue;

            rty = y;

            break;
        }
        
        // bottom-most
        for (int y = glyph.Len1 - 1; y >= 0; y--) {
            bool nb = false;

            for (int x = 0; x < glyph.Len0; x++) {
                if (glyph[x, y].Type == GlyphCellType.BACKGROUND) 
                    continue;

                nb = true;
                break;
            }
            
            if (!nb) continue;

            rby = y;

            break;
        }

        const string xErr = "Could not find a column inside the glyph containing at least one non-background cell.";
        const string yErr = "Could not find a row inside the glyph containing at least one non-background cell.";
        
        // check for errors
        if (rlx == -1 || rrx == -1)
            return new Error<string>(xErr);

        if (rty == -1 || rby == -1)
            return new Error<string>(yErr);

        // compute new glyph sizes
        var rw = rrx - rlx + 1;
        var rh = rby - rty + 1;

        var reduced = new GlyphCell[rw, rh];
        
        // copy the glyph
        Array2D.Copy(glyph, new Point2D(rlx, rty), reduced, Point2D.Zero, new Size2D(rw, rh));

        return new Success<(GlyphCell[,], Point2D)>((reduced, new Point2D(glyph.Len0 -  rlx, glyph.Len1 - rby)));
    } 
}