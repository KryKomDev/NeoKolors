# NeoKolors

NeoKolors is a comprehensive C# library suite designed for modern console application development. It provides tools for advanced color manipulation, styling (ANSI), configurable logging, settings management, and a web-inspired TUI (Text User Interface) framework.

## Project Structure

The solution is divided into several specialized libraries and examples:

*   **`NeoKolors.Common`**: Core utilities for colors (RGB, ANSI) and basic helpers.
*   **`NeoKolors.Console`**: Advanced console logger with styling, exception formatting, and output management.
*   **`NeoKolors.Extensions`**: Extension methods for standard .NET types (Collections, Strings, etc.) to enhance productivity.
*   **`NeoKolors.Settings`**: A fluent framework for defining, parsing, and transferring application settings/configurations.
*   **`NeoKolors.Tui`**: A TUI framework allowing web-like development of console interfaces (similar to ncurses but with modern C# patterns).
*   **`Examples/`**: Contains sample applications demonstrating library features (e.g., `Neofetch`, `Ascii`).
*   **`Docs/`**: Documentation source (Hugo site).

## Prerequisites

*   **Dotnet SDK**: The project is configured to use modern .NET versions (Workflows suggest .NET 10.x, but libraries target .NET Standard 2.0/2.1 and .NET 5.0). Ensure you have a recent .NET SDK installed.

## Building and Testing

The project uses standard `dotnet` CLI commands.

### Build a Library
To build a specific library (e.g., `NeoKolors.Common`):

```bash
dotnet build NeoKolors.Common/NeoKolors.Common.csproj
```

### Run Tests
To run unit tests for a library (e.g., `NeoKolors.Common.Tests`):

```bash
dotnet test NeoKolors.Common.Tests/NeoKolors.Common.Tests.csproj
```

### Run Examples
To run one of the example applications (located in `Examples/`), navigate to the folder and use `dotnet run`:

```bash
# Example: Running the Neofetch clone
cd Examples/Neofetch
dotnet run
```

## Key Workflows

*   **CI:** GitHub Actions are defined in `.github/workflows/` to automatically build and test each component on push.
*   **Documentation:** The `Docs/` folder contains a Hugo-based documentation site.

## Development Conventions

*   **Code Style:** Standard C# conventions apply.
*   **Tests:** Each core library has a corresponding `.Tests` project (e.g., `NeoKolors.Common` -> `NeoKolors.Common.Tests`). Ensure new features include unit tests.
*   **Dependencies:** The libraries are designed to be modular. Avoid circular dependencies between the components.
