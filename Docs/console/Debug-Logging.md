# Debug.Logging

The `Debug.Logging.cs` contains all the logging methods.
The logger contains five levels of messages:
1. `Fatal` - for fatal error messages
2. `Error` - for non-fatal error messages
3. `Warn` - for warning messages
4. `Info` - for info messages
5. `Log` - for debug logging messages

## Message configuration

### Color

The following example shows how you can change the color of the debug messages:

```c#
// change to perfect blue
Debug.DebugColor = 0x0000ff;

// change to console blue
Debug.DebugColor = NKConsoleColor.BLUE;
```

### Disabling colors

You can disable the color of the messages by setting the `Debug.SimpleMessages` 
field to false. This can come in handy if you're logging into a file and don't 
want it to have loads of escape codes in it. That will make the messages look 
similar to this:
```log
[2025-03-24 18:06:13] [ FATAL ] : fatal
[2025-03-24 18:06:13] [ ERROR ] : error
[2025-03-24 18:06:13] [ WARN ] : warn
[2025-03-24 18:06:13] [ INFO ] : info
[2025-03-24 18:06:13] [ DEBUG ] : debug
```


## Output configuration

### Levels

You can control what messages will be printed by setting the `Debug.Level` field.
The following example shows how you can change the settings to print only errors and debug messages:

```c#
using static NeoKolors.Console.DebugLevels;

Debug.Level = FATAL | ERROR | DEBUG;
```

### Output stream

The `Debug.Output` controls what `System.IO.TextWriter` will be used to print the messages.
For example to change the output to a file `test.log` in the runtime directory you could do:

```c#
Debug.Output = new StreamWriter("./test.log");
```

<note>
    Be sure to also change the <code>Debug.SimpleMessages</code> to true to get rid of ansi escape
    sequences when logging to a file.
</note>