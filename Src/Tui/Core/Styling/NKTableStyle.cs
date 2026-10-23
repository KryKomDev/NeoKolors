// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Common;
using static NeoKolors.Tui.Core.NKTableStyle.Features;

namespace NeoKolors.Tui.Core;

public record NKTableStyle : ITableStyle {

    public Features EnabledFeatures { get; init; }

    public AnsiChar TopLeft             { get; init; }
    public AnsiChar Top                 { get; init; }
    public AnsiChar TopRight            { get; init; }
    public AnsiChar Right               { get; init; }
    public AnsiChar BottomRight         { get; init; }
    public AnsiChar Bottom              { get; init; }
    public AnsiChar BottomLeft          { get; init; }
    public AnsiChar Left                { get; init; }
    public AnsiChar LeftT               { get; init; }
    public AnsiChar RightT              { get; init; }
    public AnsiChar BottomT             { get; init; }
    public AnsiChar TopT                { get; init; }
    public AnsiChar Cross               { get; init; }
    public AnsiChar VerticalSeparator   { get; init; }
    public AnsiChar HorizontalSeparator { get; init; }

    public AnsiChar Header_TopLeft  { get; init; }
    public AnsiChar Header_Top      { get; init; }
    public AnsiChar Header_TopRight { get; init; }
    public AnsiChar Header_Right    { get; init; }
    public AnsiChar Header_Bottom   { get; init; }
    public AnsiChar Header_Left     { get; init; }
    public AnsiChar Header_LeftT    { get; init; }
    public AnsiChar Header_RightT   { get; init; }
    public AnsiChar Header_TopT     { get; init; }
    public AnsiChar Header_Cross    { get; init; }

    public bool IsBorderless            => EnabledFeatures                                  == NONE;
    public bool HasBorderLeft           => (EnabledFeatures & BORDER_LEFT)                  != 0;
    public bool HasBorderRight          => (EnabledFeatures & BORDER_RIGHT)                 != 0;
    public bool HasBorderTop            => (EnabledFeatures & BORDER_TOP)                   != 0;
    public bool HasBorderBottom         => (EnabledFeatures & BORDER_BOTTOM)                != 0;
    public bool HasVerticalSeparators   => (EnabledFeatures & VERTICAL_ELEMENT_SEPARATOR)   != 0;
    public bool HasHorizontalSeparators => (EnabledFeatures & HORIZONTAL_ELEMENT_SEPARATOR) != 0;
    public bool HasHeaderRow            => (EnabledFeatures & HEADER_ROW)                   != 0;
    public bool HasHeaderSeparator      => (EnabledFeatures & HEADER_SEPARATOR)             != 0;

    #region Constructors

    /// <summary>
    ///     Initializes a new instance of the <see cref="NKTableStyle" /> record with individual
    ///     <see cref="AnsiChar" /> elements
    ///     for all perimeter, separator, and header parts.
    /// </summary>
    public NKTableStyle(
        AnsiChar topLeft,
        AnsiChar top,
        AnsiChar topRight,
        AnsiChar right,
        AnsiChar bottomRight,
        AnsiChar bottom,
        AnsiChar bottomLeft,
        AnsiChar left,
        AnsiChar leftT,
        AnsiChar rightT,
        AnsiChar bottomT,
        AnsiChar topT,
        AnsiChar cross,
        AnsiChar verticalSeparator,
        AnsiChar horizontalSeparator,
        AnsiChar headerTopLeft,
        AnsiChar headerTop,
        AnsiChar headerTopRight,
        AnsiChar headerRight,
        AnsiChar headerBottom,
        AnsiChar headerLeft,
        AnsiChar headerLeftT,
        AnsiChar headerRightT,
        AnsiChar headerTopT,
        AnsiChar headerCross,
        Features enabledFeatures = DEFAULT
    ) {
        EnabledFeatures     = enabledFeatures;
        TopLeft             = topLeft;
        Top                 = top;
        TopRight            = topRight;
        Right               = right;
        BottomRight         = bottomRight;
        Bottom              = bottom;
        BottomLeft          = bottomLeft;
        Left                = left;
        LeftT               = leftT;
        RightT              = rightT;
        BottomT             = bottomT;
        TopT                = topT;
        Cross               = cross;
        VerticalSeparator   = verticalSeparator;
        HorizontalSeparator = horizontalSeparator;
        Header_TopLeft      = headerTopLeft;
        Header_Top          = headerTop;
        Header_TopRight     = headerTopRight;
        Header_Right        = headerRight;
        Header_Bottom       = headerBottom;
        Header_Left         = headerLeft;
        Header_LeftT        = headerLeftT;
        Header_RightT       = headerRightT;
        Header_TopT         = headerTopT;
        Header_Cross        = headerCross;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NKTableStyle" /> record with 15
    ///     <see cref="AnsiChar" /> elements,
    ///     defaulting header characters to match the body table characters.
    /// </summary>
    public NKTableStyle(
        AnsiChar topLeft,
        AnsiChar top,
        AnsiChar topRight,
        AnsiChar right,
        AnsiChar bottomRight,
        AnsiChar bottom,
        AnsiChar bottomLeft,
        AnsiChar left,
        AnsiChar leftT,
        AnsiChar rightT,
        AnsiChar bottomT,
        AnsiChar topT,
        AnsiChar cross,
        AnsiChar verticalSeparator,
        AnsiChar horizontalSeparator,
        Features enabledFeatures = DEFAULT
    ) : this(
        topLeft,
        top,
        topRight,
        right,
        bottomRight,
        bottom,
        bottomLeft,
        left,
        leftT,
        rightT,
        bottomT,
        topT,
        cross,
        verticalSeparator,
        horizontalSeparator,
        topLeft,
        top,
        topRight,
        right,
        horizontalSeparator,
        left,
        leftT,
        rightT,
        topT,
        cross,
        enabledFeatures
    ) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NKTableStyle" /> record with characters
    ///     and individual styles.
    /// </summary>
    public NKTableStyle(
        char     topLeft,
        char     top,
        char     topRight,
        char     right,
        char     bottomRight,
        char     bottom,
        char     bottomLeft,
        char     left,
        char     leftT,
        char     rightT,
        char     bottomT,
        char     topT,
        char     cross,
        char     verticalSeparator,
        char     horizontalSeparator,
        NKStyle  topLeftStyle,
        NKStyle  topStyle,
        NKStyle  topRightStyle            = default,
        NKStyle  rightStyle               = default,
        NKStyle  bottomRightStyle         = default,
        NKStyle  bottomStyle              = default,
        NKStyle  bottomLeftStyle          = default,
        NKStyle  leftStyle                = default,
        NKStyle  leftTStyle               = default,
        NKStyle  rightTStyle              = default,
        NKStyle  bottomTStyle             = default,
        NKStyle  topTStyle                = default,
        NKStyle  crossStyle               = default,
        NKStyle  verticalSeparatorStyle   = default,
        NKStyle  horizontalSeparatorStyle = default,
        Features enabledFeatures          = DEFAULT
    ) : this(
        new AnsiChar(topLeft,             topLeftStyle),
        new AnsiChar(top,                 topStyle),
        new AnsiChar(topRight,            topRightStyle),
        new AnsiChar(right,               rightStyle),
        new AnsiChar(bottomRight,         bottomRightStyle),
        new AnsiChar(bottom,              bottomStyle),
        new AnsiChar(bottomLeft,          bottomLeftStyle),
        new AnsiChar(left,                leftStyle),
        new AnsiChar(leftT,               leftTStyle),
        new AnsiChar(rightT,              rightTStyle),
        new AnsiChar(bottomT,             bottomTStyle),
        new AnsiChar(topT,                topTStyle),
        new AnsiChar(cross,               crossStyle),
        new AnsiChar(verticalSeparator,   verticalSeparatorStyle),
        new AnsiChar(horizontalSeparator, horizontalSeparatorStyle),
        enabledFeatures
    ) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NKTableStyle" /> record with characters
    ///     and a shared style.
    /// </summary>
    public NKTableStyle(
        char     topLeft,
        char     top,
        char     topRight,
        char     right,
        char     bottomRight,
        char     bottom,
        char     bottomLeft,
        char     left,
        char     leftT,
        char     rightT,
        char     bottomT,
        char     topT,
        char     cross,
        char     verticalSeparator,
        char     horizontalSeparator,
        NKStyle  style           = default,
        Features enabledFeatures = DEFAULT
    ) : this(
        new AnsiChar(topLeft,             style),
        new AnsiChar(top,                 style),
        new AnsiChar(topRight,            style),
        new AnsiChar(right,               style),
        new AnsiChar(bottomRight,         style),
        new AnsiChar(bottom,              style),
        new AnsiChar(bottomLeft,          style),
        new AnsiChar(left,                style),
        new AnsiChar(leftT,               style),
        new AnsiChar(rightT,              style),
        new AnsiChar(bottomT,             style),
        new AnsiChar(topT,                style),
        new AnsiChar(cross,               style),
        new AnsiChar(verticalSeparator,   style),
        new AnsiChar(horizontalSeparator, style),
        enabledFeatures
    ) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NKTableStyle" /> record using line and
    ///     corner characters with individual styles.
    /// </summary>
    public NKTableStyle(
        char     vertical,
        char     horizontal,
        char     topLeft,
        char     topRight,
        char     bottomLeft,
        char     bottomRight,
        char     leftT,
        char     rightT,
        char     bottomT,
        char     topT,
        char     cross,
        NKStyle  verticalStyle,
        NKStyle  horizontalStyle,
        NKStyle  topLeftStyle     = default,
        NKStyle  topRightStyle    = default,
        NKStyle  bottomLeftStyle  = default,
        NKStyle  bottomRightStyle = default,
        NKStyle  leftTStyle       = default,
        NKStyle  rightTStyle      = default,
        NKStyle  bottomTStyle     = default,
        NKStyle  topTStyle        = default,
        NKStyle  crossStyle       = default,
        Features enabledFeatures  = DEFAULT
    ) : this(
        topLeft,
        horizontal,
        topRight,
        vertical,
        bottomRight,
        horizontal,
        bottomLeft,
        vertical,
        leftT,
        rightT,
        bottomT,
        topT,
        cross,
        vertical,
        horizontal,
        topLeftStyle,
        horizontalStyle,
        topRightStyle,
        verticalStyle,
        bottomRightStyle,
        horizontalStyle,
        bottomLeftStyle,
        verticalStyle,
        leftTStyle,
        rightTStyle,
        bottomTStyle,
        topTStyle,
        crossStyle,
        verticalStyle,
        horizontalStyle,
        enabledFeatures
    ) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NKTableStyle" /> record using line,
    ///     corner, and junction characters with a shared style.
    /// </summary>
    public NKTableStyle(
        char     vertical,
        char     horizontal,
        char     topLeft,
        char     topRight,
        char     bottomLeft,
        char     bottomRight,
        char     leftT,
        char     rightT,
        char     bottomT,
        char     topT,
        char     cross,
        NKStyle  style           = default,
        Features enabledFeatures = DEFAULT
    ) : this(
        topLeft,
        horizontal,
        topRight,
        vertical,
        bottomRight,
        horizontal,
        bottomLeft,
        vertical,
        leftT,
        rightT,
        bottomT,
        topT,
        cross,
        vertical,
        horizontal,
        style,
        enabledFeatures
    ) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NKTableStyle" /> record with dedicated
    ///     header line styling.
    /// </summary>
    public NKTableStyle(
        char     vertical,
        char     horizontal,
        char     topLeft,
        char     topRight,
        char     bottomLeft,
        char     bottomRight,
        char     leftT,
        char     rightT,
        char     bottomT,
        char     topT,
        char     cross,
        char     headerHorizontal,
        char     headerLeftT,
        char     headerRightT,
        char     headerTopT,
        char     headerCross,
        NKStyle  bodyStyle       = default,
        NKStyle  headerStyle     = default,
        Features enabledFeatures = DEFAULT
    ) : this(
        new AnsiChar(topLeft,          bodyStyle),
        new AnsiChar(horizontal,       bodyStyle),
        new AnsiChar(topRight,         bodyStyle),
        new AnsiChar(vertical,         bodyStyle),
        new AnsiChar(bottomRight,      bodyStyle),
        new AnsiChar(horizontal,       bodyStyle),
        new AnsiChar(bottomLeft,       bodyStyle),
        new AnsiChar(vertical,         bodyStyle),
        new AnsiChar(leftT,            bodyStyle),
        new AnsiChar(rightT,           bodyStyle),
        new AnsiChar(bottomT,          bodyStyle),
        new AnsiChar(topT,             bodyStyle),
        new AnsiChar(cross,            bodyStyle),
        new AnsiChar(vertical,         bodyStyle),
        new AnsiChar(horizontal,       bodyStyle),
        new AnsiChar(topLeft,          headerStyle != default ? headerStyle : bodyStyle),
        new AnsiChar(horizontal,       headerStyle != default ? headerStyle : bodyStyle),
        new AnsiChar(topRight,         headerStyle != default ? headerStyle : bodyStyle),
        new AnsiChar(vertical,         headerStyle != default ? headerStyle : bodyStyle),
        new AnsiChar(headerHorizontal, headerStyle != default ? headerStyle : bodyStyle),
        new AnsiChar(vertical,         headerStyle != default ? headerStyle : bodyStyle),
        new AnsiChar(headerLeftT,      headerStyle != default ? headerStyle : bodyStyle),
        new AnsiChar(headerRightT,     headerStyle != default ? headerStyle : bodyStyle),
        new AnsiChar(headerTopT,       headerStyle != default ? headerStyle : bodyStyle),
        new AnsiChar(headerCross,      headerStyle != default ? headerStyle : bodyStyle),
        enabledFeatures
    ) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NKTableStyle" /> record using line,
    ///     corner, T-junction, and cross characters.
    /// </summary>
    public NKTableStyle(
        char     vertical,
        char     horizontal,
        char     corner,
        char     tJunction,
        char     cross,
        NKStyle  style           = default,
        Features enabledFeatures = DEFAULT
    ) : this(
        vertical,
        horizontal,
        corner,
        corner,
        corner,
        corner,
        tJunction,
        tJunction,
        tJunction,
        tJunction,
        cross,
        style,
        enabledFeatures
    ) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NKTableStyle" /> record using line and
    ///     junction characters.
    /// </summary>
    public NKTableStyle(
        char     vertical,
        char     horizontal,
        char     junction,
        NKStyle  style           = default,
        Features enabledFeatures = DEFAULT
    ) : this(
        vertical,
        horizontal,
        junction,
        junction,
        junction,
        style,
        enabledFeatures
    ) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NKTableStyle" /> record based on an
    ///     existing <see cref="NKBorderStyle" />.
    /// </summary>
    public NKTableStyle(
        NKBorderStyle border,
        char          leftT               = '+',
        char          rightT              = '+',
        char          bottomT             = '+',
        char          topT                = '+',
        char          cross               = '+',
        char?         verticalSeparator   = null,
        char?         horizontalSeparator = null,
        NKStyle       separatorStyle      = default,
        Features      enabledFeatures     = DEFAULT
    ) : this(
        border.TopLeft,
        border.Top,
        border.TopRight,
        border.Right,
        border.BottomRight,
        border.Bottom,
        border.BottomLeft,
        border.Left,
        new AnsiChar(leftT,                                   separatorStyle != default ? separatorStyle : border.Left.Style),
        new AnsiChar(rightT,                                  separatorStyle != default ? separatorStyle : border.Right.Style),
        new AnsiChar(bottomT,                                 separatorStyle != default ? separatorStyle : border.Bottom.Style),
        new AnsiChar(topT,                                    separatorStyle != default ? separatorStyle : border.Top.Style),
        new AnsiChar(cross,                                   separatorStyle != default ? separatorStyle : border.Top.Style),
        new AnsiChar(verticalSeparator   ?? border.Left.Char, separatorStyle != default ? separatorStyle : border.Left.Style),
        new AnsiChar(horizontalSeparator ?? border.Top.Char,  separatorStyle != default ? separatorStyle : border.Top.Style),
        border.IsBorderless ? NONE : enabledFeatures
    ) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NKTableStyle" /> record with default ASCII
    ///     characters and specified features.
    /// </summary>
    public NKTableStyle(Features enabledFeatures) : this('|', '-', '+', default, enabledFeatures) { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="NKTableStyle" /> record with default ASCII
    ///     characters and all features enabled.
    /// </summary>
    public NKTableStyle() : this('|', '-', '+', style: default, enabledFeatures: DEFAULT) { }

    #endregion

    #region Feature Methods

    /// <summary>
    ///     Returns a new <see cref="NKTableStyle" /> instance with the specified features set.
    /// </summary>
    public NKTableStyle WithFeatures(Features features) => this with { EnabledFeatures = features };

    /// <summary>
    ///     Returns a new <see cref="NKTableStyle" /> instance with the specified features enabled
    ///     in addition to current features.
    /// </summary>
    public NKTableStyle EnableFeatures(Features features) => this with { EnabledFeatures = EnabledFeatures | features };

    /// <summary>
    ///     Returns a new <see cref="NKTableStyle" /> instance with the specified features disabled
    ///     from current features.
    /// </summary>
    public NKTableStyle DisableFeatures(Features features) => this with { EnabledFeatures = EnabledFeatures & ~features };

    /// <summary>
    ///     Checks whether the specified feature or combination of features is enabled.
    /// </summary>
    public bool HasFeature(Features feature) => (EnabledFeatures & feature) == feature;

    #endregion

    #region Layout & Drawing Methods

    /// <inheritdoc />
    public void Draw(Point2D offset, Size2D[,] elements, in ICharDrawer drawer, int z) {
        var numRows = elements.GetLength(0);
        var numCols = elements.GetLength(1);

        if (numRows == 0 || numCols == 0)
            return;

        ComputeMetrics(
            elements,
            out var colWidths,
            out var rowHeights,
            out var colOffsets,
            out var rowOffsets,
            out var totalWidth,
            out var totalHeight,
            out var hasHeader,
            out var hasLeftBorder,
            out var hasRightBorder,
            out var hasTopBorder,
            out var hasBottomBorder,
            out var hasColSep,
            out var hasRowSep,
            out var hasHeaderSep
        );

        if (totalWidth <= 0 || totalHeight <= 0)
            return;

        // Top border
        if (hasTopBorder) {
            var topY = offset.Y;

            if (hasLeftBorder)
                drawer.Draw(hasHeader ? Header_TopLeft : TopLeft, offset.X, topY, z);

            for (var c = 0; c < numCols; c++) {
                var xStart = offset.X + colOffsets[c];
                var width  = colWidths[c];

                if (width > 0)
                    drawer.DrawHorizontalLine(hasHeader ? Header_Top : Top, xStart, topY, width, z);

                if (c < numCols - 1 && hasColSep) {
                    var sepX = xStart + width;
                    drawer.Draw(hasHeader ? Header_TopT : TopT, sepX, topY, z);
                }
            }

            if (hasRightBorder)
                drawer.Draw(hasHeader ? Header_TopRight : TopRight, offset.X + totalWidth - 1, topY, z);
        }

        // Bottom border
        if (hasBottomBorder) {
            var bottomY = offset.Y + totalHeight - 1;

            if (hasLeftBorder)
                drawer.Draw(BottomLeft, offset.X, bottomY, z);

            for (var c = 0; c < numCols; c++) {
                var xStart = offset.X + colOffsets[c];
                var width  = colWidths[c];

                if (width > 0)
                    drawer.DrawHorizontalLine(Bottom, xStart, bottomY, width, z);

                if (c < numCols - 1 && hasColSep) {
                    var sepX = xStart + width;
                    drawer.Draw(BottomT, sepX, bottomY, z);
                }
            }

            if (hasRightBorder)
                drawer.Draw(BottomRight, offset.X + totalWidth - 1, bottomY, z);
        }

        // Left border
        if (hasLeftBorder) {
            var leftX = offset.X;

            for (var r = 0; r < numRows; r++) {
                var yStart = offset.Y + rowOffsets[r];
                var height = rowHeights[r];

                if (height > 0) {
                    var leftChar = r == 0 && hasHeader ? Header_Left : Left;
                    drawer.DrawVerticalLine(leftChar, leftX, yStart, height, z);
                }
            }
        }

        // Right border
        if (hasRightBorder) {
            var rightX = offset.X + totalWidth - 1;

            for (var r = 0; r < numRows; r++) {
                var yStart = offset.Y + rowOffsets[r];
                var height = rowHeights[r];

                if (height > 0) {
                    var rightChar = r == 0 && hasHeader ? Header_Right : Right;
                    drawer.DrawVerticalLine(rightChar, rightX, yStart, height, z);
                }
            }
        }

        // Column vertical separators
        if (hasColSep)
            for (var c = 0; c < numCols - 1; c++) {
                var sepX = offset.X + colOffsets[c] + colWidths[c];

                for (var r = 0; r < numRows; r++) {
                    var yStart = offset.Y + rowOffsets[r];
                    var height = rowHeights[r];

                    if (height > 0)
                        drawer.DrawVerticalLine(VerticalSeparator, sepX, yStart, height, z);
                }
            }

        // Horizontal separators between rows (including Header separator)
        for (var r = 0; r < numRows - 1; r++) {
            var isHeaderRowSep = r == 0 && hasHeader;
            var drawSep        = isHeaderRowSep ? hasHeaderSep : hasRowSep;

            if (!drawSep)
                continue;

            var sepY = offset.Y + rowOffsets[r] + rowHeights[r];

            if (hasLeftBorder)
                drawer.Draw(isHeaderRowSep ? Header_LeftT : LeftT, offset.X, sepY, z);

            for (var c = 0; c < numCols; c++) {
                var xStart = offset.X + colOffsets[c];
                var width  = colWidths[c];

                if (width > 0)
                    drawer.DrawHorizontalLine(isHeaderRowSep ? Header_Bottom : HorizontalSeparator, xStart, sepY, width, z);

                if (c < numCols - 1 && hasColSep) {
                    var crossX = xStart + width;
                    drawer.Draw(isHeaderRowSep ? Header_Cross : Cross, crossX, sepY, z);
                }
            }

            if (hasRightBorder)
                drawer.Draw(isHeaderRowSep ? Header_RightT : RightT, offset.X + totalWidth - 1, sepY, z);
        }
    }

    /// <inheritdoc />
    public Point2D[,] GetElementOffsets(Size2D[,] elements) {
        var numRows = elements.GetLength(0);
        var numCols = elements.GetLength(1);

        if (numRows == 0 || numCols == 0)
            return new Point2D[numRows, numCols];

        ComputeMetrics(
            elements,
            out _,
            out _,
            out var colOffsets,
            out var rowOffsets,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _
        );

        var result = new Point2D[numRows, numCols];

        for (var r = 0; r < numRows; r++)
        for (var c = 0; c < numCols; c++)
            result[r, c] = new Point2D(colOffsets[c], rowOffsets[r]);

        return result;
    }

    /// <inheritdoc />
    public Size2D GetTotalSize(Size2D[,] elements) {
        var numRows = elements.GetLength(0);
        var numCols = elements.GetLength(1);

        if (numRows == 0 || numCols == 0)
            return Size2D.Zero;

        ComputeMetrics(
            elements,
            out _,
            out _,
            out _,
            out _,
            out var totalWidth,
            out var totalHeight,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _,
            out _
        );

        return new Size2D(totalWidth, totalHeight);
    }

    public Rect2D GetContentBox(Size2D size) {
        var left   = HasBorderLeft ? 1 : 0;
        var top    = HasBorderTop ? 1 : 0;
        var right  = HasBorderRight ? 1 : 0;
        var bottom = HasBorderBottom ? 1 : 0;

        return new Rect2D(
            new Point2D(left, top),
            new Size2D(Math.Max(0, size.X - left - right), Math.Max(0, size.Y - top - bottom))
        );
    }

    public Size2D GetBorderBox(Size2D size) {
        var w = size.X + (HasBorderLeft ? 1 : 0) + (HasBorderRight ? 1 : 0);
        var h = size.Y + (HasBorderTop ? 1 : 0)  + (HasBorderBottom ? 1 : 0);

        return new Size2D(w, h);
    }

    private void ComputeMetrics(
        Size2D[,] elements,
        out int[] colWidths,
        out int[] rowHeights,
        out int[] colOffsets,
        out int[] rowOffsets,
        out int   totalWidth,
        out int   totalHeight,
        out bool  hasHeader,
        out bool  hasLeftBorder,
        out bool  hasRightBorder,
        out bool  hasTopBorder,
        out bool  hasBottomBorder,
        out bool  hasColSep,
        out bool  hasRowSep,
        out bool  hasHeaderSep
    ) {
        var numRows = elements.GetLength(0);
        var numCols = elements.GetLength(1);

        colWidths = new int[numCols];

        for (var c = 0; c < numCols; c++) {
            var maxW = 0;

            for (var r = 0; r < numRows; r++)
                if (elements[r, c].X > maxW)
                    maxW = elements[r, c].X;

            colWidths[c] = maxW;
        }

        rowHeights = new int[numRows];

        for (var r = 0; r < numRows; r++) {
            var maxH = 0;

            for (var c = 0; c < numCols; c++)
                if (elements[r, c].Y > maxH)
                    maxH = elements[r, c].Y;

            rowHeights[r] = maxH;
        }

        hasHeader = numRows > 0 && (
            (EnabledFeatures & HEADER_ROW)       != 0 ||
            (EnabledFeatures & HEADER)           != 0 ||
            (EnabledFeatures & HEADER_SEPARATOR) != 0
        );

        hasLeftBorder = (EnabledFeatures & BORDER_LEFT) != 0 ||
            (hasHeader && (EnabledFeatures & HEADER_BORDER_LEFT) != 0);

        hasRightBorder = (EnabledFeatures & BORDER_RIGHT) != 0 ||
            (hasHeader && (EnabledFeatures & HEADER_BORDER_RIGHT) != 0);

        hasTopBorder = (EnabledFeatures & BORDER_TOP) != 0 ||
            (hasHeader && (EnabledFeatures & HEADER_BORDER_TOP) != 0);

        hasBottomBorder = (EnabledFeatures & BORDER_BOTTOM) != 0;

        hasColSep = (EnabledFeatures & VERTICAL_ELEMENT_SEPARATOR)   != 0;
        hasRowSep = (EnabledFeatures & HORIZONTAL_ELEMENT_SEPARATOR) != 0;

        hasHeaderSep = hasHeader && (
            (EnabledFeatures & HEADER_SEPARATOR) != 0 ||
            (hasRowSep && numRows > 1)
        );

        colOffsets = new int[numCols];
        var curX = hasLeftBorder ? 1 : 0;

        for (var c = 0; c < numCols; c++) {
            colOffsets[c] =  curX;
            curX          += colWidths[c];

            if (c < numCols - 1 && hasColSep)
                curX += 1;
        }

        totalWidth = curX + (hasRightBorder ? 1 : 0);

        rowOffsets = new int[numRows];
        var curY = hasTopBorder ? 1 : 0;

        for (var r = 0; r < numRows; r++) {
            rowOffsets[r] =  curY;
            curY          += rowHeights[r];

            if (r < numRows - 1) {
                var sepBelow = r == 0 && hasHeader ? hasHeaderSep : hasRowSep;

                if (sepBelow)
                    curY += 1;
            }
        }

        totalHeight = curY + (hasBottomBorder ? 1 : 0);
    }

    #endregion

    #region Factory Methods

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with ASCII-style borders and separators.
    /// </summary>
    public static NKTableStyle GetAscii(NKStyle style = default, Features features = DEFAULT) => new('|', '-', '+', '+', '+', style, features);

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with standard thin-line box-drawing
    ///     characters.
    /// </summary>
    public static NKTableStyle GetNormal(NKStyle style = default, Features features = DEFAULT) => new('│', '─', '┌', '┐', '└', '┘', '├', '┤', '┴', '┬', '┼', style, features);

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with rounded corners and thin-line
    ///     borders.
    /// </summary>
    public static NKTableStyle GetRounded(NKStyle style = default, Features features = DEFAULT) => new('│', '─', '╭', '╮', '╰', '╯', '├', '┤', '┴', '┬', '┼', style, features);

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with thick-line borders.
    /// </summary>
    public static NKTableStyle GetThick(NKStyle style = default, Features features = DEFAULT) => new('┃', '━', '┏', '┓', '┗', '┛', '┣', '┫', '┻', '┳', '╋', style, features);

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with double-line borders.
    /// </summary>
    public static NKTableStyle GetDouble(NKStyle style = default, Features features = DEFAULT) => new('║', '═', '╔', '╗', '╚', '╝', '╠', '╣', '╩', '╦', '╬', style, features);

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with thin-line borders and a prominent
    ///     double-line header separator.
    /// </summary>
    public static NKTableStyle GetDoubleHeader(NKStyle style = default, NKStyle headerStyle = default, Features features = DEFAULT) =>
        new(
            '│',
            '─',
            '┌',
            '┐',
            '└',
            '┘',
            '├',
            '┤',
            '┴',
            '┬',
            '┼',
            '═',
            '╞',
            '╡',
            '┬',
            '╪',
            style,
            headerStyle != default ? headerStyle : style,
            features
        );

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with a solid background color and space
    ///     characters for borders.
    /// </summary>
    public static NKTableStyle GetSolid(NKColor color, Features features = DEFAULT) => new(' ', ' ', ' ', ' ', ' ', new NKStyle(backgroundColor: color), features);

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with no visual borders or separators.
    /// </summary>
    public static NKTableStyle GetBorderless(Features features = NONE) => new(' ', ' ', ' ', ' ', ' ', default, features);

    /// <summary>
    ///     Creates a minimal table style (markdown-like) with only header and column separators.
    /// </summary>
    public static NKTableStyle GetMinimal(NKStyle style = default) =>
        new(
            '│',
            '─',
            '┌',
            '┐',
            '└',
            '┘',
            '├',
            '┤',
            '┴',
            '┬',
            '┼',
            style,
            HEADER_ROW | HEADER_SEPARATOR | VERTICAL_ELEMENT_SEPARATOR
        );

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with an inset 3D appearance using shadow
    ///     and highlight styles.
    /// </summary>
    public static NKTableStyle GetInset(NKStyle shadow = default, NKStyle highlight = default, Features features = DEFAULT) =>
        new(
            new AnsiChar('┌', shadow),
            new AnsiChar('─', shadow),
            new AnsiChar('┐', highlight),
            new AnsiChar('│', highlight),
            new AnsiChar('┘', highlight),
            new AnsiChar('─', highlight),
            new AnsiChar('└', shadow),
            new AnsiChar('│', shadow),
            new AnsiChar('├', shadow),
            new AnsiChar('┤', highlight),
            new AnsiChar('┴', highlight),
            new AnsiChar('┬', shadow),
            new AnsiChar('┼', shadow),
            new AnsiChar('│', shadow),
            new AnsiChar('─', shadow),
            new AnsiChar('┌', shadow),
            new AnsiChar('─', shadow),
            new AnsiChar('┐', highlight),
            new AnsiChar('│', highlight),
            new AnsiChar('─', highlight),
            new AnsiChar('│', shadow),
            new AnsiChar('├', shadow),
            new AnsiChar('┤', highlight),
            new AnsiChar('┬', shadow),
            new AnsiChar('┼', shadow),
            features
        );

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with an inset 3D appearance using
    ///     shadow, highlight, and background colors.
    /// </summary>
    public static NKTableStyle GetInset(NKColor? shadow, NKColor? highlight = null, NKColor? background = null, Features features = DEFAULT) =>
        GetInset(
            new NKStyle(shadow    ?? NKColor.Default, background ?? NKColor.Inherit),
            new NKStyle(highlight ?? NKColor.Default, background ?? NKColor.Inherit),
            features
        );

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with an outset 3D appearance using
    ///     shadow and highlight styles.
    /// </summary>
    public static NKTableStyle GetOutset(NKStyle shadow = default, NKStyle highlight = default, Features features = DEFAULT) =>
        new(
            new AnsiChar('┌', highlight),
            new AnsiChar('─', highlight),
            new AnsiChar('┐', shadow),
            new AnsiChar('│', shadow),
            new AnsiChar('┘', shadow),
            new AnsiChar('─', shadow),
            new AnsiChar('└', highlight),
            new AnsiChar('│', highlight),
            new AnsiChar('├', highlight),
            new AnsiChar('┤', shadow),
            new AnsiChar('┴', shadow),
            new AnsiChar('┬', highlight),
            new AnsiChar('┼', highlight),
            new AnsiChar('│', highlight),
            new AnsiChar('─', highlight),
            new AnsiChar('┌', highlight),
            new AnsiChar('─', highlight),
            new AnsiChar('┐', shadow),
            new AnsiChar('│', shadow),
            new AnsiChar('─', shadow),
            new AnsiChar('│', highlight),
            new AnsiChar('├', highlight),
            new AnsiChar('┤', shadow),
            new AnsiChar('┬', highlight),
            new AnsiChar('┼', highlight),
            features
        );

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with an outset 3D appearance using
    ///     shadow, highlight, and background colors.
    /// </summary>
    public static NKTableStyle GetOutset(NKColor? shadow, NKColor? highlight = null, NKColor? background = null, Features features = DEFAULT) =>
        GetOutset(
            new NKStyle(shadow    ?? NKColor.Default, background ?? NKColor.Inherit),
            new NKStyle(highlight ?? NKColor.Default, background ?? NKColor.Inherit),
            features
        );

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with a thick inset 3D appearance using
    ///     shadow and highlight styles.
    /// </summary>
    public static NKTableStyle GetInsetThick(NKStyle shadow = default, NKStyle highlight = default, Features features = DEFAULT) =>
        new(
            new AnsiChar('┏', shadow),
            new AnsiChar('━', shadow),
            new AnsiChar('┓', highlight),
            new AnsiChar('┃', highlight),
            new AnsiChar('┛', highlight),
            new AnsiChar('━', highlight),
            new AnsiChar('┗', shadow),
            new AnsiChar('┃', shadow),
            new AnsiChar('┣', shadow),
            new AnsiChar('┫', highlight),
            new AnsiChar('┻', highlight),
            new AnsiChar('┳', shadow),
            new AnsiChar('╋', shadow),
            new AnsiChar('┃', shadow),
            new AnsiChar('━', shadow),
            new AnsiChar('┏', shadow),
            new AnsiChar('━', shadow),
            new AnsiChar('┓', highlight),
            new AnsiChar('┃', highlight),
            new AnsiChar('━', highlight),
            new AnsiChar('┃', shadow),
            new AnsiChar('┣', shadow),
            new AnsiChar('┫', highlight),
            new AnsiChar('┳', shadow),
            new AnsiChar('╋', shadow),
            features
        );

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with a thick inset 3D appearance using
    ///     shadow, highlight, and background colors.
    /// </summary>
    public static NKTableStyle GetInsetThick(NKColor? shadow, NKColor? highlight = null, NKColor? background = null, Features features = DEFAULT) =>
        GetInsetThick(
            new NKStyle(shadow    ?? NKColor.Default, background ?? NKColor.Inherit),
            new NKStyle(highlight ?? NKColor.Default, background ?? NKColor.Inherit),
            features
        );

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with a thick outset 3D appearance using
    ///     shadow and highlight styles.
    /// </summary>
    public static NKTableStyle GetOutsetThick(NKStyle shadow = default, NKStyle highlight = default, Features features = DEFAULT) =>
        new(
            new AnsiChar('┏', highlight),
            new AnsiChar('━', highlight),
            new AnsiChar('┓', shadow),
            new AnsiChar('┃', shadow),
            new AnsiChar('┛', shadow),
            new AnsiChar('━', shadow),
            new AnsiChar('┗', highlight),
            new AnsiChar('┃', highlight),
            new AnsiChar('┣', highlight),
            new AnsiChar('┫', shadow),
            new AnsiChar('┻', shadow),
            new AnsiChar('┳', highlight),
            new AnsiChar('╋', highlight),
            new AnsiChar('┃', highlight),
            new AnsiChar('━', highlight),
            new AnsiChar('┏', highlight),
            new AnsiChar('━', highlight),
            new AnsiChar('┓', shadow),
            new AnsiChar('┃', shadow),
            new AnsiChar('━', shadow),
            new AnsiChar('┃', highlight),
            new AnsiChar('┣', highlight),
            new AnsiChar('┫', shadow),
            new AnsiChar('┳', highlight),
            new AnsiChar('╋', highlight),
            features
        );

    /// <summary>
    ///     Creates a <see cref="NKTableStyle" /> instance with a thick outset 3D appearance using
    ///     shadow, highlight, and background colors.
    /// </summary>
    public static NKTableStyle GetOutsetThick(NKColor? shadow, NKColor? highlight = null, NKColor? background = null, Features features = DEFAULT) =>
        GetOutsetThick(
            new NKStyle(shadow    ?? NKColor.Default, background ?? NKColor.Inherit),
            new NKStyle(highlight ?? NKColor.Default, background ?? NKColor.Inherit),
            features
        );

    public static NKTableStyle Ascii        => GetAscii();
    public static NKTableStyle Normal       => GetNormal();
    public static NKTableStyle Rounded      => GetRounded();
    public static NKTableStyle Thick        => GetThick();
    public static NKTableStyle Double       => GetDouble();
    public static NKTableStyle DoubleHeader => GetDoubleHeader();
    public static NKTableStyle Borderless   => GetBorderless();
    public static NKTableStyle Minimal      => GetMinimal();

    #endregion

    [Flags]
    public enum Features {
        NONE = 0,

        BORDER_LEFT       = 1,
        BORDER_RIGHT      = 2,
        BORDER_TOP        = 4,
        BORDER_BOTTOM     = 8,
        BORDER_HORIZONTAL = BORDER_TOP  | BORDER_BOTTOM,
        BORDER_VERTICAL   = BORDER_LEFT | BORDER_RIGHT,
        BORDER            = BORDER_LEFT | BORDER_RIGHT | BORDER_TOP | BORDER_BOTTOM,

        VERTICAL_ELEMENT_SEPARATOR   = 16,
        HORIZONTAL_ELEMENT_SEPARATOR = 32,
        ELEMENT_SEPARATORS           = VERTICAL_ELEMENT_SEPARATOR | HORIZONTAL_ELEMENT_SEPARATOR,

        HEADER_ROW                = 64,
        HEADER_SEPARATOR          = 128,
        HEADER_VERTICAL_SEPARATOR = 256,
        HEADER_BORDER_TOP         = 512,
        HEADER_BORDER_BOTTOM      = HEADER_SEPARATOR,
        HEADER_BORDER_LEFT        = 1024,
        HEADER_BORDER_RIGHT       = 2048,
        HEADER_BORDER             = HEADER_BORDER_TOP | HEADER_BORDER_BOTTOM | HEADER_BORDER_LEFT | HEADER_BORDER_RIGHT,
        HEADER                    = HEADER_ROW        | HEADER_SEPARATOR     | HEADER_VERTICAL_SEPARATOR,

        OUTLINE = BORDER,
        ALL     = BORDER | ELEMENT_SEPARATORS | HEADER,
        DEFAULT = ALL
    }
}