// NeoKolors
// Copyright (c) 2025 KryKom

using Metriks;
using NeoKolors.Tui.Styles.Values;

namespace NeoKolors.Tui;

/// <summary>
/// Provides extension methods for manipulating and interacting with ICharCanvas objects
/// in the NeoKolors.Tui namespace.
/// </summary>
public static class CharCanvasExtensions {
    
    extension(ICharCanvas canvas) {
        
        public void PlaceRectangle(Rectangle rectangle, BorderStyle borderStyle) {
            if (borderStyle.IsBorderless)
                return;
            
            canvas.PlaceRectangle(
                rectangle,
                borderStyle.Vertical,        borderStyle.Horizontal,
                borderStyle.TopRight,        borderStyle.TopLeft,
                borderStyle.BottomRight,     borderStyle.BottomLeft,
                borderStyle.StyleTopLeft,    borderStyle.StyleTopRight, 
                borderStyle.StyleBottomLeft, borderStyle.StyleBottomRight,
                borderStyle.StyleTopLeft,    borderStyle.StyleTopRight, 
                borderStyle.StyleBottomLeft, borderStyle.StyleBottomRight
            );
        }

        public void PlaceRectangle(
            Rectangle rectangle, 
            char vertical,    char horizontal, 
            char topRight,    char topLeft,
            char bottomRight, char bottomLeft,
            NKStyle topStyle,     NKStyle bottomStyle,   NKStyle leftStyle,       NKStyle rightStyle,
            NKStyle topLeftStyle, NKStyle topRightStyle, NKStyle bottomLeftStyle, NKStyle bottomRightStyle) 
        {
            var lx = rectangle.LowerX;
            var ly = rectangle.LowerY;
            var hx = rectangle.HigherX;
            var hy = rectangle.HigherY;
            
            // top left corner
            if (lx == ..^canvas.Width && ly == ..^canvas.Height) {
                var cellInfo     = canvas[lx, ly];
                cellInfo.Char    = topLeft;
                cellInfo.Changed = true;
                cellInfo.Style   = cellInfo.Style.Override(topLeftStyle);
            }

            // bottom left corner
            if (lx == ..^canvas.Width && hy == ..^canvas.Height) {
                var cellInfo     = canvas[lx, hy];
                cellInfo.Char    = bottomLeft;
                cellInfo.Changed = true;
                cellInfo.Style   = cellInfo.Style.Override(bottomLeftStyle);
            }

            // top right corner
            if (hx == ..^canvas.Width && ly == ..^canvas.Height) {
                var cellInfo     = canvas[hx, ly];
                cellInfo.Char    = topRight;
                cellInfo.Changed = true;
                cellInfo.Style   = cellInfo.Style.Override(topRightStyle);
            }

            // bottom right corner
            if (hx == ..^canvas.Width && hy == ..^canvas.Height) {
                var cellInfo     = canvas[hx, hy];
                cellInfo.Char    = bottomRight;
                cellInfo.Changed = true;
                cellInfo.Style   = cellInfo.Style.Override(bottomRightStyle);
            }

            // top
            if (ly == ..^canvas.Height) {
                for (int x = Math.Clamp(lx + 1, 0, canvas.Width); x < Math.Clamp(hx, 0, canvas.Width); x++) {
                    var c = canvas[x, ly];
                    
                    c.Char    = horizontal;
                    c.Changed = true;
                    c.Style   = c.Style.Override(topStyle);
                }
            }

            // bottom
            if (hy == ..^canvas.Height) {
                for (int x = Math.Clamp(lx + 1, 0, canvas.Width); x < Math.Clamp(hx, 0, canvas.Width); x++) {
                    var c = canvas[x, hy];
                    
                    c.Char    = horizontal;
                    c.Changed = true;
                    c.Style   = c.Style.Override(bottomStyle);
                }
            }

            // left
            if (lx == ..^canvas.Width) {
                for (int y = Math.Clamp(ly + 1, 0, canvas.Height); y < Math.Clamp(hy, 0, canvas.Height); y++) {
                    var c = canvas[lx, y];
                
                    c.Char    = vertical;
                    c.Changed = true;
                    c.Style   = c.Style.Override(leftStyle);
                }
            }

            // right
            if (hx == ..^canvas.Width) {
                for (int y = Math.Clamp(ly + 1, 0, canvas.Height); y < Math.Clamp(hy, 0, canvas.Height); y++) {
                    var c = canvas[hx, y];
                
                    c.Char    = vertical;
                    c.Changed = true;
                    c.Style   = c.Style.Override(rightStyle);
                }
            }
        }

        /// <summary>
        /// Applies the specified style to all characters within the defined rectangular region on the canvas.
        /// </summary>
        /// <param name="region">The rectangular area on the canvas where the style will be applied.</param>
        /// <param name="style">The style to be applied to each character within the region.</param>
        public void Style(Rectangle region, NKStyle style) {
            for (int x = Math.Max(0, region.LowerX); x < Math.Min(canvas.Width, region.HigherX + 1); x++) {
                for (int y = Math.Max(0, region.LowerY); y < Math.Min(canvas.Height, region.HigherY + 1); y++) {
                    var cellInfo     = canvas[x, y];
                    cellInfo.Style   = cellInfo.Style.Override(style);
                    cellInfo.Changed = true;
                }
            }
        }

        /// <summary>
        /// Sets the background color for all characters within the specified rectangular region on the canvas.
        /// </summary>
        /// <param name="region">The rectangular region within which the background color will be applied.</param>
        /// <param name="color">The background color to apply to each character in the region.</param>
        public void StyleBackground(Rectangle region, NKColor color) {
            for (int x = Math.Max(0, region.LowerX); x < Math.Min(canvas.Width, region.HigherX + 1); x++) {
                for (int y = Math.Max(0, region.LowerY); y < Math.Min(canvas.Height, region.HigherY + 1); y++) {
                    var cellInfo     = canvas[x, y];
                    cellInfo.Style   = cellInfo.Style.SafeSetBColor(color);
                    cellInfo.Changed = true;
                }
            }
        }

        /// <summary>
        /// Forces the specified style to all characters within the defined rectangular region on the canvas,
        /// overwriting any existing styles.
        /// </summary>
        /// <param name="region">The area on the canvas where the style will be forcefully applied.</param>
        /// <param name="style">The style to assign to each character within the specified region.</param>
        public void ForceStyle(Rectangle region, NKStyle style) {
            for (int x = Math.Max(0, region.LowerX); x < Math.Min(canvas.Width, region.HigherX + 1); x++) {
                for (int y = Math.Max(0, region.LowerY); y < Math.Min(canvas.Height, region.HigherY + 1); y++) {
                    var cellInfo = canvas[x, y];
                    cellInfo.Style = style;
                    cellInfo.Changed = true;
                }
            }
        }

        /// <summary>
        /// Applies the specified background color to all characters within the given rectangular region on the canvas.
        /// Unlike <c>StyleBackground</c>, this method forces the background color to be applied regardless of any existing styling.
        /// </summary>
        /// <param name="region">The rectangular region on the canvas where the background color will be applied.</param>
        /// <param name="color">The background color to be applied to each character within the region.</param>
        public void ForceStyleBackground(Rectangle region, NKColor color) {
            for (int x = Math.Max(0, region.LowerX); x < Math.Min(canvas.Width, region.HigherX + 1); x++) {
                for (int y = Math.Max(0, region.LowerY); y < Math.Min(canvas.Height, region.HigherY + 1); y++) {
                    var cellInfo = canvas[x, y];
                    var s = cellInfo.Style;
                    cellInfo.Style = s with { BColor = color };
                    cellInfo.Changed = true;
                }
            }
        }

        public void Style<T>(NKStyle style, Point offset, T?[,] mask) {
            for (int x = offset.X; x < Math.Min(mask.Len0 + offset.X, canvas.Width + 1); x++) {
                for (int y = offset.Y; y < Math.Min(mask.Len1 + offset.Y, canvas.Height + 1); y++) {
                    if (mask[x - offset.X, y - offset.Y] == null) continue;
                    
                    var cellInfo = canvas[x, y];
                    cellInfo.Style = cellInfo.Style.Override(style);
                    cellInfo.Changed = true;
                }
            }
        }

        public void PlaceString(string s, Point offset, int window, HorizontalAlign align) {
            if (offset.Y != ..^canvas.Height) return;

            var xo = offset.X + align switch {
                HorizontalAlign.LEFT => 0,
                HorizontalAlign.CENTER => (window - s.Length) / 2,
                HorizontalAlign.RIGHT => window - s.Length,
                _ => 0
            };
                
            for (int i = 0; i < s.Length; i++) {
                if (i + xo == ..^canvas.Width) canvas[i + xo, offset.Y].Char = s[i];
            }
        }
        
        /// <summary>
        /// Applies the specified style to all characters within the defined rectangular region on the canvas.
        /// </summary>
        /// <param name="region">The rectangular area on the canvas where the style will be applied.</param>
        /// <param name="c">The character to be used when filling the region.</param>
        public void Fill(Rectangle region, char c) {
            for (int x = Math.Max(0, region.LowerX); x < Math.Min(canvas.Width, region.HigherX + 1); x++) {
                for (int y = Math.Max(0, region.LowerY); y < Math.Min(canvas.Height, region.HigherY + 1); y++) {
                    var cellInfo     = canvas[x, y];
                    cellInfo.Char    = c;
                    cellInfo.Changed = true;
                }
            }
        }
        
        /// <summary>
        /// Forces the specified style to all characters within the defined rectangular region on the canvas,
        /// overwriting any existing styles.
        /// </summary>
        /// <param name="region">The area on the canvas where the style will be forcefully applied.</param>
        /// <param name="c">The character to be used when filling the region.</param>
        public void ForceFill(Rectangle region, char c) {
            for (int x = Math.Max(0, region.LowerX); x < Math.Min(canvas.Width, region.HigherX + 1); x++) {
                for (int y = Math.Max(0, region.LowerY); y < Math.Min(canvas.Height, region.HigherY + 1); y++) {
                    var cellInfo = canvas[x, y];
                    cellInfo.Char = c;
                    cellInfo.Changed = true;
                }
            }
        }
    }
}