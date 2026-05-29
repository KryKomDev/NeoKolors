// NeoKolors
// Copyright (c) krystof 2026

using Metriks;
using NeoKolors.Common;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Fonts;

public interface IExtendedAsciiFont : IAsciiFont {
    
    /// <summary>
    /// Renders an ASCII string onto a specified character canvas within given bounds
    /// and using defined text rendering options.
    /// </summary>
    /// <param name="str">The <see cref="string"/> containing the text to be rendered.</param>
    /// <param name="canvas">The <see cref="ICharCanvas"/> onto which the text should be rendered.</param>
    /// <param name="bounds">The <see cref="Area2D"/> defining the boundaries within which the text will be placed.</param>
    /// <param name="options">The <see cref="TextRenderingOptions"/> used to customize the rendering process.</param>
    public void PlaceString(string str, ICharCanvas canvas, Area2D bounds, TextRenderingOptions options);
    
    /// <summary>
    /// Renders an ASCII string onto a specified character canvas within given bounds
    /// and using defined text rendering options.
    /// </summary>
    /// <param name="str">The <see cref="AnsiString"/> containing the text to be rendered.</param>
    /// <param name="canvas">The <see cref="ICharCanvas"/> onto which the text should be rendered.</param>
    /// <param name="bounds">The <see cref="Area2D"/> defining the boundaries within which the text will be placed.</param>
    /// <param name="options">The <see cref="TextRenderingOptions"/> used to customize the rendering process.</param>
    public void PlaceString(AnsiString str, ICharCanvas canvas, Area2D bounds, TextRenderingOptions options);
}