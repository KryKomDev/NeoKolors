# NeoKolors Structured Logging & Diagnostics

The `NeoKolors.Console` library includes a high-performance diagnostic system under **[NKDebug](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Console/NKDebug.cs)** and **[NKLogger](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Console/NKLogger.Config.cs)**. This system is tailored for fast, non-blocking asynchronous writing and pretty formatting.

---

## 1. Log Levels

The logging system supports standard log levels, defined in [LoggerLevel](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Console/LoggerLevel.cs):

| Level | Method | Purpose |
| :--- | :--- | :--- |
| `Trace` | `NKDebug.Trace(...)` | Granular debugging details, active only during deep diagnostics. |
| `Debug` | `NKDebug.Debug(...)` | General development diagnostic details. |
| `Info` | `NKDebug.Info(...)` | Major execution milestones (e.g. startup, network initialization). |
| `Warn` | `NKDebug.Warn(...)` | Non-critical failures or potential misconfigurations. |
| `Error` | `NKDebug.Error(...)` | Recoverable error states where operation continues. |
| `Crit` | `NKDebug.Crit(...)` | Critical failure states requiring immediate attention. |

---

## 2. Template-Based Structured Logging

The logger uses semantic template parsing. Instead of formatting strings beforehand, pass arguments as parameters. This keeps the log files searchable and optimizes serialization:

```csharp
using NeoKolors.Console;

// The logger dynamically replaces {Key} and {Value}
NKDebug.Info("Loading configuration: {Key} = {Value}", "Theme", "MaterialDark");

// Can also pass complex objects
NKDebug.Warn("User {UserId} failed authentication from {IpAddress}", 1024, "127.0.0.1");
```

---

## 3. Configuring Log Output Backends

The logger can output to multiple targets concurrently, including standard output, standard error, or log files.

### 3.1 File Logging Configuration
To configure date-based log rotation, use `LogFileConfig`:

```csharp
using NeoKolors.Console;

// Create a datetime rotated file config: logs will be written to e.g. "./logs/2026-06-05.log"
NKDebug.Logger.FileConfig = LogFileConfig.NewDatetime("./logs/{0:yyyy-MM-dd}.log");
```

### 3.2 Main Logger Level Configurations
You can adjust logging thresholds dynamically:

```csharp
using NeoKolors.Console;

// Suppress all logs below Warn in production
NKDebug.Logger.MinLevel = LoggerLevel.Warn;
```
