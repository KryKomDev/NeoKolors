# NeoKolors.Console
This package has basic console utilities like colored text writing and fancy debugging functions.

## Contents
* ConsoleColors
* Debug
* ConsoleProgressBar

## ConsoleColors
This class contains functions for writing colored text to console.

Example:
```csharp
string test = "Lorem ipsum dolor sit amet...";
ConsoleColors.PrintlnColored(test, 0xff0000);
```

This code will print red text.

## Debug
Contains 5 functions for sending colored debug messages to console.

### Methods:
* Fatal - fatal error. Program should end after calling this.
* Error - non-fatal error. 
* Warn - warning. Something wrong can happen.
* Info - info. Something happened.
* Msg - for debugging.

## ConsoleProgressBar
Prints an interactive progress bar to the console, that updates as the task is being completed.

### Usage:
```csharp
public static event EventHandler Event;

public static void Main() {
    int start = 0;
    int end = 4000;
    
    ConsoleProgressBar bar = new ConsoleProgressBar(start, end, 40, ConsoleProgressBar.BarStyle.MODERN);
        
    Event += bar.OnProgressUpdate;
        
    for (int i = start; i < end; i++) {
        var g = new ImageGenerator(400, 400, i);
        var bmp = g.GenerateImage();
        bmp.Save($@".\{SeedFormat.WordFromSeed(i)}.png", ImageFormat.Png);
        Event.Invoke(null, EventArgs.Empty);
    }
}
```

This will make a progress bar that updates when a new image is generated and saved.