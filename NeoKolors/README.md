# NeoKolors

NeoKolors is a C# library with a number of color utilities and console graphics classes.

## Common
The NeoKolors.Common namespace contains all the utilities involving color formatting and color palettes.

### classes:
> * ColorFormat - color format conversion
> * ColorPalette - color palette structs and auto generation
> * StringColors - adds color enabling characters to strings

## Console
You can find some basic console color classes in this namespace.

### classes:
> * ConsoleColors - methods for printing colored strings to console
> * Debug - colored debug messages with 5 different modes
> * VisualConsole - DEPRECATED 

## ConsoleGraphics
Includes classes for creating whole GUIs for console.

# classes:
> * ConsoleProgressBar
> * Settings/
>   * SettingsBuilder - builds settings for UI input
>   * SettingsNode - represents single option of object creation from UI
>   * SettingsGroup - a group of different methods of inputting data that transform to the united group of data
>   * SettingsGroupOption - a single option for SettingsGroup
>   * Context - holds names and data about a group of arguments
>   * Arguments - factory methods of different argument types
>   * ArgumentType/ - contains different argument types
> * GUI/
>   * 