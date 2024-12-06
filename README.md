# NeoKolors

NeoKolors is a C# library with a number of color utilities and console graphics classes.

## Common
The NeoKolors.Common namespace contains all the utilities involving color formatting and color palettes.

### classes:
> * <span class="material-symbols-outlined">code</span> ColorFormat - color format conversion
> * <span class="material-symbols-outlined">code</span> ColorPalette - color palette structs and auto generation
> * <span class="material-symbols-outlined">code</span> StringColors - adds color enabling characters to strings

## Console
You can find some basic console color classes in this namespace.

### classes:
> * <span class="material-symbols-outlined">code</span> ConsoleColors - methods for printing colored strings to console
> * <span class="material-symbols-outlined">code</span> Debug - colored debug messages with 5 different modes
> * <span class="material-symbols-outlined">code</span> VisualConsole - DEPRECATED 

## ConsoleGraphics
Includes classes for creating whole GUIs for console.

### classes:
> * <span class="material-symbols-outlined">code</span> ConsoleProgressBar - creates an interactive progress bar in the console
> * <span class="material-symbols-outlined">folder</span> Settings
>   * <span class="material-symbols-outlined">code</span> SettingsBuilder - builds settings for UI input
>   * <span class="material-symbols-outlined">code</span> SettingsNode - represents single option of object creation from UI
>   * <span class="material-symbols-outlined">code</span> SettingsGroup - a group of different methods of inputting data that transform to the united group of data
>   * <span class="material-symbols-outlined">code</span> SettingsGroupOption - a single option for SettingsGroup
>   * <span class="material-symbols-outlined">code</span> Context - holds names and data about a group of arguments
>   * <span class="material-symbols-outlined">code</span> Arguments - factory methods of different argument types
>   * <span class="material-symbols-outlined">folder</span> ArgumentType - contains different argument types
> * <span class="material-symbols-outlined">folder</span> GUI
>   * <span class="material-symbols-outlined">folder</span> Elements - contains graphical element classes
>   * <span class="material-symbols-outlined">code</span> GuiRenderer - renders



<link rel="stylesheet" href="https://fonts.googleapis.com/css2?family=Material+Symbols+Outlined" />
<style>
.material-symbols-outlined {
    font-size: small;
    font-variation-settings:
        'FILL' 0,
        'wght' 400,
        'GRAD' 0,
        'opsz' 24;
}
</style>