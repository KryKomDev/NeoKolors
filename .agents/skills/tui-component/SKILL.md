---
name: tui-component
description: >-
  Use this skill when creating, modifying, or testing declarative TUI elements,
  widgets, or layouts in NeoKolors.Tui.Elements or NeoKolors.Tui.Widgets.
---

# TUI Component Development Guide

This skill guides you through implementing and extending declarative Text User Interface (TUI) components in NeoKolors.

## Core Component Contracts

All UI components implement `IElement`:
```csharp
using Metriks;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

public interface IElement {
    Size2D Measure(LayoutContext context);
    void Render(RenderContext context, Size2D parent);
}
```

If the component requires frame/time-based animations or periodic transitions, also implement `ITickableElement`:
```csharp
public interface ITickableElement {
    void Tick(TimeSpan delta);
}
```

## Key Architectural Patterns

### 1. Declarative & Reactive Properties with `Bindable<T>`
Components expose reactive properties via `Bindable<T>` (using `Signal<T>` under the hood):
```csharp
public class MyElement : IElement {
    public Bindable<string> Text { get; init; } = "";
    public Bindable<NKStyle?> Style { get; init; } = default;
    public Bindable<Spacing> Margin { get; init; } = default;
    public Bindable<Spacing> Padding { get; init; } = default;
}
```

### 2. Measuring Phase (`Measure`)
- Compute the desired width and height (`Size2D`) given `LayoutContext.AvailableSize`.
- Consider margins, padding, and inner children sizes using `Dimension` and `Metriks` helpers.
- Never write to canvas or mutate external state during `Measure`.

### 3. Rendering Phase (`Render`)
- Access `RenderContext.Canvas` to draw into the character buffer:
  - `context.Canvas.DrawChar(x, y, ch, style)`
  - `context.Canvas.DrawText(x, y, text, style)`
  - `context.Canvas.DrawRect(x, y, width, height, style)`
- Clip regions appropriately with `context.ClipRect`.
- For interactive elements, register mouse interaction bounds with `context.RegisterMouseRegion(...)` or the `HitMap`.

### 4. Layout Containers
- **`Flex`**: Direction (`Row`, `Column`), `JustifyContent`, `AlignItems`, `Wrap`.
- **`Grid`**: Row/Column definitions using auto and star (`*`) sizing.
- **`Border`**: Wraps content with border styles (`NKBorderStyle.GetRounded()`, `Double`, `Thick`, `Inset`).
- **`Table`**: Column headers, cells, and alignment.

## Unit Testing Components
Test component measuring and rendering against an in-memory `NKCharCanvas`:
1. Instantiate element with desired properties.
2. Call `element.Measure(new LayoutContext(availableSize))`.
3. Call `element.Render(renderContext, measuredSize)`.
4. Assert characters and styles at specific coordinates in `canvas`.
See examples in `Tests/Tui.Tests/Elements/`.
