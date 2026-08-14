# Exception Formatting & Visualization

The `NeoKolors.Console` project features a rich, highly customizable terminal exception formatter that maps standard .NET exceptions, inner exceptions, aggregate exceptions, and stack traces into structured, syntax-highlighted visual blocks.

---

## 1. Quick Usage

```csharp
using NeoKolors.Console;

// Enable global interception of unhandled exceptions
NKDebug.EnableExceptionInterruption();

// Format an exception to a stylized AnsiString
Exception ex = new InvalidOperationException("Failed to process transaction.");
AnsiString styled = ExceptionFormatter.Format(ex);

System.Console.WriteLine(styled);
```

---

## 2. Formatting Configuration (`ExceptionFormat`)

The exception formatter provides fine-grained control over colors, borders, stack trace depth, local source code snippets, inner exceptions, and data dictionaries.

```csharp
using NeoKolors.Console;

// Configure global ExceptionFormatter settings
ExceptionFormatter.Config.BorderStyle = ExceptionBorderStyle.Box;
ExceptionFormatter.Config.ShowSourceSnippet = true;
ExceptionFormatter.Config.SourceSnippetContextLines = 3;
ExceptionFormatter.Config.ShowHResult = true;
ExceptionFormatter.Config.ShowData = true;

// Or create a custom ExceptionFormat instance
var customFormat = new ExceptionFormat {
    BorderStyle = ExceptionBorderStyle.LeftBar,
    HighlightColor = NKConsoleColor.RED,
    FilterSystemFrames = true,
    PathFormat = PathFormatMode.FileNameOnly,
    ShowInnerExceptions = true
};

AnsiString result = ExceptionFormatter.Format(ex, customFormat);
```

---

## 3. Presets

Pre-built layout configurations are available via static properties on `ExceptionFormat`:

- **`ExceptionFormat.Default`**: Balanced layout with left highlight bar, file/line highlighting, and local source snippets.
- **`ExceptionFormat.Compact`**: Single-line stack frames, system/framework frames filtered out, max depth of 5.
- **`ExceptionFormat.Detailed`**: Enclosed in a rounded box frame with HResult, data dictionary, and 3 context lines of source code.
- **`ExceptionFormat.Minimal`**: Exception type and message only, without stack trace.
- **`ExceptionFormat.Plain`**: Clean un-styled plain text format without highlights.

---

## 4. Key Customization Features

| Option | Type | Description |
|---|---|---|
| `BorderStyle` | `ExceptionBorderStyle` | Border style (`LeftBar`, `Box`, `None`). |
| `HighlightColor` | `NKColor` | Color of the highlight bar or box frame. |
| `HighlightChar` | `string` | Character sequence for the left bar (default `"▍ "`). |
| `ShowSourceSnippet` | `bool` | Extracts local source code lines around the error line. |
| `SourceSnippetContextLines` | `int` | Number of context lines before and after error line. |
| `FilterSystemFrames` | `bool` | Hides framework/system stack frames (`System.*`, `Microsoft.*`). |
| `DimSystemFrames` | `bool` | Dims framework stack frames instead of hiding them. |
| `PathFormat` | `PathFormatMode` | File path display mode (`FullPath`, `FileNameOnly`, `RelativePath`). |
| `FrameFilter` | `Func<ParsedStackFrame, bool>` | Custom delegate to filter stack frames. |
| `PathTransformer` | `Func<string, string>` | Custom transformer for file paths in stack traces. |
| `ShowInnerExceptions` | `bool` | Recursively formats inner & aggregate exceptions. |
| `ShowData` | `bool` | Formats key-value pairs from `Exception.Data`. |
| `ShowHResult` | `bool` | Formats HResult code (e.g. `[0x80131500]`). |
