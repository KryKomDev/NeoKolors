//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Common;

/// <summary>
/// A static class that provides predefined color palettes for use in various graphical applications.
/// </summary>
/// <remarks>
/// The NKPalettes class includes a collection of statically defined color palettes, each represented
/// as a <see cref="NKColorPalette"/> object. These palettes offer a wide range of color schemes
/// suitable for UI design, data visualization, and artistic projects. The class also provides
/// functionality to display all available palettes in a formatted manner.
/// </remarks>
/// <example>
/// This class is commonly used to access predefined color palettes without the need for manual
/// creation. Each palette is accessible as a static property, and the <see cref="PrintAll"/> method
/// allows for the listing of all palettes and their colors in the console.
/// </example>
public static class NKPalettes {
    
    public static NKColorPalette VermilionGreen => new("ffe084-b8b42d-697a21-fffce8-3e363f-8e3b3d-dd403a");
    public static NKColorPalette VibrantRainbow => new("e6c229-f17105-d11149-6610f2-1a8fe3");
    public static NKColorPalette Green => new("dad7cd-a3b18a-588157-3a5a40-344e41");
    public static NKColorPalette GreenExtended => new("f2f2f2-dad7cd-a3b18a-588157-3a5a40-344e41-262e2a-111312");
    public static NKColorPalette PinkishHeaven => new("fdc5f5-f7aef8-b388eb-8093f1-72ddf7");
    public static NKColorPalette BrownBlue => new("483d3f-058ed9-f4ebd9-a39a92-77685d");
    public static NKColorPalette BlueOrange => new("3d348b-7678ed-f7b801-f18701-f35b04");
    public static NKColorPalette Purple => new("e2c2c6-b9929f-9c528b-610f7f-2f0147");
    public static NKColorPalette BlueGray => new("8a716a-c2b8b2-197bbd-125e8a-204b57");
    public static NKColorPalette PaleBlue => new("000000-1098f7-ffffff-b89e97-decccc");
    public static NKColorPalette BrownPink => new("260c1a-432e36-5f4842-af8d86-edbfc6-f3d7dc-f1f1f1");
    public static NKColorPalette Gymlit => new("2a15e1-e2e20d-ffffff-504850-3e363f");
    public static NKColorPalette GrayRed => new("141418-2b2d42-8d99ae-edf2f4-ef233c-d90429-9b2337");
    public static NKColorPalette MonoBlue => new("f4f4ff-bfcaeb-899fd6-46567c-020c22");

    /// <summary>
    /// Prints all color palettes defined in the NKPalettes class to the console.
    /// Each palette is displayed with its name and corresponding colors in the palette.
    /// </summary>
    /// <remarks>
    /// The method dynamically retrieves all static properties of type NKColorPalette
    /// from the NKPalettes class, formats their names and colors, and prints
    /// them in a readable format through the associated PrintPalette method.
    /// </remarks>
    public static void PrintAll() {
        GetAllPalettes(out var palettes, out var maxLength);

        foreach (var p in palettes) {
            Console.Write($"{p.Key}:{new string(' ', maxLength - p.Key.Length)} ");
            p.Value.PrintPalette();
        }
    }

    /// <summary>
    /// Retrieves all color palettes defined in the NKPalettes class,
    /// along with their names and the length of the longest name.
    /// </summary>
    /// <param name="palettes">
    /// An output parameter that will contain a dictionary where keys represent palette
    /// names and values represent their respective NKColorPalette instances.
    /// </param>
    /// <param name="maxNameLength">
    /// An output parameter that will hold the length of the longest palette name.
    /// </param>
    public static void GetAllPalettes(out Dictionary<string, NKColorPalette> palettes, out int maxNameLength) {
        var fields = typeof(NKPalettes).GetProperties();
        palettes = new();
        int maxLength = 0;
        
        foreach (var field in fields) {
            if (field.PropertyType != typeof(NKColorPalette)) continue;
            
            var palette = field.CanRead ? field.GetValue(null) : null;
            if (palette is not NKColorPalette p) continue;
                
            palettes.Add(field.Name, p);
            if (field.Name.Length > maxLength) maxLength = field.Name.Length;
        }
        
        maxNameLength = maxLength;
    }
}