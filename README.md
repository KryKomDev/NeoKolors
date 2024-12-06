# NeoKolors

NeoKolors is a C# library with a number of color utilities and console graphics classes.

## Common
The NeoKolors.Common namespace contains all the utilities involving color formatting and color palettes.

### classes:
> * ![Code](code.svg) ColorFormat - color format conversion
> * ![Code](code.svg) ColorPalette - color palette structs and auto generation
> * ![Code](code.svg) StringColors - adds color enabling characters to strings

## Console
You can find some basic console color classes in this namespace.

### classes:
> * ![Code](code.svg) ConsoleColors - methods for printing colored strings to console
> * ![Code](code.svg) Debug - colored debug messages with 5 different modes
> * ![Code](code.svg) VisualConsole - DEPRECATED 

## ConsoleGraphics
Includes classes for creating whole GUIs for console.

### classes:
> * ![Code](code.svg) ConsoleProgressBar - creates an interactive progress bar in the console
> * ![Folder](folder.svg) Settings
>   * ![Code](code.svg) SettingsBuilder - builds settings for UI input
>   * ![Code](code.svg) SettingsNode - represents single option of object creation from UI
>   * ![Code](code.svg) SettingsGroup - a group of different methods of inputting data that transform to the united group of data
>   * ![Code](code.svg) SettingsGroupOption - a single option for SettingsGroup
>   * ![Code](code.svg) Context - holds names and data about a group of arguments
>   * ![Code](code.svg) Arguments - factory methods of different argument types
>   * ![Folder](folder.svg) ArgumentType - contains different argument types
> * ![Folder](folder.svg) GUI
>   * ![Folder](folder.svg) Elements - contains graphical element classes
>   * ![Code](code.svg) GuiRenderer - renders