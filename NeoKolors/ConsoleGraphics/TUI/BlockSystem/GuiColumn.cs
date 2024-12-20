using NeoKolors.ConsoleGraphics.TUI.Elements;
using NeoKolors.ConsoleGraphics.TUI.Exceptions;

namespace NeoKolors.ConsoleGraphics.TUI.BlockSystem;

public class GuiColumn {
    
    public GuiRow[]? SubRows { get; private init; }
    public IGraphicElement[]? Arguments { get; private init; }

    public string? Name { get; private set; }
    
    public float WidthRatio { get; private init; }
    public int Width { get; internal set; }
    public int Height { get; internal set; }
    public int PositionX { get; internal set; }
    public int PositionY { get; internal set; }

    public bool VisibleBorder { get; private init; }

    private int level;

    private GuiColumn(int level) {
        this.level = level + 1;
        if (level > GuiBuilder.MAX_NESTING) throw new GuiNestingTooDeepException(Name);
    }

    /// <summary>
    /// creates a new row section with name and arguments
    /// </summary>
    /// <param name="name">name of the row section</param>
    /// <param name="width">width ratio</param>
    /// <param name="visible">whether the column's border will be visible</param>
    /// <param name="arguments">input arguments</param>
    public GuiColumn Build(string name, float width, bool visible = true, params IGraphicElement[] arguments) {
        GuiColumn column = new GuiColumn(level) {
            Name = name,
            WidthRatio = width,
            Arguments = arguments,
            SubRows = null,
            VisibleBorder = visible
        };
        
        return column;
    }

    /// <summary>
    /// creates a new row section with name and arguments
    /// </summary>
    /// <param name="name">name of the row section</param>
    /// <param name="width">width ratio</param>
    /// <param name="visible">whether the column's border will be visible</param>
    /// <param name="rows">rows</param>
    public GuiColumn Build(string name, float width, bool visible = true, params Func<GuiRow, GuiRow>[] rows) {
        GuiRow[] row = new GuiRow[rows.Length];

        for (int i = 0; i < rows.Length; i++) {
            row[i] = rows[i](new GuiRow());
        }
        
        GuiColumn column = new GuiColumn(level) {
            Name = name,
            WidthRatio = width,
            Arguments = null,
            SubRows = row,
            VisibleBorder = visible
        };
        
        
        float[] heights = new float[column.SubRows.Length];

        for (int i = 0; i < column.SubRows.Length; i++) {
            heights[i] = column.SubRows[i].HeightRatio;
        }
        
        heights = GuiBuilder.Normalize(heights);

        for (int i = 0; i < column.SubRows.Length; i++) {
            column.SubRows[i].HeightRatio = heights[i];
        }
        
        return column;
    }
    
    /// <summary>
    /// creates a new row section with name and arguments
    /// </summary>
    /// <param name="width">width ratio</param>
    /// <param name="visible">whether the column's border will be visible</param>
    /// <param name="arguments">input arguments</param>
    public GuiColumn Build(float width, bool visible = true, params IGraphicElement[] arguments) {
        GuiColumn column = new GuiColumn(level) {
            Name = null,
            WidthRatio = width,
            Arguments = arguments,
            SubRows = null,
            VisibleBorder = visible
        };
        
        return column;
    }

    /// <summary>
    /// creates a new row section with name and arguments
    /// </summary>
    /// <param name="width">width ratio</param>
    /// <param name="visible">whether the column's border will be visible</param>
    /// <param name="rows">rows</param>
    public GuiColumn Build(float width, bool visible = true, params Func<GuiRow, GuiRow>[] rows) {
        GuiRow[] row = new GuiRow[rows.Length];

        for (int i = 0; i < rows.Length; i++) {
            row[i] = rows[i](new GuiRow());
        }
        
        GuiColumn column = new GuiColumn(level) {
            Name = null,
            WidthRatio = width,
            Arguments = null,
            SubRows = row,
            VisibleBorder = visible
        };
        
        float[] heights = new float[column.SubRows.Length];

        for (int i = 0; i < column.SubRows.Length; i++) {
            heights[i] = column.SubRows[i].HeightRatio;
        }
        
        heights = GuiBuilder.Normalize(heights);
        
        for (int i = 0; i < column.SubRows.Length; i++) {
            column.SubRows[i].HeightRatio = heights[i];
        }
        
        return column;
    }
}