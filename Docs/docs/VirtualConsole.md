# VirtualConsole

Global virtual console that saves all the styles, colors and characters.

## Fields
* [Styles](#styles)
* [Characters](#characters)

### Styles

```c#
private static long[,] Styles { get; set; }
```

Array that holds all styles of individual characters.
When accessing with `[x, y]`, `x` represents the x-th column from left to right
and `y` represents y-th row from top to bottom.

| bit number | meaning                                                                                      |
|------------|----------------------------------------------------------------------------------------------|
| 0 - 23     | custom foreground color in RGB where R is byte 0                                             |
| 23 - 47    | custom background color in RGB where R is byte 3                                             |
| 48 - 51    | palette foreground color                                                                     |
| 52 - 55    | palette foreground color                                                                     |
| 56 - 61    | text style bitmap (bold, italic, underline, faint, negative and strikethrough respectively)  |

### Characters

```C#
private static char[,] Characters { get; set; }
```

Array that holds all characters of the screen.
When accessing with `[x, y]`, `x` represents the x-th column from left to right
and `y` represents y-th row from top to bottom.