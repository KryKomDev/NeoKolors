---
name: build-and-test
description: >-
  Use this skill when the user asks to build the NeoKolors solution or projects,
  run unit tests, debug test failures, or check multi-targeting across .NET 8,
  .NET 10, and preview .NET 11 runtimes.
---

# Build and Test Runbook

This skill outlines how to properly build and test the NeoKolors ecosystem.

## Solution Structure

The main solution file is `NeoKolors.slnx`. The repository multi-targets:
- **`net8.0`** (LTS)
- **`net10.0`** (Current)
- **`net11.0`** (Preview runtime)
- **`netstandard2.1`** (for selected library abstractions)

## Build Commands

### 1. Build Entire Solution
```bash
# Debug build (fastest, includes profiling symbols)
dotnet build NeoKolors.slnx -c Debug

# Release build (optimizations enabled, matches CI)
dotnet build NeoKolors.slnx -c Release --no-restore
```

### 2. Build Specific Projects
When working on a single area, build only the affected project to save time:
```bash
# Common primitives
dotnet build Src/Common/NeoKolors.Common.csproj -c Debug

# Terminal console driver
dotnet build Src/Console/NeoKolors.Console.csproj -c Debug

# TUI Elements
dotnet build Src/Tui/Elements/NeoKolors.Tui.Elements.csproj -c Debug
```

## Running Tests

### 1. Run Tests Across All Target Frameworks
```bash
dotnet test NeoKolors.slnx -c Debug
```

### 2. Fast Inner-Loop Testing (Single Framework)
Running tests against all target frameworks can be redundant during iterative development. Target a single runtime (`net10.0` or `net8.0`) for fast feedback:
```bash
# Test Common library on net10.0
dotnet test Tests/Common.Tests/NeoKolors.Common.Tests.csproj -f net10.0

# Test TUI Core on net10.0
dotnet test Tests/Tui.Tests/Core/NeoKolors.Tui.Core.Tests.csproj -f net10.0

# Test TUI Elements on net10.0
dotnet test Tests/Tui.Tests/Elements/NeoKolors.Tui.Elements.Tests.csproj -f net10.0

# Test Extensions on net10.0
dotnet test Tests/Extensions.Tests/NeoKolors.Extensions.Tests.csproj -f net10.0
```

### 3. Filter Specific Tests
```bash
# Run specific test class
dotnet test Tests/Common.Tests/NeoKolors.Common.Tests.csproj -f net10.0 --filter "FullyQualifiedName~NKColorTests"

# Run specific test method
dotnet test Tests/Common.Tests/NeoKolors.Common.Tests.csproj -f net10.0 --filter "Name=RgbColor_ShouldMatchExpectedBytes"
```

## Troubleshooting Build & Test Issues

1. **Missing .NET Preview SDK**:
   If `net11.0` fails due to SDK availability, check installed SDKs:
   ```bash
   dotnet --list-sdks
   ```
   Ensure `global.json` permits roll-forward (`"rollForward": "latestMajor"`, `"allowPrerelease": true`).
2. **MinVer Tag Resolution**:
   MinVer computes versions from git tags (`v2026.3...`). If git history is shallow, MinVer falls back to `2026.3.0-preview.0`.
3. **Double Check Warnings**:
   Check for warnings like `CS8763` (marked `[DoesNotReturn]` returning) or `CS1956` (interface ambiguity) when adding new methods or modifying classes.
