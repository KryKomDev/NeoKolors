# NeoKolors Extensions Overview

The `NeoKolors.Extensions` library provides high-performance utility extension methods on top of standard .NET types. These extensions are heavily optimized for string manipulation, case conversion, keyboard-to-action bindings, and spatial calculations.

---

## 1. String & ANSI Extensions

String manipulation is crucial in text user interfaces (TUI) where ANSI colors and formatting must not break character position alignments.

### 1.1 Word Wrapping with `Chop`
The `Chop` extension wraps long paragraphs of text into lines of a maximum length while preserving word boundaries.

```csharp
using NeoKolors.Extensions;

string text = "NeoKolors provides premium tools for terminal user interfaces.";
string[] wrappedLines = text.Chop(maxLineLength: 20);
// Returns lines bounded to 20 chars without splitting individual words
```

### 1.2 Measuring Plain Length
When strings contain ANSI escape sequences (e.g. `\u001b[31;1mHello\u001b[0m`), calling `Length` returns the code sequence characters along with the visible ones. The `GetPlainLength` method strips away formatting sequences to return the actual visible string length.

```csharp
using NeoKolors.Extensions;

string styledString = "\u001b[38;2;255;0;0mRedText\u001b[0m";
int visualLength = styledString.GetPlainLength(); // Returns 7
```

---

## 2. Naming Case Conversions

The library provides case transformation helpers using the **[UniversalNamingConvertor](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Extensions/UniversalNamingConvertor.cs)** class.

### 2.1 Supported Cases (`NamingCase`)

* `CAMEL`: `myVariableName`
* `PASCAL`: `MyVariableName`
* `SNAKE`: `my_variable_name`
* `SCREAMING_SNAKE`: `MY_VARIABLE_NAME`
* `KEBAB`: `my-variable-name`
* `TRAIN`: `MY-VARIABLE-NAME`

### 2.2 Usage
```csharp
using NeoKolors.Extensions;

string original = "user_profile_data";

// Convert snake_case to PascalCase
string pascal = original.ToCase(NamingCase.PASCAL); // "UserProfileData"

// Convert snake_case to kebab-case
string kebab = original.ToCase(NamingCase.KEBAB); // "user-profile-data"
```

---

## 3. Keyboard & Collection Extensions

### 3.1 `ConsoleKeyInfo` Mapping Helpers
Simplify keyboard input handling inside TUI update loops by inspecting key states:

```csharp
using System;
using NeoKolors.Extensions;

ConsoleKeyInfo keyInfo = Console.ReadKey(true);

if (keyInfo.IsCtrl(ConsoleKey.C)) {
    // Handle cancel operation
}
```

### 3.2 Collection Grid Translations
Provides extensions for two-dimensional structures, facilitating viewport and coordinates mapping on the TUI canvas:

```csharp
using NeoKolors.Extensions;

// Index helpers for 2D grids, conversion between flattened and coordinates arrays
```
