# NeoKolors TUI Overview

The `NeoKolors.Tui` package is a state-driven, component-oriented Text User Interface (TUI) framework. It borrows design principles from modern desktop UI frameworks (like UWP/WPF) to replace legacy HTML-like DOM nodes with a typed control hierarchy, structured star-sizing layout system, visual states, and routed events.

---

## 1. Key Concepts

### 1.1 The Control Hierarchy
Instead of unstructured divs or text tags, UI screens are assembled from stateful controls inheriting from **[Control\<T\>](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Docs/Elements/Type-Hierarchy.md)**:
* **Layout Panels**: `Grid`, `StackPanel`, `Canvas`, and `RelativePanel`.
* **Content Controls**: `Page`, `GroupBox`, `ScrollViewer`, `Button`, `CheckBox`.
* **Text & Images**: `TextBlock`, `SixelImage`, `AsciiImage`.
* **Items Presenters**: `ListView`, `ComboBox`, `TreeView`.
* **Input Elements**: `TextBox`, `PasswordBox`.

### 1.2 Layout Computation
Layout resolution runs in three distinct computation phases:
1. **Measure**: Controls recursively calculate their desired size boundaries based on parent constraints.
2. **Arrange**: Controls are positioned inside specific cell coordinates.
3. **Render**: The computed layout tree is drawn onto the off-screen `NKCharCanvas` grid.

---

## 2. Visual States & Styling

Styling uses a priority-based property resolution engine:
* **Style Collection**: Manages layout margins, paddings, ZIndex, and text/background color properties.
* **Visual State Manager**: Listens to control interaction flags (e.g. `PointerOver`, `Focused`, `Disabled`) and applies dynamic visual state triggers.

---

## 3. Application Lifecycle Bootstrapping

Applications are launched by passing a root view/page to **[NKApplication](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/NKApplication.cs)**:

```csharp
using NeoKolors.Tui;
using NeoKolors.Tui.Elements;

var config = new NKAppConfig();
var mainPage = new Page {
    Content = new TextBlock { Text = "Hello NeoKolors TUI!" }
};

var app = new NKApplication(config, mainPage);
app.Start();
```
