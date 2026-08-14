# String Extensions

The **[String](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Extensions/String.cs)** extension class under namespace `NeoKolors.Extensions` provides high-performance methods for string operations, formatting, word wrapping, and conversion helpers tailored for TUI environments.

---

## 1. Extension Methods Reference

### 1.1 `GetPlainLength()`
Returns the total count of the printable / visible characters contained by the string, stripping out all ANSI style escape sequences. Use this to determine cell alignment bounds on grid views.
```csharp
string styled = "\e[31mHello\e[0m";
int visualLen = styled.GetPlainLength(); // Returns 5
```

### 1.2 `Chop(int maxLength)`
Splits a paragraph string into multiple rows, wrapping them so that each row is at most `maxLength` characters wide. This wraps on word boundaries to keep words intact.
```csharp
string text = "NeoKolors provides TUI utilities.";
string[] lines = text.Chop(15);
```

### 1.3 Case Formatting
- **`Capitalize()`**: Capitalizes all words in the string.
- **`CapitalizeFirst(CultureInfo? cultureInfo = null)`**: Capitalizes the first character of the string.
- **`DecapitalizeFirst(CultureInfo? cultureInfo = null)`**: Decapitalizes the first character of the string.

### 1.4 `Format(params object[] args)`
Formats the composite string using `string.Format` with invariant culture defaults.
```csharp
string output = "Result: {0}".Format(42);
```

### 1.5 `InRange(int startIndex, int endIndex)`
Extracts a substring from `startIndex` (inclusive) to `endIndex` (exclusive).

### 1.6 `ToRoman(bool lowercase = false)`
Converts an integer (value 1 to 3999) to its Roman numeral representation.
```csharp
string roman = 1994.ToRoman(); // Returns "MCMXCIV"
```
