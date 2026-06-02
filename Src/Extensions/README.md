# NeoKolors.Extensions

`NeoKolors.Extensions` is a high-performance productivity library providing utility extensions for standard .NET and C# types. Optimized for performance and readability, it utilizes modern C# language features (such as C# 14 Extension Types) to clean up codebases.

---

## Key Features

- **String Manipulation**:
  - Word Wrapping & Chopping ([String.cs](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Extensions/String.cs)): Chop strings into lines of maximum length without breaking words (`Chop`).
  - Text Visual Length: Calculate visible length of text by stripping ANSI escape sequences (`GetPlainLength`).
  - Casing Transformations: Methods for capitalizing sentences or decapitalizing initial characters (`Capitalize`, `CapitalizeFirst`).
- **Case Conversions (`NamingCase`)**:
  - Convert strings between standard programming cases via [UniversalNamingConvertor](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Extensions/UniversalNamingConvertor.cs):
    - `CAMEL` (e.g. `helloWorld`)
    - `PASCAL` (e.g. `HelloWorld`)
    - `SNAKE` (e.g. `hello_world`)
    - `SCREAMING_SNAKE` (e.g. `HELLO_WORLD`)
    - `KEBAB` (e.g. `hello-world`)
    - `TRAIN` (e.g. `HELLO-WORLD`)
- **Collection & Grid Enhancements**:
  - Provides extension methods for collections and two-dimensional arrays ([Enumerable.cs](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Extensions/Enumerable.cs)), simplifying spatial grid indexing and manipulations in the TUI screen space.
- **Console Input Helpers**:
  - Adds extensions to `ConsoleKeyInfo` ([ConsoleKeyInfo.cs](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Extensions/ConsoleKeyInfo.cs)) to ease mapping between keystrokes and custom TUI actions.

---

## Core Usage

### 1. Word Wrapping (Chop)
Wrap text lines while respecting word boundaries:

```csharp
using NeoKolors.Extensions;

string longParagraph = "Lorem ipsum dolor sit amet, consetetur sadipscing elitr...";

// Chops the text to lines of maximum 20 characters
string[] lines = longParagraph.Chop(20);
```

### 2. Stripping ANSI Codes
Measure the printable character length of styled ANSI strings:

```csharp
using NeoKolors.Extensions;

string coloredText = "\u001b[31mBoldRed\u001b[0m";
int visualLength = coloredText.GetPlainLength(); // Returns 7
```

### 3. Naming Conversions
Convert names dynamically across case schemes:

```csharp
using NeoKolors.Extensions;

string snakeCase = "user_profile_photo";

// Convert to PascalCase (returns "UserProfilePhoto")
string pascal = snakeCase.ToCase(NamingCase.PASCAL);

// Convert to spinal/kebab-case (returns "user-profile-photo")
string kebab = snakeCase.ToCase(NamingCase.KEBAB);
```