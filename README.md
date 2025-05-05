# NeoKolors

[![License](https://img.shields.io/github/license/KryKomDev/NeoKolors?color=2159a3)](https://raw.githubusercontent.com/KryKomDev/NeoKolors/main/LICENSE)

NeoKolors is a C# library with a number of color utilities and console graphics classes.

> [!NOTE]
> This project is the successor of the Kolors project. Consider moving away from the legacy 
> library.

---

## Common

![.NET Standard](https://img.shields.io/badge/.NET-Standard2.0-512bd4)
![.NET 5](https://img.shields.io/badge/.NET-5.0-682a7b)
[![NuGet](https://img.shields.io/nuget/v/NeoKolors.Common?color=a53c7a)](https://www.nuget.org/packages/NeoKolors.Common)
![Downloads](https://img.shields.io/nuget/dt/NeoKolors.Common?color=a31c35)

The NeoKolors.Common namespace contains all the utilities involving color formatting and color palettes.

### contents:
> * **[README](NeoKolors.Common/README.md)** - more information
> * **[ColorFormat](NeoKolors.Common/ColorFormat.cs)** - color format conversion methods
> * **[ColorPalette](NeoKolors.Common/ColorPalette.cs)** - color palette struct and color palette generation
> * **[StringEffects](NeoKolors.Common/StringEffects.cs)** - methods for adding colors and font styles to strings using ansi control characters
> * **[Color](NeoKolors.Common/Color.cs)** - structure for holding both console color and custom rgb color
> * **[UInt24](NeoKolors.Common/UInt24.cs)** - unsigned 24-bit integer

---

## Console

![.NET Standard](https://img.shields.io/badge/.NET-Standard2.0-512bd4)
![.NET 5](https://img.shields.io/badge/.NET-5.0-682a7b)
[![NuGet](https://img.shields.io/nuget/v/NeoKolors.Console?color=a53c7a)](https://www.nuget.org/packages/NeoKolors.Console)
![Downloads](https://img.shields.io/nuget/dt/NeoKolors.Console?color=a31c35)

You can find some basic console color classes in this namespace.

### contents:
> * **[README](NeoKolors.Console/README.md)** - more information
> * **[ConsoleColors](NeoKolors.Console/ConsoleColors.cs)** - methods for printing colored strings to console
> * **Debug**
>   * [.Message](NeoKolors.Console/Debug.Message.cs) - colored debug messages with 5 different modes
>   * [.Exception](NeoKolors.Console/Debug.Exceptions.cs) - fancy exception throwing
>   * [.Settings](NeoKolors.Console/Debug.Settings.cs) - set styles of the fancy exception and debug messages
> * **[FancyException](NeoKolors.Console/FancyException.cs)** - wrapper for other exceptions, fancy when printed
> * **[ConsoleProgressBar](NeoKolors.Console/ConsoleProgressBar.cs)** - updating console progress bar

---

## Settings

![.NET Standard](https://img.shields.io/badge/.NET-Standard2.0-512bd4)
![.NET 5](https://img.shields.io/badge/.NET-5.0-682a7b)
[![NuGet](https://img.shields.io/nuget/v/NeoKolors.Settings?color=a53c7a)](https://www.nuget.org/packages/NeoKolors.Settings)
![Downloads](https://img.shields.io/nuget/dt/NeoKolors.Settings?color=a31c35)

Framework for instance-from-settings automation and communicating settings between different parts of applications.

Tutorial on how to use available in [README](NeoKolors.Settings/README.md)

---

## Tui

![.NET Standard](https://img.shields.io/badge/.NET-Standard2.0-512bd4)
![.NET 5](https://img.shields.io/badge/.NET-5.0-682a7b)

[//]: # ([![NuGet]&#40;https://img.shields.io/nuget/v/NeoKolors.Tui&#41;]&#40;https://www.nuget.org/packages/NeoKolors.Tui&#41;)

Framework for creating TUIs in console.

Coming soon...

---

## Gui
Multiplatform GUI framework.

Nobody knows if it will even be. But if yes, coming soon, but later than Tui...