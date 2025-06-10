
#if NET5_0_OR_GREATER
using System.ComponentModel.DataAnnotations;
#endif
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using NeoKolors.Common;

namespace NeoKolors.Tui.Styles;

[StructLayout(LayoutKind.Sequential, Size = sizeof(char) * 26 + sizeof(ulong) * 26)]
public struct TableBorderStyle {
    
    public char InnerVertical { get; set; }
    public char InnerHorizontal { get; set; }
    public char OuterVertical { get; set; }
    public char OuterHorizontal { get; set; }
    public char TopRight { get; set; }
    public char TopLeft { get; set; }
    public char BottomRight { get; set; }
    public char BottomLeft { get; set; }
    public char Cross { get; set; }
    public char VerticalLeftT { get; set; }
    public char VerticalRightT { get; set; }
    public char HorizontalTopT { get; set; }
    public char HorizontalBottomT { get; set; }
    
    public char HeaderInnerVertical { get; set; }
    public char HeaderInnerHorizontal { get; set; }
    public char HeaderOuterVertical { get; set; }
    public char HeaderOuterHorizontal { get; set; }
    public char HeaderTopRight { get; set; }
    public char HeaderTopLeft { get; set; }
    public char HeaderBottomRight { get; set; }
    public char HeaderBottomLeft { get; set; }
    public char HeaderCross { get; set; }
    public char HeaderVerticalLeftT { get; set; }
    public char HeaderVerticalRightT { get; set; }
    public char HeaderHorizontalTopT { get; set; }
    public char HeaderHorizontalBottomT { get; set; }
    
    public NKStyle InnerVerticalStyle { get; set; }
    public NKStyle InnerHorizontalStyle { get; set; }
    public NKStyle OuterVerticalStyle { get; set; }
    public NKStyle OuterHorizontalStyle { get; set; }
    public NKStyle TopRightStyle { get; set; }
    public NKStyle TopLeftStyle { get; set; }
    public NKStyle BottomRightStyle { get; set; }
    public NKStyle BottomLeftStyle { get; set; }
    public NKStyle CrossStyle { get; set; }
    public NKStyle VerticalLeftTStyle { get; set; }
    public NKStyle VerticalRightTStyle { get; set; }
    public NKStyle HorizontalTopTStyle { get; set; }
    public NKStyle HorizontalBottomTStyle { get; set; }
    
    public NKStyle HeaderInnerVerticalStyle { get; set; }
    public NKStyle HeaderInnerHorizontalStyle { get; set; }
    public NKStyle HeaderOuterVerticalStyle { get; set; }
    public NKStyle HeaderOuterHorizontalStyle { get; set; }
    public NKStyle HeaderTopRightStyle { get; set; }
    public NKStyle HeaderTopLeftStyle { get; set; }
    public NKStyle HeaderBottomRightStyle { get; set; }
    public NKStyle HeaderBottomLeftStyle { get; set; }
    public NKStyle HeaderCrossStyle { get; set; }
    public NKStyle HeaderVerticalLeftTStyle { get; set; }
    public NKStyle HeaderVerticalRightTStyle { get; set; }
    public NKStyle HeaderHorizontalTopTStyle { get; set; }
    public NKStyle HeaderHorizontalBottomTStyle { get; set; }

    public TableBorderStyle(
        char innerVertical,
        char innerHorizontal,
        char outerVertical,
        char outerHorizontal,
        char topRight,
        char topLeft,
        char bottomRight,
        char bottomLeft,
        char cross,
        char verticalLeftT,
        char verticalRightT,
        char horizontalTopT,
        char horizontalBottomT,
        char? headerInnerVertical = null,
        char? headerInnerHorizontal = null,
        char? headerOuterVertical = null,
        char? headerOuterHorizontal = null,
        char? headerTopRight = null,
        char? headerTopLeft = null,
        char? headerBottomRight = null,
        char? headerBottomLeft = null,
        char? headerCross = null,
        char? headerVerticalLeftT = null,
        char? headerVerticalRightT = null,
        char? headerHorizontalTopT = null,
        char? headerHorizontalBottomT = null,
        NKStyle? innerVerticalStyle = null,
        NKStyle? innerHorizontalStyle = null,
        NKStyle? outerVerticalStyle = null,
        NKStyle? outerHorizontalStyle = null,
        NKStyle? topRightStyle = null,
        NKStyle? topLeftStyle = null,
        NKStyle? bottomRightStyle = null,
        NKStyle? bottomLeftStyle = null,
        NKStyle? crossStyle = null,
        NKStyle? verticalLeftTStyle = null,
        NKStyle? verticalRightTStyle = null,
        NKStyle? horizontalTopTStyle = null,
        NKStyle? horizontalBottomTStyle = null,
        NKStyle? headerInnerVerticalStyle = null,
        NKStyle? headerInnerHorizontalStyle = null,
        NKStyle? headerOuterVerticalStyle = null,
        NKStyle? headerOuterHorizontalStyle = null,
        NKStyle? headerTopRightStyle = null,
        NKStyle? headerTopLeftStyle = null,
        NKStyle? headerBottomRightStyle = null,
        NKStyle? headerBottomLeftStyle = null,
        NKStyle? headerCrossStyle = null,
        NKStyle? headerVerticalLeftTStyle = null,
        NKStyle? headerVerticalRightTStyle = null,
        NKStyle? headerHorizontalTopTStyle = null,
        NKStyle? headerHorizontalBottomTStyle = null)
    {
        InnerVertical = innerVertical;
        InnerHorizontal = innerHorizontal;
        OuterVertical = outerVertical;
        OuterHorizontal = outerHorizontal;
        TopRight = topRight;
        TopLeft = topLeft;
        BottomRight = bottomRight;
        BottomLeft = bottomLeft;
        Cross = cross;
        VerticalLeftT = verticalLeftT;
        VerticalRightT = verticalRightT;
        HorizontalTopT = horizontalTopT;
        HorizontalBottomT = horizontalBottomT;
        HeaderInnerVertical = headerInnerVertical ?? InnerVertical;
        HeaderInnerHorizontal = headerInnerHorizontal ?? InnerHorizontal;
        HeaderOuterVertical = headerOuterVertical ?? OuterVertical;
        HeaderOuterHorizontal = headerOuterHorizontal ?? OuterHorizontal;
        HeaderTopRight = headerTopRight ?? TopRight;
        HeaderTopLeft = headerTopLeft ?? TopLeft;
        HeaderBottomRight = headerBottomRight ?? BottomRight;
        HeaderBottomLeft = headerBottomLeft ?? BottomLeft;
        HeaderCross = headerCross ?? Cross;
        HeaderVerticalLeftT = headerVerticalLeftT ?? VerticalLeftT;
        HeaderVerticalRightT = headerVerticalRightT ?? VerticalRightT;
        HeaderHorizontalTopT = headerHorizontalTopT ?? HorizontalTopT;
        HeaderHorizontalBottomT = headerHorizontalBottomT ?? HorizontalBottomT;
        InnerVerticalStyle = innerVerticalStyle ?? NKStyle.Default;
        InnerHorizontalStyle = innerHorizontalStyle ?? NKStyle.Default;
        OuterVerticalStyle = outerVerticalStyle ?? NKStyle.Default;
        OuterHorizontalStyle = outerHorizontalStyle ?? NKStyle.Default;
        TopRightStyle = topRightStyle ?? NKStyle.Default;
        TopLeftStyle = topLeftStyle ?? NKStyle.Default;
        BottomRightStyle = bottomRightStyle ?? NKStyle.Default;
        BottomLeftStyle = bottomLeftStyle ?? NKStyle.Default;
        CrossStyle = crossStyle ?? NKStyle.Default;
        VerticalLeftTStyle = verticalLeftTStyle ?? NKStyle.Default;
        VerticalRightTStyle = verticalRightTStyle ?? NKStyle.Default;
        HorizontalTopTStyle = horizontalTopTStyle ?? NKStyle.Default;
        HorizontalBottomTStyle = horizontalBottomTStyle ?? NKStyle.Default;
        HeaderInnerVerticalStyle = headerInnerVerticalStyle ?? InnerVerticalStyle;
        HeaderInnerHorizontalStyle = headerInnerHorizontalStyle ?? InnerHorizontalStyle;
        HeaderOuterVerticalStyle = headerOuterVerticalStyle ?? OuterVerticalStyle;
        HeaderOuterHorizontalStyle = headerOuterHorizontalStyle ?? OuterHorizontalStyle;
        HeaderTopRightStyle = headerTopRightStyle ?? TopRightStyle;
        HeaderTopLeftStyle = headerTopLeftStyle ?? TopLeftStyle;
        HeaderBottomRightStyle = headerBottomRightStyle ?? BottomRightStyle;
        HeaderBottomLeftStyle = headerBottomLeftStyle ?? BottomLeftStyle;
        HeaderCrossStyle = headerCrossStyle ?? CrossStyle;
        HeaderVerticalLeftTStyle = headerVerticalLeftTStyle ?? VerticalLeftTStyle;
        HeaderVerticalRightTStyle = headerVerticalRightTStyle ?? VerticalRightTStyle;
        HeaderHorizontalTopTStyle = headerHorizontalTopTStyle ?? HorizontalTopTStyle;
        HeaderHorizontalBottomTStyle = headerHorizontalBottomTStyle ?? HorizontalBottomTStyle;
    }
    
    public TableBorderStyle(
        char innerVertical,
        char innerHorizontal,
        char outerVertical,
        char outerHorizontal,
        char topRight,
        char topLeft,
        char bottomRight,
        char bottomLeft,
        char cross,
        char verticalLeftT,
        char verticalRightT,
        char horizontalTopT,
        char horizontalBottomT,
        char? headerInnerVertical = null,
        char? headerInnerHorizontal = null,
        char? headerOuterVertical = null,
        char? headerOuterHorizontal = null,
        char? headerTopRight = null,
        char? headerTopLeft = null,
        char? headerBottomRight = null,
        char? headerBottomLeft = null,
        char? headerCross = null,
        char? headerVerticalLeftT = null,
        char? headerVerticalRightT = null,
        char? headerHorizontalTopT = null,
        char? headerHorizontalBottomT = null,
        NKColor? text = null,
        NKColor? background = null) 
    {
        InnerVertical = innerVertical;
        InnerHorizontal = innerHorizontal;
        OuterVertical = outerVertical;
        OuterHorizontal = outerHorizontal;
        TopRight = topRight;
        TopLeft = topLeft;
        BottomRight = bottomRight;
        BottomLeft = bottomLeft;
        Cross = cross;
        VerticalLeftT = verticalLeftT;
        VerticalRightT = verticalRightT;
        HorizontalTopT = horizontalTopT;
        HorizontalBottomT = horizontalBottomT;
        HeaderInnerVertical = headerInnerVertical ?? InnerVertical;
        HeaderInnerHorizontal = headerInnerHorizontal ?? InnerHorizontal;
        HeaderOuterVertical = headerOuterVertical ?? OuterVertical;
        HeaderOuterHorizontal = headerOuterHorizontal ?? OuterHorizontal;
        HeaderTopRight = headerTopRight ?? TopRight;
        HeaderTopLeft = headerTopLeft ?? TopLeft;
        HeaderBottomRight = headerBottomRight ?? BottomRight;
        HeaderBottomLeft = headerBottomLeft ?? BottomLeft;
        HeaderCross = headerCross ?? Cross;
        HeaderVerticalLeftT = headerVerticalLeftT ?? VerticalLeftT;
        HeaderVerticalRightT = headerVerticalRightT ?? VerticalRightT;
        HeaderHorizontalTopT = headerHorizontalTopT ?? HorizontalTopT;
        HeaderHorizontalBottomT = headerHorizontalBottomT ?? HorizontalBottomT;
        var s = new NKStyle(text ?? NKColor.Default, background ?? NKColor.Inherit, TextStyles.NONE);
        InnerVerticalStyle = s;
        InnerHorizontalStyle = s;
        OuterVerticalStyle = s;
        OuterHorizontalStyle = s;
        TopRightStyle = s;
        TopLeftStyle = s;
        BottomRightStyle = s;
        BottomLeftStyle = s;
        CrossStyle = s;
        VerticalLeftTStyle = s;
        VerticalRightTStyle = s;
        HorizontalTopTStyle = s;
        HorizontalBottomTStyle = s;
        HeaderInnerVerticalStyle = s;
        HeaderInnerHorizontalStyle = s;
        HeaderOuterVerticalStyle = s;
        HeaderOuterHorizontalStyle = s;
        HeaderTopRightStyle = s;
        HeaderTopLeftStyle = s;
        HeaderBottomRightStyle = s;
        HeaderBottomLeftStyle = s;
        HeaderCrossStyle = s;
        HeaderVerticalLeftTStyle = s;
        HeaderVerticalRightTStyle = s;
        HeaderHorizontalTopTStyle = s;
        HeaderHorizontalBottomTStyle = s;
    }
    
    private const string STR_LEN_ERR = "TableBorderStyle chars string must be long 26 chars.";
    
    public TableBorderStyle(
        #if NET5_0_OR_GREATER
        [StringLength(maximumLength: 26, MinimumLength = 26, ErrorMessage = STR_LEN_ERR)] 
        #endif
        string chars,
        NKStyle? normal = null,
        NKStyle? header = null) 
    {
        if (chars.Length != 26) 
            throw new ArgumentException(STR_LEN_ERR, nameof(chars)); 
        InnerVertical = chars[0];
        InnerHorizontal = chars[1];
        OuterVertical = chars[2];
        OuterHorizontal = chars[3];
        TopRight = chars[4];
        TopLeft = chars[5];
        BottomRight = chars[6];
        BottomLeft = chars[7];
        Cross = chars[8];
        VerticalLeftT = chars[9];
        VerticalRightT = chars[10];
        HorizontalTopT = chars[11];
        HorizontalBottomT = chars[12];
        HeaderInnerVertical = chars[13];
        HeaderInnerHorizontal = chars[14];
        HeaderOuterVertical = chars[15];
        HeaderOuterHorizontal = chars[16];
        HeaderTopRight = chars[17];
        HeaderTopLeft = chars[18];
        HeaderBottomRight = chars[19];
        HeaderBottomLeft = chars[20];
        HeaderCross = chars[21];
        HeaderVerticalLeftT = chars[22];
        HeaderVerticalRightT = chars[23];
        HeaderHorizontalTopT = chars[24];
        HeaderHorizontalBottomT = chars[25];
        normal ??= NKStyle.Default;
        InnerVerticalStyle = normal.Value;
        InnerHorizontalStyle = normal.Value;
        OuterVerticalStyle = normal.Value;
        OuterHorizontalStyle = normal.Value;
        TopRightStyle = normal.Value;
        TopLeftStyle = normal.Value;
        BottomRightStyle = normal.Value;
        BottomLeftStyle = normal.Value;
        CrossStyle = normal.Value;
        VerticalLeftTStyle = normal.Value;
        VerticalRightTStyle = normal.Value;
        HorizontalTopTStyle = normal.Value;
        HorizontalBottomTStyle = normal.Value;
        header ??= NKStyle.Default;
        HeaderInnerVerticalStyle = header.Value;
        HeaderInnerHorizontalStyle = header.Value;
        HeaderOuterVerticalStyle = header.Value;
        HeaderOuterHorizontalStyle = header.Value;
        HeaderTopRightStyle = header.Value;
        HeaderTopLeftStyle = header.Value;
        HeaderBottomRightStyle = header.Value;
        HeaderBottomLeftStyle = header.Value;
        HeaderCrossStyle = header.Value;
        HeaderVerticalLeftTStyle = header.Value;
        HeaderVerticalRightTStyle = header.Value;
        HeaderHorizontalTopTStyle = header.Value;
        HeaderHorizontalBottomTStyle = header.Value;
    }

    [Pure]
    public static TableBorderStyle GetAscii(NKStyle? normal = null, NKStyle? header = null) =>
        new("|-|-+++++++++|-|-+++++++++", normal, header);
    
    [Pure]
    public static TableBorderStyle GetAsciiHeader(NKStyle? normal = null, NKStyle? header = null) =>
        new("|-|-+++++++++|=|-+++++++++", normal, header);
    
    [Pure]
    public static TableBorderStyle GetNormal(NKStyle? normal = null, NKStyle? header = null) =>
        new("│─│─┐┌┘└┼├┤┬┴│─│─┐┌┘└┼├┤┬┴", normal, header);
    
    [Pure]
    public static TableBorderStyle GetNormalHeaderDouble(NKStyle? normal = null, NKStyle? header = null) =>
        new("│─│─┐┌┘└┼├┤┬┴║═║═╗╔╝╚╬╠╣╦╩", normal, header);
    
    [Pure]
    public static TableBorderStyle GetNormalHeaderThick(NKStyle? normal = null, NKStyle? header = null) =>
        new("│─│─┐┌┘└┼├┤┬┴┃━┃━┓┏┛┗╋┡┩┳┻", normal, header);
    
    [Pure]
    public static TableBorderStyle GetRounded(NKStyle? normal = null, NKStyle? header = null) =>
        new("│─│─╮╭╯╰┼├┤┬┴│─│─╮╭╯╰┼├┤┬┴", normal, header);
    
    [Pure]
    public static TableBorderStyle GetRoundedHeaderDouble(NKStyle? normal = null, NKStyle? header = null) =>
        new("│─│─╮╭╯╰┼├┤┬┴║═║═╗╔╝╚╬╠╣╦╩", normal, header);
    
    [Pure]
    public static TableBorderStyle GetRoundedHeaderThick(NKStyle? normal = null, NKStyle? header = null) =>
        new("│─│─╮╭╯╰┼├┤┬┴┃━┃━┓┏┛┗╋┡┩┳┻", normal, header);
    
    [Pure]
    public static TableBorderStyle GetDouble(NKStyle? normal = null, NKStyle? header = null) =>
        new("║═║═╗╔╝╚╬╠╣╦╩║═║═╗╔╝╚╬╠╣╦╩", normal, header);
    
    [Pure]
    public static TableBorderStyle GetThick(NKStyle? normal = null, NKStyle? header = null) =>
        new("┃━┃━┓┏┛┗╋┣┫┳┻┃━┃━┓┏┛┗╋┣┫┳┻", normal, header);
}