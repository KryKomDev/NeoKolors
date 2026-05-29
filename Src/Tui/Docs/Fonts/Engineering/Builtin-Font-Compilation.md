+++
Written for NeoKolors.Tui.Fonts version 2026.3 (NKFont V3)
by KryKom with love and care :)
+++

# Built-in Font Compilation and Asset Library Embedding Pipeline

This document explains the architecture, compilation process, and automated build pipeline that compiles the raw XML built-in fonts and embeds them into a dedicated asset assembly (`NeoKolors.Tui.Fonts.Assets`) at compile time.

---

## 1 Architectural Overview

To keep fonts easy to maintain and version-controlled, they are written and stored in an unpackaged XML-based structure. However, parsing XML and loading hundreds of individual glyph configuration files at runtime is slow and memory-intensive.

To solve this, NeoKolors uses a modular asset architecture:
1. **Unpackaged Source (XML V3):** Stored in version control as highly readable XML files and folders inside the `Assets/` directory.
2. **Packaged Binary (MessagePack `.nkf`):** Compiled at build time into an extremely compact, pre-validated binary format using the custom NativeAOT-compliant MessagePack serialization pipeline.
3. **Dedicated Asset Assembly (`NeoKolors.Tui.Fonts.Assets`):** Stored inside a separate project reference, avoiding any compile-time circular dependency.
4. **Dynamic Loading:** Automatically loaded dynamically from the `NeoKolors.Tui.Fonts.Assets` assembly namespace (`NeoKolors.Tui.Fonts.Assets.*.nkf`) at runtime.

### Directory Structure

```
Src/Tui/
├── NeoKolors.Tui.csproj
│
└── Fonts/
    ├── NeoKolors.Tui.Fonts.csproj  <-- Core font loading & rendering library
    │
    ├── Serialization/
    │   └── NKFontSerializer.Xml.V3.cs  <-- Dynamic runtime loader
    │
    └── Assets/                                <-- Root folder for the Assets project and built-in fonts
        ├── Bytesized/                         <-- Source XML definition folder
        ├── Dummy/                             <-- Source XML definition folder
        ├── Future/                            <-- Source XML definition folder
        ├── Bytesized.nkf                      <-- Compiled binary file (Git ignored, auto-generated)
        ├── Dummy.nkf                          <-- Compiled binary file (Git ignored, auto-generated)
        ├── Future.nkf                         <-- Compiled binary file (Git ignored, auto-generated)
        └── NeoKolors.Tui.Fonts.Assets.csproj  <-- Asset project containing inline C# task
```

---

## 2 Inline C# Compiler Task (`CompileFontsTask`)

Instead of utilizing an external compiler console utility that locks dll files, the compilation process has been refactored into a high-performance in-process MSBuild inline C# task (`CompileFontsTask`) utilizing the `RoslynCodeTaskFactory` inside `NeoKolors.Tui.Fonts.Assets.csproj`.

Its responsibilities include:
1. **Dependency Resolution:** Dynamically intercepting assembly resolution via `AppDomain.CurrentDomain.AssemblyResolve` and `AssemblyLoadContext.Resolving` using the `@(ReferencePath)` item group to load package dependencies (such as `Metriks`, `MessagePack`, etc.) in-memory.
2. **Non-Locking Loading:** Reading the core `NeoKolors.Tui.Fonts.dll` assembly via bytes (`File.ReadAllBytes`) rather than assembly paths to completely avoid locking DLLs on disk, ensuring clean parallel and incremental builds.
3. **In-Process XML Compilation:** Reflectively invoking `NKFontSerializer.DeserializeXml` to parse the unpackaged XML files and then `NKFontSerializer.SerializeBinary` to output binary `.nkf` MessagePack files.

---

## 3 MSBuild Automated Pipeline

The fonts must compile automatically on every project run or build. Integrating this into MSBuild requires resolving three major architectural challenges: circular dependencies, build-cache clashing during parallel cross-targeting builds, and file-globbing conflicts.

These are solved in `NeoKolors.Tui.Fonts.Assets.csproj` using the following configuration:

### 3.1 Item Exclusions & Embedded Resources
The source XML folders are excluded from standard compilation, while the compiled binary `.nkf` assets are declared as embedded resources:

```xml
    <ItemGroup>
        <EmbeddedResource Include="Bytesized.nkf" />
        <EmbeddedResource Include="Dummy.nkf" />
        <EmbeddedResource Include="Future.nkf" />
    </ItemGroup>
```

### 3.2 The Compilation Target
We defined a custom target named `CompileBuiltinFonts` that runs in the proper MSBuild lifecycle phase:

```xml
    <Target Name="CompileBuiltinFonts" AfterTargets="ResolveReferences" BeforeTargets="PrepareResources">
        <PropertyGroup>
            <!-- Resolve the path to the referenced NeoKolors.Tui.Fonts.dll assembly -->
            <FontsDllPath>..\bin\$(Configuration)\$(TargetFramework)\NeoKolors.Tui.Fonts.dll</FontsDllPath>
        </PropertyGroup>
        <CompileFontsTask SourceDir="$(ProjectDir)." FontsAssemblyPath="$(FontsDllPath)" References="@(ReferencePath)" />
    </Target>
```

### 3.3 Solving the Key Challenges

#### Challenge A: Circular Build Dependencies
* **Problem:** The font compiler needs the `NeoKolors.Tui.Fonts` assembly to serialize the fonts. However, if the core fonts library depended directly on the compiler or the compiled assets, it would create a circular reference.
* **Solution:** The core library `NeoKolors.Tui.Fonts.csproj` has **zero** compile-time references to `NeoKolors.Tui.Fonts.Assets.csproj`. Instead, the asset library references the core library. At runtime, the core library dynamically resolves the assets from the asset assembly by loading it reflectively, completely breaking any circular reference loop.

#### Challenge B: Concurrency Clashes and Target Lifecycles
* **Problem:** During early build lifecycle phases (such as `BeforeTargets="BeforeBuild"`), the `@(ReferencePath)` item group is empty, meaning package dependencies like `Metriks` cannot be located, causing the Roslyn task to fail.
* **Solution:** The target runs `AfterTargets="ResolveReferences"`. At this stage, all NuGet dependencies have been successfully resolved and are available in the `@(ReferencePath)` item group, allowing the dynamic assembly resolver in `CompileFontsTask` to find and load packages directly in-memory from the local cache.

---

## 4 Serialization and Auto-Compound Validation

When compiling XML fonts into MessagePack format, `NKFontSerializer` enforces strict validation rules to guarantee correct layout and rendering behavior at runtime:

### 4.1 Single Precomposed Character Check
For every `AutoCompound` element (e.g. adding a diacritic accent to a base character), the serializer combines the base and diacritic characters and normalizes the resulting string using `NormalizationForm.FormC` (Canonical Composition):

```csharp
var symbolString = (autoCompound.SecondaryAfterBase 
    ? $"{char.ToCombining(baseSymbol)}{c}" 
    : $"{c}{char.ToCombining(baseSymbol)}")
    .Normalize(NormalizationForm.FormC);

if (symbolString.Length != 1) {
    LOGGER.Error($"The composed symbol '{symbolString}' is not valid for auto-compounds.");
    return null;
}
```

* **Length Verification:** The composed string length **must be exactly 1**. This ensures that the generated glyph corresponds to a valid precomposed Unicode character that can be rendered as a single text cell on the terminal, rather than an unresolvable multi-code-point sequence.

### 4.2 Group Scoping to Prevent Double-Accents
Accents and diacritics should only be automatically compound-generated for unaccented basic characters:
* **Scope Isolation:** AutoCompound rules for accents (`´`) and umlauts (`¨`) are strictly bound to `<ApplicableGroup>basic-vowels</ApplicableGroup>` to isolate them to standard basic vowels (`a`, `e`, etc.), ensuring the precomposition length is always 1 and double-accented characters are prevented.

---

## 5 Loading Fonts at Runtime

Once compiled and embedded, built-in fonts are automatically resolved by `NKFontSerializer.ReadInternal`. 

When a consumer accesses a built-in font, the core library dynamically loads the asset assembly and resolves the embedded resource stream:

```csharp
    internal static NKFont? ReadInternal(string name) {
        try {
            var assembly = Assembly.Load("NeoKolors.Tui.Fonts.Assets");
            return ReadEmbedded($"NeoKolors.Tui.Fonts.Assets.{name}.nkf", assembly);
        } 
        catch (Exception ex) {
            LOGGER.Error($"Failed to load built-in font assembly 'NeoKolors.Tui.Fonts.Assets': {ex.Message}");
            return null;
        }
    }
```

### 5.2 Public Assets Provider API

For consumer applications that reference `NeoKolors.Tui.Fonts.Assets` directly, the `AssetsProvider` class exposes a high-level API to retrieve pre-compiled fonts and their underlying streams in a strongly-typed, zero-reflection manner:

```csharp
namespace NeoKolors.Tui.Fonts.Assets;

public static class AssetsProvider {
    // Strongly-typed access to pre-compiled fonts
    public static NKFont? Bytesized { get; }
    public static NKFont? Future { get; }
    public static NKFont? Dummy { get; }

    // Direct access to embedded asset streams
    public static Stream? GetFontStream(string name);

    // Retrieve or deserialize font dynamically by name
    public static NKFont? GetFont(string name);
}
```

This guarantees **zero filesystem lookups** at runtime and maximum NativeAOT compatibility, while maintaining the flexibility of full automation during development.
