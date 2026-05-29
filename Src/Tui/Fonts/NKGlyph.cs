// NeoKolors
// Copyright (c) krystof 2026

using System.Text;
using Metriks;
using NeoKolors.Console;
using static NeoKolors.Tui.Fonts.Serialization.Xml.V3.XmlGlyphAlignDirection;
using Drct = NeoKolors.Tui.Fonts.Serialization.Xml.V3.XmlGlyphAlignDirection;

namespace NeoKolors.Tui.Fonts;

/// <summary>
/// Represents the default implementation of a glyph (<see cref="IGlyph"/>) in NeoKolors.Tui.
/// </summary>
public class NKGlyph : IGlyph {
    private static readonly NKLogger LOGGER = NKDebug.GetLogger<NKGlyph>();

    /// <inheritdoc/>
    public GlyphCell[,] Glyph { get; }

    public int    Width  => Glyph.Len0;
    public int    Height => Glyph.Len1;
    public Size2D Size   => Glyph.Size;

    /// <inheritdoc/>
    public int BaselineOffset { get; }

    /// <inheritdoc/>
    public AlignPointCollection AlignPoints { get; }

    public NKGlyph(GlyphCell[,] glyph, int baselineOffset) 
        : this(glyph, baselineOffset, new AlignPointCollection()) { }
    
    public NKGlyph(GlyphCell[,] glyph, int baselineOffset, AlignPoint[] alignPoints) {
        Glyph = glyph;
        BaselineOffset = baselineOffset;
        AlignPoints = new AlignPointCollection(alignPoints);
    }

    public NKGlyph(GlyphCell[,] glyph, int baselineOffset, AlignPointCollection alignPoints) {
        Glyph = glyph;
        BaselineOffset = baselineOffset;
        AlignPoints = alignPoints;
    }

    /// <summary>
    /// Combines two NKGlyph instances into a single, compound NKGlyph based on a specified alignment point.
    /// </summary>
    /// <param name="main">The main NKGlyph to be used as the primary component in the combination.</param>
    /// <param name="secondary">The secondary NKGlyph to be combined with the main glyph.</param>
    /// <param name="alignPoint">The character representing the alignment point used to align the two glyphs.</param>
    /// <returns>
    /// A new NKGlyph representing the combined glyphs if the alignment point exists in both glyphs; otherwise, null.
    /// </returns>
    public static NKGlyph? Combine(NKGlyph main, NKGlyph secondary, char alignPoint) {
        if (!main.AlignPoints.TryGetValue(alignPoint, out var mainAlignPoint)) {
            LOGGER.Error(
                $"Could not create compound glyph. " +
                $"Main glyph does not contain the specified align point ('{alignPoint}')."
            );

            return null;
        }

        if (!secondary.AlignPoints.TryGetValue(alignPoint, out var secondaryAlignPoint)) {
            LOGGER.Error(
                $"Could not create compound glyph. " +
                $"Secondary glyph does not contain the specified align point ('{alignPoint}')."
            );

            return null;
        }

        var transpositionVector = mainAlignPoint.Position - secondaryAlignPoint.Position;

        var minCoords = Point2D.Min(Point2D.Zero,        transpositionVector - secondaryAlignPoint.Position);
        var maxCoords = Point2D.Max(main.Size.ToPoint(), transpositionVector + secondaryAlignPoint.Position);

        var newSize  = (maxCoords - minCoords).ToSize();
        var newGlyph = new GlyphCell[newSize.X, newSize.Y];

        var mainOffset      = Point2D.Max(Point2D.Zero, -transpositionVector);
        var secondaryOffset = Point2D.Max(Point2D.Zero,  transpositionVector);

        Array2D.Copy(main     .Glyph, Point2D.Zero, newGlyph, mainOffset,      main     .Size);
        Array2D.Copy(secondary.Glyph, Point2D.Zero, newGlyph, secondaryOffset, secondary.Size);

        var newBaseline = main.BaselineOffset -
            Math.Max(
                0,
                transpositionVector.Y
                + secondary.Height
                - main.Height
            );

        var newAlignPoints = new AlignPointCollection(main.AlignPoints);
        newAlignPoints.AddRange(secondary.AlignPoints);
        newAlignPoints.Remove(alignPoint);

        return new NKGlyph(newGlyph, newBaseline, newAlignPoints);
    }

    /// <summary>
    /// Combines two NKGlyph instances into a single, compound NKGlyph based on a specified alignment direction.
    /// </summary>
    /// <param name="main">The primary NKGlyph to serve as the base for the combination.</param>
    /// <param name="secondary">The secondary NKGlyph to be combined with the primary glyph.</param>
    /// <param name="alignDirection">The directional alignment to be used to combine the two glyphs.</param>
    /// <returns>
    /// A new NKGlyph representing the combined glyphs based on the specified alignment direction, or null if the combination fails.
    /// </returns>
    public static NKGlyph Combine(NKGlyph main, NKGlyph secondary, Drct alignDirection) {
        var mainAnchor      = GetMainAnchor(main, alignDirection);
        var secondaryAnchor = GetSecondaryAnchor(secondary, alignDirection);

        var transpositionVector = mainAnchor - secondaryAnchor + CreateTranspositionOffset(alignDirection);

        var minCoords = Point2D.Min(Point2D.Zero,        transpositionVector);
        var maxCoords = Point2D.Max(main.Size.ToPoint(), transpositionVector + secondary.Size.ToPoint());

        var newSize  = (maxCoords - minCoords).ToSize();
        var newGlyph = new GlyphCell[newSize.X, newSize.Y];

        var mainOffset      = Point2D.Max(Point2D.Zero, -minCoords);
        var secondaryOffset = Point2D.Max(Point2D.Zero, transpositionVector - minCoords);

        Array2D.Copy(main     .Glyph, Point2D.Zero, newGlyph, mainOffset,      main     .Size);
        Array2D.Copy(secondary.Glyph, Point2D.Zero, newGlyph, secondaryOffset, secondary.Size);

        // Calculate the new baseline by taking the maximum of the adjusted baselines of both glyphs
        var newBaseline = Math.Max(main.BaselineOffset + mainOffset.Y, secondary.BaselineOffset + secondaryOffset.Y);

        var newAlignPoints = new AlignPointCollection(main.AlignPoints);
        newAlignPoints.AddRange(secondary.AlignPoints);

        return new NKGlyph(newGlyph, newBaseline, newAlignPoints);
    }

    private static Point2D CreateTranspositionOffset(Drct alignDirection) {
        return alignDirection switch {
            TOP    => new Point2D( 0, -1),
            BOTTOM => new Point2D( 0,  1),
            LEFT   => new Point2D(-1,  0),
            RIGHT  => new Point2D( 1,  0),
            
            TOP_LEFT     or LEFT_TOP     or CORNER_TOP_LEFT     => new Point2D(-1, -1),
            TOP_RIGHT    or RIGHT_TOP    or CORNER_TOP_RIGHT    => new Point2D( 1, -1),
            LEFT_BOTTOM  or BOTTOM_LEFT  or CORNER_BOTTOM_LEFT  => new Point2D(-1,  1),
            BOTTOM_RIGHT or RIGHT_BOTTOM or CORNER_BOTTOM_RIGHT => new Point2D( 1,  1),
            
            _ => Point2D.Zero
        };
    }

    private static Point2D GetMainAnchor(NKGlyph glyph, Drct direction) {
        var centerX = glyph.Width  / 2;
        var centerY = glyph.Height / 2;

        return direction switch {
            TOP    => new Point2D(centerX,         0),
            BOTTOM => new Point2D(centerX,         glyph.Height - 1),
            LEFT   => new Point2D(0,               centerY),
            RIGHT  => new Point2D(glyph.Width - 1, centerY),

            TOP_LEFT     or LEFT_TOP     or CORNER_TOP_LEFT     => new Point2D(0,               0),
            TOP_RIGHT    or RIGHT_TOP    or CORNER_TOP_RIGHT    => new Point2D(glyph.Width - 1, 0),
            LEFT_BOTTOM  or BOTTOM_LEFT  or CORNER_BOTTOM_LEFT  => new Point2D(0,               glyph.Height - 1),
            BOTTOM_RIGHT or RIGHT_BOTTOM or CORNER_BOTTOM_RIGHT => new Point2D(glyph.Width - 1, glyph.Height - 1),

            _ => new Point2D(0, 0)
        };
    }

    private static Point2D GetSecondaryAnchor(NKGlyph glyph, Drct direction) {
        var centerX = glyph.Width  / 2;
        var centerY = glyph.Height / 2;

        return direction switch {
            TOP    => new Point2D(centerX,         glyph.Height - 1),
            BOTTOM => new Point2D(centerX,         0),
            LEFT   => new Point2D(glyph.Width - 1, centerY),
            RIGHT  => new Point2D(0,               centerY),

            BOTTOM_RIGHT or RIGHT_BOTTOM or CORNER_BOTTOM_RIGHT => new Point2D(0,               0),
            LEFT_BOTTOM  or BOTTOM_LEFT  or CORNER_BOTTOM_LEFT  => new Point2D(glyph.Width - 1, 0),
            TOP_RIGHT    or RIGHT_TOP    or CORNER_TOP_RIGHT    => new Point2D(0,               glyph.Height - 1),
            TOP_LEFT     or LEFT_TOP     or CORNER_TOP_LEFT     => new Point2D(glyph.Width - 1, glyph.Height - 1),

            _ => new Point2D(0, 0)
        };
    }

    /// <summary>
    /// Converts the glyph into a string representation suitable for textual rendering.
    /// </summary>
    /// <returns>
    /// A string where each line represents a row of the glyph and each character corresponds to a
    /// <see cref="GlyphCell"/>, depending on the cell's type.
    /// </returns>
    public string AsString {
        get {
            var sb = new StringBuilder();

            for (int y = 0; y < Height; y++) {
                for (int x = 0; x < Width; x++) {
                    var cell = Glyph[x, y];
                    sb.Append(cell.Type == GlyphCellType.CHARACTER ? cell.Character : ' ');
                }
                
                sb.AppendLine();
            }
            
            return sb.ToString();
        }
    }

    /// <summary>
    /// Retrieves the rendered lines of the glyph as an array of strings,
    /// where each string represents a single row of the glyph.
    /// </summary>
    /// <remarks>
    /// Each character in the resulting strings corresponds to a cell in the glyph's grid.
    /// Non-character cells are replaced with a space character (' ').
    /// </remarks>
    public string[] AsLines {
        get {
            var lines = new string[Height];

            for (int y = 0; y < Height; y++) {
                var sb = new StringBuilder();
                
                for (int x = 0; x < Width; x++) {
                    var cell = Glyph[x, y];
                    sb.Append(cell.Type == GlyphCellType.CHARACTER ? cell.Character : ' ');
                }
                
                lines[y] = sb.ToString();
            }
            
            return lines;
        }
    }
}