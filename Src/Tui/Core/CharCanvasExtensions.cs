// NeoKolors
// Copyright (c) 2026 KryKom

using Metriks;
using NeoKolors.Common;

namespace NeoKolors.Tui.Core;

/// <summary>
/// Provides extension methods for manipulating and interacting with ICharCanvas objects
/// in the NeoKolors.Tui namespace.
/// </summary>
public static class CharCanvasExtensions {
    
    extension(ICharCanvas canvas) {
        
        public void PlaceRectangle(Rectangle rectangle, BorderStyle borderStyle, int zIndex = 0) {
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
                borderStyle.StyleBottomLeft, borderStyle.StyleBottomRight,
                zIndex
            );
        }

        public void PlaceRectangle(
            Rectangle rectangle, 
            char vertical,    char horizontal, 
            char topRight,    char topLeft,
            char bottomRight, char bottomLeft,
            NKStyle topStyle,     NKStyle bottomStyle,   NKStyle leftStyle,       NKStyle rightStyle,
            NKStyle topLeftStyle, NKStyle topRightStyle, NKStyle bottomLeftStyle, NKStyle bottomRightStyle,
            int zIndex = 0) 
        {
            var lx = rectangle.LowerX;
            var ly = rectangle.LowerY;
            var hx = rectangle.HigherX;
            var hy = rectangle.HigherY;
            
            // top left corner
            if (lx == ..^canvas.Width && ly == ..^canvas.Height) {
                var cellInfo   = canvas[lx, ly];
                if (cellInfo.ZIndex <= zIndex) {
                    cellInfo.Char  = topLeft;
                    cellInfo.Style = cellInfo.Style.OverrideWith(topLeftStyle);
                    cellInfo.ZIndex = zIndex;
                }
            }

            // bottom left corner
            if (lx == ..^canvas.Width && hy == ..^canvas.Height) {
                var cellInfo   = canvas[lx, hy];
                if (cellInfo.ZIndex <= zIndex) {
                    cellInfo.Char  = bottomLeft;
                    cellInfo.Style = cellInfo.Style.OverrideWith(bottomLeftStyle);
                    cellInfo.ZIndex = zIndex;
                }
            }

            // top right corner
            if (hx == ..^canvas.Width && ly == ..^canvas.Height) {
                var cellInfo   = canvas[hx, ly];
                if (cellInfo.ZIndex <= zIndex) {
                    cellInfo.Char  = topRight;
                    cellInfo.Style = cellInfo.Style.OverrideWith(topRightStyle);
                    cellInfo.ZIndex = zIndex;
                }
            }

            // bottom right corner
            if (hx == ..^canvas.Width && hy == ..^canvas.Height) {
                var cellInfo   = canvas[hx, hy];
                if (cellInfo.ZIndex <= zIndex) {
                    cellInfo.Char  = bottomRight;
                    cellInfo.Style = cellInfo.Style.OverrideWith(bottomRightStyle);
                    cellInfo.ZIndex = zIndex;
                }
            }

            // top
            if (ly == ..^canvas.Height) {
                for (int x = Math.Clamp(lx + 1, 0, canvas.Width); x < Math.Clamp(hx, 0, canvas.Width); x++) {
                    var c = canvas[x, ly];
                    if (c.ZIndex <= zIndex) {
                        c.Char  = horizontal;
                        c.Style = c.Style.OverrideWith(topStyle);
                        c.ZIndex = zIndex;
                    }
                }
            }

            // bottom
            if (hy == ..^canvas.Height) {
                for (int x = Math.Clamp(lx + 1, 0, canvas.Width); x < Math.Clamp(hx, 0, canvas.Width); x++) {
                    var c = canvas[x, hy];
                    if (c.ZIndex <= zIndex) {
                        c.Char  = horizontal;
                        c.Style = c.Style.OverrideWith(bottomStyle);
                        c.ZIndex = zIndex;
                    }
                }
            }

            // left
            if (lx == ..^canvas.Width) {
                for (int y = Math.Clamp(ly + 1, 0, canvas.Height); y < Math.Clamp(hy, 0, canvas.Height); y++) {
                    var c = canvas[lx, y];
                    if (c.ZIndex <= zIndex) {
                        c.Char  = vertical;
                        c.Style = c.Style.OverrideWith(leftStyle);
                        c.ZIndex = zIndex;
                    }
                }
            }

            // right
            if (hx == ..^canvas.Width) {
                for (int y = Math.Clamp(ly + 1, 0, canvas.Height); y < Math.Clamp(hy, 0, canvas.Height); y++) {
                    var c = canvas[hx, y];
                    if (c.ZIndex <= zIndex) {
                        c.Char  = vertical;
                        c.Style = c.Style.OverrideWith(rightStyle);
                        c.ZIndex = zIndex;
                    }
                }
            }
        }

        /// <summary>
        /// Applies the specified style to all characters within the defined rectangular region on the canvas.
        /// </summary>
        /// <param name="region">The rectangular area on the canvas where the style will be applied.</param>
        /// <param name="style">The style to be applied to each character within the region.</param>
        public void Style(Rectangle region, NKStyle style, int zIndex = 0) {
            for (int x = Math.Max(0, region.LowerX); x < Math.Min(canvas.Width, region.HigherX + 1); x++) {
                for (int y = Math.Max(0, region.LowerY); y < Math.Min(canvas.Height, region.HigherY + 1); y++) {
                    var cellInfo   = canvas[x, y];
                    if (cellInfo.ZIndex <= zIndex) {
                        cellInfo.Style = cellInfo.Style.OverrideWith(style);
                        cellInfo.ZIndex = zIndex;
                    }
                }
            }
        }

        public void StyleBackground(Rectangle region, NKColor color, int zIndex = 0) {
            for (int x = Math.Max(0, region.LowerX); x < Math.Min(canvas.Width, region.HigherX + 1); x++) {
                for (int y = Math.Max(0, region.LowerY); y < Math.Min(canvas.Height, region.HigherY + 1); y++) {
                    var cellInfo   = canvas[x, y];
                    if (cellInfo.ZIndex <= zIndex) {
                        cellInfo.Style = cellInfo.Style.SafeSetBColor(color);
                        cellInfo.ZIndex = zIndex;
                    }
                }
            }
        }

        public void StyleTextColor(Rectangle region, NKColor color, int zIndex = 0) {
            for (int x = Math.Max(0, region.LowerX); x < Math.Min(canvas.Width, region.HigherX + 1); x++) {
                for (int y = Math.Max(0, region.LowerY); y < Math.Min(canvas.Height, region.HigherY + 1); y++) {
                    var cellInfo   = canvas[x, y];
                    if (cellInfo.ZIndex <= zIndex) {
                        cellInfo.Style = cellInfo.Style.SafeSetFColor(color);
                        cellInfo.ZIndex = zIndex;
                    }
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
                    var cellInfo   = canvas[x, y];
                    cellInfo.Style = style;
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
                    var cellInfo   = canvas[x, y];
                    var s          = cellInfo.Style;
                    cellInfo.Style = s with { BColor = color };
                }
            }
        }

        public void Style<T>(NKStyle style, Point offset, T?[,] mask) {
            for (int x = offset.X; x < Math.Min(mask.Len0 + offset.X, canvas.Width); x++) {
                for (int y = offset.Y; y < Math.Min(mask.Len1 + offset.Y, canvas.Height); y++) {
                    if (mask[x - offset.X, y - offset.Y] == null) continue;
                    
                    var cellInfo   = canvas[x, y];
                    cellInfo.Style = cellInfo.Style.OverrideWith(style);
                }
            }
        }

        public void Place(string s, Point offset, int window, HorizontalAlign align) {
            if (offset.Y != ..^canvas.Height) return;

            var xo = offset.X + align switch {
                HorizontalAlign.LEFT   => 0,
                HorizontalAlign.CENTER => (window - s.Length) / 2,
                HorizontalAlign.RIGHT  => window - s.Length,
                _ => 0
            };
                
            for (int i = 0; i < s.Length; i++) {
                if (i + xo == ..^canvas.Width) canvas[i + xo, offset.Y].Char = s[i];
            }
        }
        
        public void Place(AnsiString s, Point offset, int window, HorizontalAlign align) {
            if (offset.Y != ..^canvas.Height) return;

            var xo = offset.X + align switch {
                HorizontalAlign.LEFT   => 0,
                HorizontalAlign.CENTER => (window - s.Length) / 2,
                HorizontalAlign.RIGHT  => window - s.Length,
                _                      => 0
            };

            canvas.Place(s, offset with { X = xo });
        }
        
        /// <summary>
        /// Applies the specified style to all characters within the defined rectangular region on the canvas.
        /// </summary>
        /// <param name="region">The rectangular area on the canvas where the style will be applied.</param>
        /// <param name="c">The character to be used when filling the region.</param>
        public void Fill(Rectangle region, char c, int zIndex = 0) {
            for (int x = Math.Max(0, region.LowerX); x < Math.Min(canvas.Width, region.HigherX + 1); x++) {
                for (int y = Math.Max(0, region.LowerY); y < Math.Min(canvas.Height, region.HigherY + 1); y++) {
                    var cellInfo  = canvas[x, y];
                    if (cellInfo.ZIndex <= zIndex) {
                        cellInfo.Char = c;
                        cellInfo.ZIndex = zIndex;
                    }
                }
            }
        }

        /// <summary>
        /// Fills the specified rectangular region on the canvas with the given <see cref="AnsiChar"/>.
        /// This method updates both the character and the style of each cell in the region.
        /// </summary>
        /// <param name="region">The rectangular area on the canvas to be filled.</param>
        /// <param name="c">The AnsiChar containing the character and style to apply.</param>
        public void Fill(Rectangle region, AnsiChar c, int zIndex = 0) {
            for (int x = Math.Max(0, region.LowerX); x < Math.Min(canvas.Width, region.HigherX + 1); x++) {
                for (int y = Math.Max(0, region.LowerY); y < Math.Min(canvas.Height, region.HigherY + 1); y++) {
                    var cellInfo   = canvas[x, y];
                    if (cellInfo.ZIndex <= zIndex) {
                        cellInfo.Char  = c.Char;
                        cellInfo.Style = cellInfo.Style.OverrideWith(c.Style);
                        cellInfo.ZIndex = zIndex;
                    }
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
                    var cellInfo  = canvas[x, y];
                    cellInfo.Char = c;
                }
            }
        }

        /// <summary>
        /// Forces a fill of the specified rectangular region on the canvas with the given <see cref="AnsiChar"/>,
        /// overwriting any existing styles.
        /// </summary>
        /// <param name="region">The area on the canvas to be forcefully filled.</param>
        /// <param name="c">The AnsiChar containing the character and style to forcefully apply.</param>
        public void ForceFill(Rectangle region, AnsiChar c) {
            for (int x = Math.Max(0, region.LowerX); x < Math.Min(canvas.Width, region.HigherX + 1); x++) {
                for (int y = Math.Max(0, region.LowerY); y < Math.Min(canvas.Height, region.HigherY + 1); y++) {
                    var cellInfo   = canvas[x, y];
                    cellInfo.Char  = c.Char;
                    cellInfo.Style = c.Style;
                }
            }
        }

        internal void StyleCheckerBckg(Rectangle region, Size fieldSize, NKColor c0, NKColor c1) {
            int cw = Math.Max(1, fieldSize.Width);
            int ch = Math.Max(1, fieldSize.Height);

            int startX = Math.Max(0, region.LowerX);
            int endX   = Math.Min(canvas.Width, region.HigherX + 1);
            int startY = Math.Max(0, region.LowerY);
            int endY   = Math.Min(canvas.Height, region.HigherY + 1);

            if (startX >= endX || startY >= endY) 
                return;

            for (int x = startX; x < endX; x++) {
                bool xs = x / cw % 2 == 1;
                
                for (int y = startY; y < endY; y++) {
                    bool ys = y / ch % 2 == 1;
                    var color = xs ^ ys ? c0 : c1;
                    
                    var cellInfo   = canvas[x, y];
                    cellInfo.Char  = ' ';
                    cellInfo.Style = cellInfo.Style with { BColor = color, Styles = TextStyles.NONE };
                }
            }
        }

        /// <summary>
        /// Resets the style of cells within the specified region of the canvas to have no styles,
        /// except for cells overlapping with the defined mask.
        /// </summary>
        /// <param name="region">The rectangular area on the canvas within which the styles will be reset.</param>
        /// <param name="mask">The rectangular mask that specifies cells to exclude from the style reset operation.</param>
        public void ResetStyle(Rectangle region, Rectangle mask) {
            for (int x = Math.Max(0, region.LowerX); x < Math.Min(canvas.Width, region.HigherX + 1); x++) {
                for (int y = Math.Max(0, region.LowerY); y < Math.Min(canvas.Height, region.HigherY + 1); y++) {
                    if (mask.Contains(x, y)) continue;
                    var cellInfo = canvas[x, y];
                    cellInfo.Style = cellInfo.Style with { Styles = TextStyles.NONE };
                }
            }
        }
    }
}