---
name: zero-alloc-perf
description: >-
  Use this skill when designing or refactoring performance-critical code in NeoKolors,
  such as ANSI string processing, console character buffer rendering, screen diffing,
  hot loops, or layout calculations to ensure zero unnecessary heap allocations.
---

# Zero-Allocation & High-Performance Engineering Guide

NeoKolors is engineered for sub-millisecond, low-latency, zero-flicker terminal rendering. Hot paths must adhere to strict zero-allocation and cache-friendly design.

## Core Rules for Hot Paths

### 1. Zero Allocations in Render & Diff Loops
- **Screen Diffing (`NKCharScreen`)**: Comparing previous frame characters/styles to the current buffer must not allocate any heap objects per character.
- **Canvas Operations (`NKCharCanvas`)**: Drawing operations (lines, rectangles, characters, text) mutate existing pre-allocated buffers. Never re-allocate the screen buffer during active frames.
- **Event Dispatching**: Mouse and keyboard dispatching loops (`ElementsMouseDispatcher`, `HitMap`) should reuse event parameter structs or cached handlers.

### 2. Slicing and Text Processing
- **Use `ReadOnlySpan<char>`**: Never call `.Substring()` or `.Split()` on strings in layout or rendering code.
- **Use `AnsiString` / `AnsiStringBuilder`**: For ANSI sequences, leverage pre-sized builders and pooled buffers instead of string concatenation (`+`).
- **Word Wrapping & Chopping**: Leverage `NeoKolors.Extensions` (`Chop`) which calculates and segments words efficiently.

### 3. Struct Design & Memory Footprint
- **`NKColor`**: 32-bit compact value type (`uint`). Do not box `NKColor` into `object` or `ValueType`.
- **`NKStyle`**: Explicit 128-bit memory layout containing foreground color, background color, and bitpacked style flags (bold, underline, italic, etc.). Keep it as a pure value type.
- Avoid passing large structs by value if modified frequently; use `in`, `ref`, or `readonly ref` where appropriate.

### 4. Avoiding Hidden Allocations
- **No LINQ in Loops**: Replace `.Where()`, `.Select()`, `.ToList()`, and `.Any()` with plain `for` / `foreach` loops on structs or indexed arrays.
- **No Closures / Lambdas in Hot Paths**: Anonymous delegates capture variables on the heap. Pass explicit state or use static lambdas if required.
- **Avoid Boxing**: Watch out for `Enum.HasFlag()`, string formatting (`$"Value: {x}"`), or interface calls on structs without constraints.

### 5. Profiling & Diagnostics
- Check compile-time profiling flag `NK_RENDERING_PROFILING`:
  - When enabled, `NKCharScreen` and lifecycle controllers track frame time, diff time, and render operations.
  - Profile using built-in FPS metrics or DotMemory / BenchmarkDotNet before and after critical rendering changes.
