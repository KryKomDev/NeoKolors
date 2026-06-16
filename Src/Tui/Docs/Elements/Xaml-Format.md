# NeoKolors XAML Format Reference

NeoKolors features a declarative, XML-based XAML (Extensible Application Markup Language) 
parser implemented in [XamlElementLoader](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Dom/XamlElementLoader.cs). This format allows you to design rich terminal user interfaces (TUIs) declaratively, separating view layouts from backing logic.

---

## 1. Document Structure & Setup

A NeoKolors XAML file represents a single UI visual tree hierarchy. It typically begins with a container root node (like a `Page` or `Grid`) and maps to a corresponding partial code-behind class.

### Example Root Declaration
```xml
<Page x:Class="DashBoard.MainView" 
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
    <Grid>
        <!-- Layout contents go here -->
    </Grid>
</Page>
```

### Key Directives
* **`x:Class`**: Mapped target code-behind class that defines the backing logic. During bootstrapping, this tells the parser which backing class instance handles runtime events.
* **`x:Name` / `Name` / `Id`**: Registers a unique identifier for the control. Elements with IDs can be referenced programmatically in code-behind or targeted by relative panel layout rules.

---

## 2. Element Registry (Supported Controls)

The following element tags are registered by default in the parser:

| Category             | XAML Control    | backing Class Link                                                                                          | Description                                              |
|----------------------|-----------------|-------------------------------------------------------------------------------------------------------------|----------------------------------------------------------|
| **Layout Panels**    | `Grid`          | [Grid](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/Grid.cs)                   | Multi-row, multi-column layout grid                      |
|                      | `StackPanel`    | [StackPanel](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/StackPanel.cs)       | Arranges children sequentially in a single row or column |
|                      | `RelativePanel` | [RelativePanel](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/RelativePanel.cs) | Arranges children relative to each other or the panel    |
|                      | `Canvas`        | [Canvas](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/Canvas.cs)               | Coordinates-based absolute layout canvas                 |
| **Content Controls** | `Page`          | [Page](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/Page.cs)                   | The root window container page                           |
|                      | `GroupBox`      | [GroupBox](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/GroupBox.cs)           | Labeled border framing control                           |
|                      | `ScrollViewer`  | [ScrollViewer](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/ScrollViewer.cs)   | Scrollable container control                             |
|                      | `Expander`      | [Expander](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/Expander.cs)           | Collapsible/expandable content drawer                    |
| **Buttons & Input**  | `Button`        | [Button](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/Button.cs)               | Clickable command trigger button                         |
|                      | `TextBox`       | [TextBox](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/TextBox.cs)             | Editable plaintext input box                             |
|                      | `PasswordBox`   | [PasswordBox](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/PasswordBox.cs)     | Masked secure input box                                  |
|                      | `CheckBox`      | [CheckBox](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/CheckBox.cs)           | Selectable toggle checkbox control                       |
|                      | `RadioButton`   | [RadioButton](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/RadioButton.cs)     | Exclusive toggle choice button                           |
| **Visual & Media**   | `TextBlock`     | [TextBlock](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/TextBlock.cs)         | Read-only styled text block                              |
|                      | `AsciiImage`    | [AsciiImage](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/AsciiImage.cs)       | Rendered ASCII art image control                         |
|                      | `SixelImage`    | [SixelImage](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/Elements/SixelImage.cs)       | Hardware sixel graphics graphic control                  |

---

## 3. Styling & Presentation Properties

Properties can be styled either directly via XML attributes on control elements or within `<Setter>` tags.

### Standard Styling Attributes

| Attribute Name         | Value Format                                                                 | Description                                            |
|------------------------|------------------------------------------------------------------------------|--------------------------------------------------------|
| **`BackgroundColor`**  | Named ANSI color (e.g. `red`, `dark-grey`, `dark-blue`, `magenta`)           | Background color of the control                        |
| **`TextColor`**        | Named ANSI color (e.g. `yellow`, `cyan`, `white`, `green`)                   | Text or foreground color of the control                |
| **`Border`**           | `None`, `Normal`, `Rounded`, `Double`                                        | Frame border style of the control                      |
| **`Margin`**           | Single value `[all]`, pair `[horiz vert]`, or quad `[left top right bottom]` | Spacing buffer outside control bounds (e.g. `1 0 1 0`) |
| **`Padding`**          | Single value `[all]`, pair `[horiz vert]`, or quad `[left top right bottom]` | Spacing buffer inside control bounds (e.g. `2 1`)      |
| **`Width` / `Height`** | Numeric layout metric (e.g. `20`, `25ch`, `10%`)                             | Explicit dimensions (see Layout Metrics section)       |
| **`HorizontalAlign`**  | `Left`, `Center`, `Right`, `Stretch`                                         | Horizontal content alignment                           |
| **`VerticalAlign`**    | `Top`, `Center`, `Bottom`, `Stretch`                                         | Vertical content alignment                             |
| **`Visible`**          | `True`, `False`                                                              | Visibility status                                      |
| **`Font`**             | Font Name (e.g. `Dummy`, `Bytesized`, `Future`)                              | Selects ASCII font asset to render the control text    |

---

## 4. Layout Metrics & Units

Unlike standard pixel layouts, NeoKolors calculates layout geometry in character cells. Supported units parsed by the layout engine (described in [Dimenstions.md](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Tui/md/Dimenstions.md)) include:

* **Character cells (`ch`)**: Explicit grid units. E.g. `Width="25ch"`. (If no unit suffix is defined, `ch` is assumed).
* **Percentage (`%`)**: Dimensions relative to parent boundary bounds. E.g. `Height="50%"`.
* **Viewport Metrics (`vw` / `vh` / `vmin` / `vmax`)**: Size bounds relative to the current 
  terminal screen size.
* **Special Metrics**:
    * `auto`: Automatic layout determination based on contents.
    * `min-content`: Minimum spacing required to contain the element content.
    * `max-content`: Maximum spacing requested by the element content.

---

## 5. Panel-Specific Attached Properties

Certain layout panels locate and scale child elements through attached attributes prefixing
the parent container class.

### 5.1 Grid Properties
A `Grid` determines row and column bounds using attached integer and span values:
* **`Grid.Row`**: The row index (0-indexed) where the control resides.
* **`Grid.Column`**: The column index (0-indexed) where the control resides.
* **`Grid.RowSpan`**: The number of grid rows the control spans vertically.
* **`Grid.ColumnSpan`**: The number of grid columns the control spans horizontally.

*Example:*
```xml
<TextBlock Grid.Row="1" Grid.Column="0" Grid.ColumnSpan="2" Content="Spanning Footer" />
```

### 5.2 RelativePanel Properties
A `RelativePanel` positions child controls relative to siblings using their ID coordinates:
* **`RelativePanel.RightOf`**: Places the control directly to the right of the target element ID.
* **`RelativePanel.Below`**: Places the control directly below the target element ID.
* **`RelativePanel.LeftOf`**: Places the control directly to the left of the target element ID.
* **`RelativePanel.Above`**: Places the control directly above the target element ID.

*Example:*
```xml
<TextBox x:Name="SearchBox" Width="20" />
<Button Content="Submit" RelativePanel.RightOf="SearchBox" />
```

---

## 6. Styling Triggers & Visual States

To support dynamic interface reactions (like mouse hovers or keyboard focus), elements support inline styling
triggers. Triggers evaluate runtime state values (using `IsHovered`, `IsFocused`, etc.) and dynamically overlay
overriding setters.

### Trigger Schema
Triggers are nested within the `<ControlName.Triggers>` block:
* `<Trigger Property="[StateProperty]" Value="[StateValue]">`: Defines target trigger criteria.
* `<Trigger.Setters>`: Contains one or more `<Setter>` declarations applied while the trigger remains active.

### Trigger Demo Example
```xml
<Button Content="Interactive Button" Border="Normal">
    <Button.Triggers>
        <!-- Change border and background on hover -->
        <Trigger Property="IsHovered" Value="True">
            <Trigger.Setters>
                <Setter Property="BackgroundColor" Value="dark-blue" />
                <Setter Property="TextColor" Value="cyan" />
                <Setter Property="Border" Value="Rounded" />
            </Trigger.Setters>
        </Trigger>
        
        <!-- Shift style on keyboard focus -->
        <Trigger Property="IsFocused" Value="True">
            <Trigger.Setters>
                <Setter Property="BackgroundColor" Value="dark-magenta" />
            </Trigger.Setters>
        </Trigger>
    </Button.Triggers>
</Button>
```

---

## 7. Event & Code-Behind Binding

Interactive events on controls are declared inside XAML markup and dynamically wired up to
backing logic in your mapped code-behind class (indicated by the root `x:Class` target).

### Markup Binding
```xml
<Button Content="Click Here" OnClick="OnButtonClicked" />
```

### Backing Code-Behind Handler
In the corresponding partial class:
```csharp
namespace DashBoard;

public partial class MainView {
    // Mapped handler method is resolved by name (case-insensitive)
    public void OnButtonClicked() {
        NKDebug.Debug("Button was clicked!");
    }
}
```

The loader automatically matches signature and invokes delegate registration using 
reflection (handling standard events or custom handlers bound to backing structures).
