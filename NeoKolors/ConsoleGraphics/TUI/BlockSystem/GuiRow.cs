using NeoKolors.ConsoleGraphics.TUI.Elements;

namespace NeoKolors.ConsoleGraphics.TUI.BlockSystem;

public class GuiRow {
    
    public GuiColumn[]? SubColumns { get; private init; }
    public IGraphicElement[]? Arguments { get; private init; }
    
    public string? Name { get; private set; }
    
    public float HeightRatio { get; internal set; }
    public int Height { get; internal set; }
    public int Width { get; internal set; }
    public int PositionX { get; internal set; }
    public int PositionY { get; internal set; }
    
    public bool VisibleBorder { get; private init; }

    /// <summary>
    /// creates a new row section with name and arguments
    /// </summary>
    /// <param name="name">name of the row section</param>
    /// <param name="height">height ratio</param>
    /// <param name="visible">whether the row's border will be rendered</param>
    /// <param name="arguments">input arguments</param>
    public static GuiRow Build(string name, float height, bool visible = true, params IGraphicElement[] arguments) {
        GuiRow row = new GuiRow {
            Name = name,
            HeightRatio = height,
            Arguments = arguments,
            SubColumns = null,
            VisibleBorder = visible
        };
        
        return row;
    }
    
    /// <summary>
    /// creates a new row section with name and column sections
    /// </summary>
    /// <param name="name">name of the row section</param>
    /// <param name="height">height ratio</param>
    /// <param name="visible">whether the row's border will be rendered</param>
    /// <param name="columns">column sections</param>
    public static GuiRow Build(string name, float height, bool visible = true, params GuiColumn[] columns) {
        GuiRow row = new GuiRow {
            Name = name,
            HeightRatio = height,
            Arguments = null,
            SubColumns = columns,
            VisibleBorder = visible
        };
        
        float[] widths = columns.Select(c => c.WidthRatio).ToArray();
        widths = GuiBuilder.Normalize(widths);
        
        return row;
    }
    
    /// <summary>
    /// creates a new row section with arguments
    /// </summary>
    /// <param name="height">height ratio</param>
    /// <param name="visible">whether the row's border will be rendered</param>
    /// <param name="arguments">input arguments</param>
    public static GuiRow Build(float height, bool visible = true, params IGraphicElement[] arguments) {
        GuiRow row = new GuiRow {
            Name = null,
            HeightRatio = height,
            Arguments = arguments,
            SubColumns = null,
            VisibleBorder = visible
        };
        
        return row;
    }
    
    /// <summary>
    /// creates a new row section with column sections
    /// </summary>
    /// <param name="height">height ratio</param>
    /// <param name="visible">whether the row's border will be rendered</param>
    /// <param name="columns">column sections</param>
    public static GuiRow Build(float height, bool visible = true, params GuiColumn[] columns) {
        GuiRow row = new GuiRow {
            Name = null,
            HeightRatio = height,
            Arguments = null,
            SubColumns = columns,
            VisibleBorder = visible
        };
        
        return row;
    }
}