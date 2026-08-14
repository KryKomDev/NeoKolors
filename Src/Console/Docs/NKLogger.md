# Logging and Diagnostics Engine

**[NKLogger](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Console/Logger/NKLogger.cs)** provides high-performance, asynchronous structured logging capabilities for the NeoKolors ecosystem. It supports multiple logging levels, ANSI-colored output formatting, file targets, and custom layout configurations.

---

## 1. Log Levels & Methods

`NKLogger` exposes methods corresponding to different diagnostic priority levels:

```csharp
public void Fatal(string message); // Critical system failure
public void Error(string message); // Operational errors
public void Warn(string message);  // Warning conditions
public void Info(string message);  // Informational notifications
public void Debug(string message); // Debugging traces
public void Trace(string message); // Low-level detailed tracing
```

### Overloads
- **Object inputs**: Logs the `ToString()` value of any object. E.g., `logger.Debug(myObject);`
- **Composite Formatting**: E.g., `logger.Info("Initial startup complete in {0}ms", elapsedMs);`

---

## 2. Global Diagnostics Access (`NKDebug`)

The static class **[NKDebug](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Console/NKDebug.cs)** acts as the central hub for logging. It provides a default static logger (`NKDebug.Logger`) and holds configuration switches:

```csharp
using NeoKolors.Console;

// Log info using the global logger
NKDebug.Info("Application initialized successfully.");

// Configure global exception interception
NKDebug.ExceptionFormatting = true;
NKDebug.EnableExceptionInterruption();
```

---

## 3. Logger Configurations

Each logger instance is configured via its **`Config`** property.

```csharp
// Configure logger level filters
NKDebug.Logger.Level = LoggerLevel.ERROR | LoggerLevel.WARNING | LoggerLevel.INFORMATION;

// Disable ANSI coloring outputs (prints plain text logs)
NKDebug.Logger.SimpleMessages = true;
```

For more on formatting output targets and log rolling, see **[LoggerConfig](LoggerConfig.md)** and **[LogFileConfig](LogFileConfig.md)**.
