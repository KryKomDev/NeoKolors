+++
date = '2025-05-16T17:12:28+02:00'
draft = false
title = 'LoggerConfig'
+++

```c# {lineNos=false}
public struct LoggerConfig;
```

`LoggerConfig` struct holds configurations for the `NKLogger` logger.

---

## Level

```c# {lineNos=false}
public LoggerLevel Level { get; set; } = FATAL | ERROR | WARN | INFO | DEBUG | TRACE;
```

Defines which log messages will be sent to the output stream and which will be ignored. 

---

## FatalColor

```c# {lineNos=false}
public NKColor FatalColor { get; set; } = NKConsoleColor.DARK_RED;
```

Defines the color of fatal log messages.

---

## ErrorColor

```c# {lineNos=false}
public NKColor ErrorColor { get; set; } = NKConsoleColor.RED;
```

Defines the color of error log messages.

---

## WarnColor

```c# {lineNos=false}
public NKColor WarnColor { get; set; } = NKConsoleColor.YELLOW;
```

Defines the color of warning log messages.

---

## LogColor

```c# {lineNos=false}
public NKColor InfoColor { get; set; } = NKConsoleColor.GREEN;
```

Defines the color of info log messages.

---

## DebugColor

```c# {lineNos=false}
public NKColor DebugColor { get; set; } = NKConsoleColor.BLUE;
```

Defines the color of debug log messages.

---

## TraceColor

```c# {lineNos=false}
public NKColor TraceColor { get; set; } = NKConsoleColor.GRAY;
```

Defines the color of trace log messages.

---

## Output

```c# {lineNos=false}
public TextWriter Output { get; set; } = System.Console.Out;
```

Defines the output stream of the logger. By default, it is the standard output stream of a console.

---

## SimpleMessages

```c# {lineNos=false}
public bool SimpleMessages { get; set; } = false;
```

If true disables all color and style formatting of all the messages.

---

## HideTime

```c# {lineNos=false}
public bool HideTime { get; set; } = false;
```

If true does not show the timestamp of the messages.

---

## FileConfig

```c# {lineNos=false}
public LogFileConfig FileConfig { get; set; } = LogFileConfig.Custom();
```

Controls the built-in configurations of log files.
See [`LogFileConfig`](/console/logfileconfig) for more.

By default, the config type is `CUSTOM`, meaning that the stream 
stored in [`Output`](#output) is used.

> [!IMPORTANT]
> When `FileConfig` is edited (to a non-custom configuration), the `Output` property 
> is automatically set to match the new configuration. When it is set to custom, 
> it will not override the `Output` to allow the user to set a custom output stream.