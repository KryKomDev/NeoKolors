+++
date = '2026-08-14T22:54:00+02:00'
draft = false
title = 'NKLogger'
+++

```c# {lineNos=false}
public sealed class NKLogger : ILogger
```

`NKLogger` provides high-performance structured logging, ANSI color styling, file logging backends, and integration with `Microsoft.Extensions.Logging.Abstractions` and .NET Dependency Injection (`IServiceCollection`).

---

## Log Methods

```c# {lineNos=false}
// Direct logging
public void Crit(AnsiString? message, string? source = null, EventId? id = null);
public void Error(AnsiString? message, string? source = null, EventId? id = null);
public void Warn(AnsiString? message, string? source = null, EventId? id = null);
public void Info(AnsiString? message, string? source = null, EventId? id = null);
public void Debug(AnsiString? message, string? source = null, EventId? id = null);
public void Trace(AnsiString? message, string? source = null, EventId? id = null);

// ILogger interface implementation
public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter);
public bool IsEnabled(LogLevel logLevel);
public IDisposable? BeginScope<TState>(TState state);
```

---

## Microsoft.Extensions.Logging Integration

NeoKolors can be registered as the primary logging provider in any .NET application:

```c# {lineNos=false}
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NeoKolors.Console;

var services = new ServiceCollection();

services.AddLogging(builder => {
    // Add NeoKolors console logging
    builder.AddNeoKolors(ansiConfig => {
        ansiConfig.ShowTime = true;
        ansiConfig.TimeFormat = "HH:mm:ss";
    });

    // Add NeoKolors file logging (sequential app_1.log, app_2.log...)
    builder.AddNeoKolorsFile(LogFileConfig.NewCount("./logs/app_{0}.log"));
});

using var provider = services.BuildServiceProvider();
var logger = provider.GetRequiredService<ILogger<Program>>();

logger.LogInformation("NeoKolors logger active!");
```

---

## Log Writers & Multi-Target Logging

`NKLogger` outputs via `ILogWriter`:

- **`AnsiLogWriter`**: Colorized ANSI output for terminal consoles with level themes and Powerline badges.
- **`TextLogWriter`**: Plain text logger for files or standard text streams.
- **`BinaryLogWriter`**: Binary log record serialization (`NKLogRecordSerializer`).
- **`CompositeLogWriter`**: Multi-target writer that dispatches log records simultaneously to multiple writers (e.g. Console + File log).

### Multi-Target Example

```c# {lineNos=false}
var consoleWriter = new AnsiLogWriter();
var fileWriter = TextLogWriter.CreateFromFile(LogFileConfig.Append("./logs/app.log"));

var compositeWriter = new CompositeLogWriter(consoleWriter, fileWriter);
var logger = new NKLogger(writer: compositeWriter, source: "AppService");

logger.Info("Logged to console and file simultaneously!");
```