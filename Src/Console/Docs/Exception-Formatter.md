# Exception Formatting & Visualization

The `NeoKolors.Console` project features a terminal-based exception formatter that maps standard .NET exceptions and unhandled stack traces into readable, structured, and syntax-highlighted blocks.

---

## 1. Global Interception

By default, an unhandled exception in a .NET application prints a dense, plain-text stack trace that disrupts terminal layout flow. NeoKolors can intercept these exceptions globally and format them:

```csharp
using NeoKolors.Console;

// Installs a global AppDomain UnhandledException / TaskScheduler UnobservedTaskException handler
NKDebug.EnableExceptionInterruption();
```

Once enabled, any unhandled thread exceptions will automatically render with custom NeoKolors ANSI decoration before the application terminates.

---

## 2. Formatting Configurations

The exception formatter can be configured using `ExceptionFormatter.Config`:

```csharp
using NeoKolors.Console;

// Configure whether stack trace line numbers and files are highlighted
NKDebug.Formatter.Config.ColorizeFiles = true;
NKDebug.Formatter.Config.ShowSourceSnippet = true; // Attempts to extract the source lines if local
```

---

## 3. Fancy Exceptions

The **[FancyException](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Console/FancyException.cs)** class is a custom exception type that embeds layout information. If thrown, the formatting engine reads its metadata to draw explicit warning containers:

```csharp
using NeoKolors.Console;

// Throws an exception containing styled details
throw new FancyException(
    message: "Failed to parse configuration file.",
    details: "The syntax at line 42 contains an invalid character token '{'. Expected ':'."
);
```
