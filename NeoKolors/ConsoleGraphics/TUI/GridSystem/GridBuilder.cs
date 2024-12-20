namespace NeoKolors.ConsoleGraphics.TUI.GridSystem;

public class GridBuilder : IGuiBuilder {
    public int Rows { get; }
    public int Columns { get; }
    public GridSection[] Sections { get; private set; }
    
    public IGuiSection[] GetRenderedSections() {
        List<IGuiSection> sections = new List<IGuiSection>();

        foreach (var s in Sections) {
            sections.Add(s);
            if (s.Sections != null) sections.AddRange(s.Sections);
        }
        
        return sections.ToArray();
    }

    public void Build(params IGuiSection[] sections) {
        Sections = new GridSection[sections.Length];
        for (var i = 0; i < sections.Length; i++) {
            Sections[i] = (GridSection)sections[i];
        }
    }

    public GridBuilder(int rows, int columns) {
        Rows = rows;
        Columns = columns;
        Sections = [];
    }

    private GridBuilder(int rows, int columns, IGuiSection[] sections) {
        Sections = ((GridSection[]?)sections)!;
        Rows = rows;
        Columns = columns;
    }
    
    public static IGuiBuilder Build(int rows, int columns, params IGuiSection[] sections) {
        return new GridBuilder(rows, columns, sections);
    }
}