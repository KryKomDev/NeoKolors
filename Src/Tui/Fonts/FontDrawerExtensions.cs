// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Fonts;

public static class FontDrawerExtensions {
    
    extension(ICharDrawer drawer) {
        
        public void DrawString(string str, IAsciiFont font, Point2D position = default, int z = 0) =>
            font.DrawString(str, drawer, position, z);

        public void DrawString(string str, IAsciiFont font, int maxWidth, Point2D position = default, int z = 0) =>
            font.DrawString(str, drawer, maxWidth, position, z);

        public void DrawString(
            string          str,
            IAsciiFont      font,
            Rect2D          bounds,
            NKStyle         style,
            HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
            VerticalAlign   verticalAlign   = VerticalAlign.TOP,
            bool            overflow        = false,
            int             z               = 0
        ) => font.DrawString(str, drawer, bounds, style, horizontalAlign, verticalAlign, overflow, z);

        public void DrawString(AnsiString str, IAsciiFont font, Point2D position = default, int z = 0) =>
            font.DrawString(str, drawer, position, z);

        public void DrawString(AnsiString str, IAsciiFont font, int maxWidth, Point2D position = default, int z = 0) =>
            font.DrawString(str, drawer, maxWidth, position, z);

        public void DrawString(
            AnsiString      str,
            IAsciiFont      font,
            Rect2D          bounds,
            HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
            VerticalAlign   verticalAlign   = VerticalAlign.TOP,
            bool            overflow        = false,
            int             z               = 0
        ) => font.DrawString(str, drawer, bounds, horizontalAlign, verticalAlign, overflow, z);

        public void DrawString(string str, IAsciiFont font, Rect2D bounds, TextRenderingOptions options, int z = 0) =>
            font.DrawString(str, drawer, bounds, options, z);

        public void DrawString(AnsiString str, IAsciiFont font, Rect2D bounds, TextRenderingOptions options, int z = 0) =>
            font.DrawString(str, drawer, bounds, options, z);

        public void DrawString(string str, IAsciiFont font, TextRenderingOptions options, int z = 0) =>
            font.DrawString(str, drawer, options, z);

        public void DrawString(AnsiString str, IAsciiFont font, TextRenderingOptions options, int z = 0) =>
            font.DrawString(str, drawer, options, z);

        public void DrawText(string str, IAsciiFont font, Point2D position = default, int z = 0) =>
            font.DrawString(str, drawer, position, z);

        public void DrawText(string str, IAsciiFont font, int maxWidth, Point2D position = default, int z = 0) =>
            font.DrawString(str, drawer, maxWidth, position, z);

        public void DrawText(
            string          str,
            IAsciiFont      font,
            Rect2D          bounds,
            NKStyle         style,
            HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
            VerticalAlign   verticalAlign   = VerticalAlign.TOP,
            bool            overflow        = false,
            int             z               = 0
        ) => font.DrawString(str, drawer, bounds, style, horizontalAlign, verticalAlign, overflow, z);

        public void DrawText(AnsiString str, IAsciiFont font, Point2D position = default, int z = 0) =>
            font.DrawString(str, drawer, position, z);

        public void DrawText(AnsiString str, IAsciiFont font, int maxWidth, Point2D position = default, int z = 0) =>
            font.DrawString(str, drawer, maxWidth, position, z);

        public void DrawText(
            AnsiString      str,
            IAsciiFont      font,
            Rect2D          bounds,
            HorizontalAlign horizontalAlign = HorizontalAlign.LEFT,
            VerticalAlign   verticalAlign   = VerticalAlign.TOP,
            bool            overflow        = false,
            int             z               = 0
        ) => font.DrawString(str, drawer, bounds, horizontalAlign, verticalAlign, overflow, z);

        public void DrawText(string str, IAsciiFont font, Rect2D bounds, TextRenderingOptions options, int z = 0) =>
            font.DrawString(str, drawer, bounds, options, z);

        public void DrawText(AnsiString str, IAsciiFont font, Rect2D bounds, TextRenderingOptions options, int z = 0) =>
            font.DrawString(str, drawer, bounds, options, z);

        public void DrawText(string str, IAsciiFont font, TextRenderingOptions options, int z = 0) =>
            font.DrawString(str, drawer, options, z);

        public void DrawText(AnsiString str, IAsciiFont font, TextRenderingOptions options, int z = 0) =>
            font.DrawString(str, drawer, options, z);
    }
}
