# Logger Configuration

**[LoggerConfig](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Console/Logger/LoggerConfig.cs)** holds property settings that define how loggers behave, how messages are styled, and where they are output.

---

## 1. Filter Levels

```csharp
public LoggerLevel Level { get; set; }
```

Determines which log messages are written. By default, it is a flags mask: `FATAL | ERROR | WARNING | INFORMATION | DEBUG | TRACE`. You can filter logs by setting this flag combination:

```csharp
// Only print error and fatal logs to stdout
myLogger.Config.Level = LoggerLevel.ERROR | LoggerLevel.FATAL;
```

---

## 2. Style and Colors

Each log level has an assigned `NKColor` that styles the severity prefix (e.g. `[INFO]`) in supported terminal windows:

* **`FatalColor`**: Default is `NKConsoleColor.DARK_RED`.
* **`ErrorColor`**: Default is `NKConsoleColor.RED`.
* **`WarnColor`**: Default is `NKConsoleColor.YELLOW`.
* **`InfoColor`**: Default is `NKConsoleColor.GREEN`.
* **`DebugColor`**: Default is `NKConsoleColor.BLUE`.
* **`TraceColor`**: Default is `NKConsoleColor.GRAY`.

---

## 3. Formatting Flags

* **`SimpleMessages`** (`bool`): If set to `true`, disables all ANSI color and style escape sequences, outputting plain text logs (useful for cloud environments or standard files).
* **`HideTime`** (`bool`): If set to `true`, removes the date/time prefix from the start of logged strings.

---

## 4. Targets and Streams

### Output Stream
```csharp
public TextWriter Output { get; set; }
```
Specifies the target writing stream. Defaults to `System.Console.Out`.

### File Configuration
```csharp
public LogFileConfig FileConfig { get; set; }
```
Controls how files are created and rolled when logging to the filesystem instead of standard streams. See **[LogFileConfig](LogFileConfig.md)** for further details.
