# NeoKolors Documentation

NeoKolors is a C# library ecosystem providing color utilities, console graphics, and a Text User Interface (TUI) framework.

---

## Projects

### NeoKolors.Common
Provides common structures and classes for working with colors, ANSI escape sequences, Sixel graphics, and color palettes. Key types include `NKColor`, which supports RGB, hex, and standard console colors, and `NKStyle`, which combines foreground and background colors with text styles. The package also handles immutable styled `AnsiString` structures and high-performance raster-to-Sixel conversion for rendering images directly in compatible terminals.

### NeoKolors.Console
Provides a configurable console logger, low-level terminal controls, mouse and keyboard input events capture, and unhandled exception formatting. The `NKConsole` driver provides access to virtual terminal states, alternate screen buffers, bracketed paste mode, and mouse reporting. `NKLogger` provides asynchronous structured logging with levels and templates, while `ExceptionFormatter` transforms unhandled stack traces into visually descriptive layouts.

### NeoKolors.Extensions
Provides utility extension methods for string manipulation, word wrapping, case conversion, collection indexing, and key mapping. String helpers include `Chop` for paragraph wrapping and `GetPlainLength` to calculate printable text length after stripping ANSI formatting. The `UniversalNamingConvertor` supports case conversions across Pascal, camel, snake, screaming snake, kebab, and train case naming styles.

### NeoKolors.Settings
Provides a type-safe settings building and parsing framework to parse configurations and construct typed objects. The system uses a tree-based structure composed of `SettingsNode`, `SettingsGroup`, and `IArgument` variants. Developers define arguments and construct final typed configurations using builder contexts, allowing input validation and mutual options merging.

### NeoKolors.Tui
Provides a component-driven Text User Interface (TUI) framework featuring stateful controls, layout panels, visual states, and routed events. It uses stateful controls (such as `Panel`, `ContentControl`, `TextBlock`, and `TextBox`) and a structured layout system composed of `Grid`, `StackPanel`, `RelativePanel`, and `Canvas`. A visual state manager handles cursor hover and focus triggers, and events propagate via tunneling and bubbling.

### NeoKolors.Tui.Fonts
Provides typography and text rendering engines supporting ASCII and ANSI font assets and XML-based font compilation. It includes built-in stylized font resources (such as `Bytesized` and `Future`) and integrates an MSBuild compiler task to package font descriptor assets into embedded resources at build time. Custom fonts are defined using XML specifications for metrics and glyph coordinate grids.

---

## Repository and Package Availability

### GitHub Repository
The NeoKolors project is open-source and hosted on GitHub. The repository contains the source code, examples, unit tests, and the issue tracker for the library ecosystem.
* Repository URL: https://github.com/KryKomDev/NeoKolors

### NuGet Packages
All projects in the NeoKolors ecosystem are published on NuGet for integration into .NET applications. You can add references to individual packages using the .NET CLI or reference them manually in your project configuration.

```bash
dotnet add package NeoKolors.Common
dotnet add package NeoKolors.Console
dotnet add package NeoKolors.Settings
dotnet add package NeoKolors.Tui
```