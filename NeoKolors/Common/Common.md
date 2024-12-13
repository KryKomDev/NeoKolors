# NeoKolors.Common

The NeoKolors.Common namespace includes 3 library classes:
* ColorFormat
* ColorPalette
* StringEffects

---

## ColorFormat

Exposes methods for color format conversion.

Example use:

```csharp
Color c = Color.FromArgb(255, 255, 255);
(double h, double s, double v) HSV = ColorFormat.ColorToHsv(c);
// hsv : 1, 1, 1
```

---

## ColorPalette

Stores color palettes. You can also generate new ones with the ```GeneratePalette``` method.

Example use:

```csharp
ColorPalette cp = ColorPalette.GeneratePalette(5, 7);
cp.PrintPalette();
```

this program will output the following colors:
`#CCBDA1` `#BDA260` `#AC4A5F` `#9AF19E` `#86D40A` `#720587` `#5D66F1`

---

## StringEffects

Methods that add color and other effect characters to the string

Example usage:

```csharp
// this will add special effects to the string
string s = StringEffects.AddTextStyles(
    "<b>bold</b>\n" +
    "<i>italic</i>\n" +
    "<u>underline</u>\n" + 
    "<f>faint</f>\n" +
    "<u>underline</u>\n" + 
    "<n>inverse</n>\n" +
    "<s>strikethrough</s>"
);

// the following will add a color to the string:
string s = StringEffects.AddColor("this text will be colored!", 0x5D66F1);
```