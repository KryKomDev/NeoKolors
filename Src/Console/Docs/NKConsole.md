# NKConsole Input and Output Driver

**[NKConsole](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Console/NKConsole.Out.cs)** is the developer-facing class of the `NeoKolors.Console` driver. It wraps low-level virtual terminal I/O, controls screen buffers, listens for mouse/keyboard inputs, and formats stylized outputs like colored logs and aligned text tables.

---

## 1. Colored Writing APIs

`NKConsole` provides overloaded `Write` and `WriteLine` methods that accept color tokens (RGB, HEX, ConsoleColor, or `NKColor`):

### Standard Foreground and Background Output
```csharp
using NeoKolors.Console;

// Write text in RGB orange
NKConsole.WriteLine("Important Notice", NKColor.FromRgb(255, 120, 0));

// Write text with dark gray background
NKConsole.WriteLineB("Warning Line", NKConsoleColor.DARK_GRAY);

// Write text with both foreground and background styling
NKConsole.WriteLine("Success!", NKConsoleColor.GREEN, NKConsoleColor.BLACK);
```

### Formatted Printing (`WriteF` & `WriteLineF`)
You can format strings dynamically by supplying color parameters corresponding to formatting markers `{0}`, `{1}`, etc.:
```csharp
NKConsole.WriteLineF("{0}ERROR:{1} Action failed.", NKColor.Red, NKColor.Default);
```

### Styled Struct Writing
```csharp
var myStyle = new NKStyle(NKColor.FromRgb(0, 255, 0), NKColor.Inherit, TextStyles.Bold);
NKConsole.WriteLine("Success Output", myStyle);
```

---

## 2. Structured Table Printing (`WriteTable`)

`NKConsole` includes a table formatter to write data grids directly to the output stream. It automatically calculates column spacing, wraps contents, and renders neat border dividers.

### A. Rendering Custom Objects via Reflection
You can print an array of objects by specifying the headers. The formatter will automatically scan properties matching the header names using reflection:
```csharp
public class User 
{
    public string Name { get; set; }
    public string Role { get; set; }
}

var users = new[] {
    new User { Name = "Alice", Role = "Admin" },
    new User { Name = "Bob", Role = "Developer" }
};

// Prints a table with columns 'Name' and 'Role' populated from the users array
NKConsole.WriteTable(new[] { "Name", "Role" }, users);
```

### B. Custom Row Selector Tables
For exact control, supply a row selector delegate to convert items to string arrays:
```csharp
NKConsole.WriteTable(
    header: new[] { "ID", "Message" },
    data: logs,
    rowSelector: log => new[] { log.Id.ToString(), log.Text }
);
```

---

## 3. Terminal Control & Alt Screen Buffers

To build fullscreen TUIs without polluting the user's terminal scrollback history, `NKConsole` manages virtual terminal states:

```csharp
// 1. Enter the alternate screen buffer
NKConsole.EnableAltBuffer();

// 2. Hide the blinking text cursor for cleaner UI redraws
NKConsole.HideCursor();

// 3. Enable bracketed paste and mouse coordinate tracking
NKConsole.EnableMouseReporting(MouseReportingLevel.ALL);

// ... Run application loop ...

// 4. Cleanup on exit
NKConsole.ShowCursor();
NKConsole.DisableAltBuffer();
```

---

## 4. Input Events Hooks

Instead of blocking threads with `Console.ReadKey()`, subscribe to input event channels:

```csharp
// Subscribe to key press events
NKConsole.KeyDown += (KeyEventArgs e) => {
    if (e.Key == KeyCode.Escape) {
        NKConsole.WriteLine("Exit request received.");
    }
};

// Subscribe to mouse clicks and scrolls
NKConsole.Mouse += (MouseEvent e) => {
    if (e.Action == MouseAction.CLICK) {
        NKConsole.WriteLine($"Clicked at cell coordinates: X={e.Position.X}, Y={e.Position.Y}");
    }
};
```
