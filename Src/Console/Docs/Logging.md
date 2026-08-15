# NeoKolors Logging & Diagnostics Guide

The **NeoKolors** logging engine provides high-performance ANSI-styled console output, file logging (plain text and binary), structured log record serialization, and seamless integration with `Microsoft.Extensions.Logging.Abstractions` and .NET Dependency Injection (`IServiceCollection`).

---

## 1. Overview & Architecture

The logging system consists of several modular components:

| Component                  | Class                                            | Description                                                                                                      |
|:---------------------------|:-------------------------------------------------|:-----------------------------------------------------------------------------------------------------------------|
| **Logger Core**            | `NKLogger`                                       | Primary logger implementing `Microsoft.Extensions.Logging.ILogger`.                                              |
| **DI Integration**         | `NKLoggerProvider`, `NKLoggingBuilderExtensions` | `ILoggerProvider` implementation and `ILoggingBuilder` / `IServiceCollection` extension methods.                 |
| **Console Output**         | `AnsiLogWriter`                                  | Formats and prints styled ANSI log records to console with configurable level themes and Powerline badges.       |
| **Plain Text File Output** | `TextLogWriter`                                  | Formats and writes unstyled log records to files or any `TextWriter`.                                            |
| **Binary Output**          | `BinaryLogWriter`                                | Serializes log records to binary streams using `NKLogRecordSerializer`.                                          |
| **Multi-Target Logging**   | `CompositeLogWriter`                             | Dispatches log records simultaneously to multiple writers (e.g. Console + File log).                             |
| **File Management**        | `LogFileConfig`                                  | Configures file creation and rotation modes (`Replace`, `Append`, `NewCount`, `NewDatetime`, `NewHashDatetime`). |

---

## 2. Using `NKLogger` with .NET Dependency Injection

NeoKolors integrates natively with `Microsoft.Extensions.Logging`. You can register NeoKolors in any generic host, Web API, Worker Service, or `ServiceCollection`.

### 2.1 Basic DI Registration

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NeoKolors.Console;

var services = new ServiceCollection();

// Register NeoKolors as the logging provider
services.AddLogging(builder => {
    builder.AddNeoKolors();
});

using var provider = services.BuildServiceProvider();
var logger = provider.GetRequiredService<ILogger<Program>>();

logger.LogInformation("NeoKolors logger registered successfully!");
logger.LogWarning("System warning event ID: {EventId}", 404);
```

### 2.2 Configuring ANSI Console Theme

You can customize timestamp formats and ANSI level styles when registering:

```csharp
services.AddLogging(builder => {
    builder.AddNeoKolors(ansiConfig => {
        ansiConfig.ShowTime = true;
        ansiConfig.TimeFormat = "HH:mm:ss.fff";
        ansiConfig.FormatExceptions = true;
    });
});
```

### 2.3 File Logging via `ILoggingBuilder`

To write logs simultaneously to Console and a log file:

```csharp
services.AddLogging(builder => {
    // Generates sequential log files: logs/app_1.log, logs/app_2.log, etc.
    builder.AddNeoKolorsFile(LogFileConfig.NewCount("./logs/app_{0}.log"));
});
```

---

## 3. Standalone Usage

You can also use `NKLogger` directly without Dependency Injection.

### 3.1 Basic Standalone Usage

```csharp
using NeoKolors.Console;

// Create a logger with default AnsiLogWriter
var logger = new NKLogger(source: "DatabaseService");

logger.Info("Connected to database successfully.");
logger.Warn("Query execution took longer than expected.");
logger.Error("Database connection lost!");
```

### 3.2 Global Diagnostic Hub (`NKDebug`)

For application-wide logging and unhandled exception capture, use `NKDebug`:

```csharp
using NeoKolors.Console;

NKDebug.Info("Initializing application...");
NKDebug.Warn("Configuration value missing, using fallback.");

// Intercept unhandled exceptions with styled formatting
NKDebug.ExceptionFormatting = true;
NKDebug.EnableExceptionInterruption();
```

---

## 4. Log Levels & Level Filtering

`NKLogger` supports six log levels defined in the bitflag enum `NKLogLevel`:

| Level Flag    | `LogLevel` Equivalent  | Helper Method           |
|:--------------|:-----------------------|:------------------------|
| `CRITICAL`    | `LogLevel.Critical`    | `logger.SetLogCrit()`   |
| `ERROR`       | `LogLevel.Error`       | `logger.SetLogErrors()` |
| `WARNING`     | `LogLevel.Warning`     | `logger.SetLogWarn()`   |
| `INFORMATION` | `LogLevel.Information` | `logger.SetLogInfo()`   |
| `DEBUG`       | `LogLevel.Debug`       | `logger.SetLogAll()`    |
| `TRACE`       | `LogLevel.Trace`       | `logger.SetLogAll()`    |

### Configuring Thresholds

```csharp
var logger = new NKLogger();

// Only log Warnings, Errors, and Critical messages
logger.SetLogWarn();

// Or set custom level flags explicitly using bitwise OR
logger.Level = NKLogLevel.ERROR | NKLogLevel.CRITICAL;
```

---

## 5. Log File Configuration (`LogFileConfig`)

`LogFileConfig` controls path resolution and file creation strategies.

### 5.1 File Modes

```csharp
// Overwrite target file on each run
var replaceConfig = LogFileConfig.Replace("./logs/current.log");

// Append new entries to target file
var appendConfig = LogFileConfig.Append("./logs/app.log");

// Stateless sequential count: app_1.log, app_2.log, app_3.log...
var countConfig = LogFileConfig.NewCount("./logs/app_{0}.log");

// Date/time timestamped file: app_2026.08.14-22.30.00.log
var dateTimeConfig = LogFileConfig.NewDatetime("./logs/app_{0}.log");

// Unique hash + timestamped file
var hashConfig = LogFileConfig.NewHash("./logs/app_{0}.log");
```

> **Note**: `LogFileConfig.NewCount` is completely stateless—it scans existing files in the target directory to determine the next sequential index without creating hidden `.nklog` tracking files.

---

## 6. Multi-Target Logging (`CompositeLogWriter`)

To direct log output to multiple destinations (for example, printing ANSI output to console while appending plain text to a file):

```csharp
using NeoKolors.Console;

var consoleWriter = new AnsiLogWriter();
var fileWriter = TextLogWriter.CreateFromFile(LogFileConfig.NewCount("./logs/app_{0}.log"));

// Combine writers
var compositeWriter = new CompositeLogWriter(consoleWriter, fileWriter);

// Use composite writer in logger
var logger = new NKLogger(writer: compositeWriter, source: "AppCore");

logger.Info("This message appears in BOTH the console and the log file!");
```

Alternatively, configure `NKLoggerOptions`:

```csharp
var options = new NKLoggerOptions()
    .UseFileLogging(LogFileConfig.Append("./logs/app.log"));

using var provider = new NKLoggerProvider(options);
var logger = provider.CreateLogger("OrderService");
```

---

## 7. Exception Formatting & Scopes

### 7.1 Rich Exception Formatting

`AnsiLogWriter` integrates with `ExceptionFormatter` to format exception stack traces with syntax-highlighted frames, parameters, and inner exception trees.

```csharp
try {
    throw new InvalidOperationException("Failed to process transaction.");
}
catch (Exception ex) {
    logger.Error(ex);
}
```

### 7.2 Logging Scopes

`NKLogger` supports `BeginScope`:

```csharp
using (logger.BeginScope("TransactionScope: {Id}", Guid.NewGuid())) {
    logger.LogInformation("Processing payment step 1");
    logger.LogInformation("Processing payment step 2");
}
```
