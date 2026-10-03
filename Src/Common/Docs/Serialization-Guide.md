# Binary Serialization & Class Update Guide

This guide explains how binary serialization works across NeoKolors using **Protobuf** (via
`protobuf-net`) and provides step-by-step instructions for updating core classes—such as
[`NKStyle`](../NKStyle.cs), [`NKColor`](../NKColor.cs), [`AnsiString`](../AnsiString.cs),
[`NKLogRecord`](../../Console/Logger/NKLogRecord.cs), and [`NKFont`](../../Tui/Fonts/NKFont.cs)
— without breaking serialization formats.

---

## 1. Overview & Architecture

NeoKolors uses a code-first Protobuf approach implemented with [
`protobuf-net`](https://github.com/protobuf-net/protobuf-net). This design:

- Requires no external `.proto` files or build-time compilation steps.
- Uses **surrogates** to cleanly decouple bit-packed, explicit memory layout structs from
  serialization concerns.
- Enables core types ([`NKStyle`](../NKStyle.cs), [`NKColor`](../NKColor.cs)) to be embedded
  directly as fields in other Protobuf contracts across the codebase.

### Serialization Map

```
┌────────────────────────────────────────────────────────┐
│                   NeoKolors.Common                     │
│  NKColor  ──>  NKColorSurrogate  (Raw uint)            │
│  NKStyle  ──>  NKStyleSurrogate  (Raw0, Raw1 ulongs)   │
│  AnsiString  ──>  AnsiStringSerializer                 │
└──────────────────────────┬─────────────────────────────┘
                           │
             ┌─────────────┴─────────────┐
             ▼                           ▼
┌─────────────────────────┐ ┌─────────────────────────┐
│    NeoKolors.Console    │ │   NeoKolors.Tui.Fonts   │
│  NKLogRecordSerializer  │ │  NKFontSerializer       │
│  (Length-Prefixed Log)  │ │  (Magic + Version + DTO)│
└─────────────────────────┘ └─────────────────────────┘
```

---

## 2. Updating `NKColor`

[`NKColor`](../../Common/NKColor.cs) is a
32-bit explicit layout struct (`uint _value`).

### Serialization Rule

The class defines its own Protobuf surrogate:

```csharp
[ProtoContract(Surrogate = typeof(NKColorSurrogate))]
[StructLayout(LayoutKind.Explicit, Size = sizeof(uint))]
public readonly record struct NKColor : IFormattable, IParsablePolyfill.IParsable<NKColor> { ... }

[ProtoContract]
public struct NKColorSurrogate {
    [ProtoMember(1)] public uint Value { get; set; }

    public static implicit operator NKColorSurrogate(NKColor color) => new() { Value = color.GetRaw() };
    public static implicit operator NKColor(NKColorSurrogate surrogate) => NKColor.FromRaw(surrogate.Value);
}
```

### How to Update `NKColor`

1. **Preserving 32-bit packing**:
    - The lower 24 bits store RGB or console color index.
    - The upper 8 bits store [`ColorType`](../../Common/NKColor.cs).
2. **If adding new color modes**:
    - Add new enum values to `ColorType` (must fit within the 8-bit space).
    - Because serialization records `uint Value` via `GetRaw()` / `FromRaw()`, no changes to
      `NKColorSurrogate` are needed.
3. **If expanding beyond 32 bits**:
    - Add a new field tag to `NKColorSurrogate` (e.g.
      `[ProtoMember(2)] public uint ExtendedData { get; set; }`).
    - Never change the meaning or type of `[ProtoMember(1)]`.
4. **Verification**:
    - Run tests:
      `dotnet test Tests/Common.Tests/NeoKolors.Common.Tests.csproj --filter NKColorTests`.

---

## 3. Updating `NKStyle`

[`NKStyle`](../../Common/NKStyle.cs) is a
128-bit explicit layout struct composed of two 64-bit integers: `_raw0` and `_raw1`.

### Serialization Rule

`NKStyle` defines its own Protobuf surrogate:

```csharp
[ProtoContract(Surrogate = typeof(NKStyleSurrogate))]
[StructLayout(LayoutKind.Explicit, Size = sizeof(ulong) * 2)]
public readonly record struct NKStyle : IFormattable, IParsablePolyfill.IParsable<NKStyle> { ... }

[ProtoContract]
public struct NKStyleSurrogate {
    [ProtoMember(1)] public ulong Raw0 { get; set; }
    [ProtoMember(2)] public ulong Raw1 { get; set; }

    public static implicit operator NKStyleSurrogate(NKStyle style) => new() { Raw0 = style.Raw0, Raw1 = style.Raw1 };
    public static implicit operator NKStyle(NKStyleSurrogate surrogate) => new(surrogate.Raw0, surrogate.Raw1);
}
```

### How to Update `NKStyle`

1. **Adding style properties or flags within 128 bits**:
    - `Raw0` (bits 0–63): Foreground color (`FColor`), Background color (`BColor`).
    - `Raw1` (bits 64–127): Text styles (`Styles`), Inherited styles (`InheritedStyles`), Underline
      (`Underline`).
    - If adjusting bit positions or adding flags within the two 64-bit words, `NKStyleSurrogate`
      already serializes both `Raw0` and `Raw1`.
2. **Using `NKStyle` in other contracts**:
    - You can use `NKStyle` directly as a field in any Protobuf class:
      ```csharp
      [ProtoMember(2)]
      public NKStyle Style { get; set; }
      ```
    - Do **not** unpack `Raw0` and `Raw1` manually in higher-level serializers.
3. **If expanding beyond 128 bits**:
    - Add `[ProtoMember(3)] public ulong Raw2 { get; set; }` to `NKStyleSurrogate`.
    - Update implicit conversion operators in `NKStyleSurrogate`.
4. **Verification**:
    - Run tests:
      `dotnet test Tests/Common.Tests/NeoKolors.Common.Tests.csproj --filter NKStyleTests`.

---

## 4. Updating `AnsiString` & `AnsiStringSerializer`

[
`AnsiStringSerializer`](../../Common/AnsiStringSerializer.cs)
serializes styled text to/from Protobuf byte arrays or streams.

### Contract Definition

```csharp
[ProtoContract]
public class ProtoAnsiStringPayload {
    [ProtoMember(1)] public string Plain { get; set; } = string.Empty;
    [ProtoMember(2)] public List<ProtoStyleMarker> Markers { get; set; } = [];
}

[ProtoContract]
public struct ProtoStyleMarker {
    [ProtoMember(1)] public int Index { get; set; }
    [ProtoMember(2)] public NKStyle Style { get; set; }
}

[ProtoContract]
public class ProtoNullableAnsiString {
    [ProtoMember(1)] public bool HasValue { get; set; }
    [ProtoMember(2)] public ProtoAnsiStringPayload? Payload { get; set; }
}
```

### Guidelines

- Notice that `ProtoStyleMarker` embeds `NKStyle Style` directly, leveraging `NKStyleSurrogate`.
- When adding metadata to `AnsiString` (e.g. hyperlinks, annotations), add new `[ProtoMember(N)]`
  tags to `ProtoAnsiStringPayload`.
- Run tests:
  `dotnet test Tests/Common.Tests/NeoKolors.Common.Tests.csproj --filter AnsiStringSerializerTests`.

---

## 5. Updating Binary Logging (`NKLogRecordSerializer`)

[`NKLogRecordSerializer`](../../Console/Logger/NKLogRecordSerializer.cs)
manages serialization of log records (`.blog` files).

### Streaming & Delimitation

Because logs are written continuously to a stream (e.g., via [
`BinaryLogWriter`](../../Console/Logger/BinaryLogWriter.cs)),
messages **must** be length-prefixed:

```csharp
// Writing:
Serializer.SerializeWithLengthPrefix(stream, proto, PrefixStyle.Base128);

// Reading:
var proto = Serializer.DeserializeWithLengthPrefix<ProtoNKLogRecord>(stream, PrefixStyle.Base128);
```

### How to Update `NKLogRecord`

1. When adding new fields (e.g. process ID, thread ID, tags):
    - Add new `[ProtoMember(N)]` tags to `ProtoNKLogRecord` using the next incremental number (do
      not renumber 1 through 5).
2. If extending exception logging:
    - Modify `ProtoExceptionInfo` and
      [`DeserializedException`](../../Console/Logger/NKLogRecordSerializer.cs).
3. Run tests:
   `dotnet test Tests/Console.Tests/NeoKolors.Console.Tests.csproj --filter NKBinaryLoggerTests`.

---

## 6. Updating Font Binary Serialization (`NKFontSerializer.Binary`)

[`NKFontSerializer.Binary.cs`](../../Tui/Fonts/Serialization/NKFontSerializer.Binary.cs)
serializes compiled fonts (`.nkf`).

### Header Specification

Every compiled `.nkf` file starts with a 5-byte header:

1. **Magic Number** (4 bytes): `0x4E4B4642` (ASCII `"NKFB"`).
2. **Version** (1 byte): `BINARY_VERSION` (currently `3`).

Followed immediately by the Protobuf payload (`ProtoNKFont`).

### How to Update Font Schemas

1. **When making non-breaking additions** (e.g. new optional metadata field on
   [`NKFontInfo`](../../Tui/Fonts/NKFontInfo.cs)):
    - Add a new `[ProtoMember(N)]` tag to `ProtoNKFontInfo` or `ProtoNKFont`.
    - Update `ToProto` and `ToDomain` mapping methods.
2. **When making breaking changes**:
    - Increment `BINARY_VERSION` in `NKFontSerializer.Binary.cs` (e.g. from `3` to `4`).
3. **Recompiling Builtin Font Assets**:
    - Run MSBuild on the assets project:
      ```powershell
      dotnet build Src/Tui/Fonts/Assets/NeoKolors.Tui.Fonts.Assets.csproj
      ```
    - This automatically re-runs
      [`CompileFontsTask`](../../Tui/Fonts/Build/CompileFontsTask.cs),
      regenerating `Bytesized.nkf`, `Future.nkf`, and `Dummy.nkf`.
4. **Verification**:
    - Run tests:
      `dotnet test Tests/Tui.Tests/NeoKolors.Tui.Tests.csproj --filter NKFontBinarySerializerTests`.

---

## 7. General Protobuf Rules & Best Practices

1. **Tag Numbers are Permanent**:
    - Never change an existing `[ProtoMember(N)]` tag number.
    - Never reorder tag numbers.
    - If a field is deprecated, leave the tag number unused.
2. **Boolean Defaults Gotcha**:
    - In Protobuf, the default value for `bool` is `false` (which is omitted from the wire format).
    - If a boolean property should default to `true` (such as `RenderStrikethroughAboveLetters`),
      you must annotate it with `[System.ComponentModel.DefaultValue(true)]`.
3. **Use Surrogates for Structs**:
    - Structs with explicit memory layouts or complex constructors should always use
      `[ProtoContract(Surrogate = typeof(...))]`.
    - Surrogates keep domain structs immutable and clean while giving Protobuf a simple, mutable
      DTO.
4. **Verification Checklist**:
    - Run all test suites across the solution after making serialization changes:
      ```powershell
      dotnet test Tests/Common.Tests/NeoKolors.Common.Tests.csproj
      dotnet test Tests/Console.Tests/NeoKolors.Console.Tests.csproj
      dotnet test Tests/Tui.Tests/NeoKolors.Tui.Tests.csproj --filter "FullyQualifiedName~Fonts"
      ```
