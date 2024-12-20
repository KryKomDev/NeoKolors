using NeoKolors.ConsoleGraphics.TUI.Elements;
using NeoKolors.ConsoleGraphics.TUI.Exceptions;

namespace NeoKolors.ConsoleGraphics.TUI.GridSystem;

public class GridSection : IGuiSection {
    public GridSection[]? Sections { get; private set; }
    public IGraphicElement[]? Arguments { get; private set; }
    
    public string Name { get; }
    
    public int Rows { get; private init; }
    public int Columns { get; private init; }
    public (int x, int y) GridPositionA { get; }
    public (int x, int y) GridPositionB { get; }
    public bool Active { get; set; } = false;

    public int Width { get; internal set; }
    public int Height { get; internal set; }

    public (int x, int y) PositionA { get; internal set; }
    public (int x, int y) PositionB { get; internal set; }

    private GridSection(
        string name, int rows, int columns, 
        (int x, int y) gridPositionA, 
        (int x, int y) gridPositionB, 
        params IGraphicElement[]? arguments) 
    {
        Name = name;
        Rows = rows;
        Columns = columns;
        Arguments = arguments;
        Sections = null;
        
        int xa = gridPositionA.x;
        int ya = gridPositionA.y;
        int xb = gridPositionB.x;
        int yb = gridPositionB.y;
        GridPositionA = (Math.Min(xa, xb), Math.Min(ya, yb));
        GridPositionB = (Math.Max(xa, xb), Math.Max(ya, yb));
    }

    private GridSection(
        string name, int rows, int columns, 
        (int x, int y) gridPositionA, 
        (int x, int y) gridPositionB,  
        params GridSection[] sections) 
    {
        Name = name;
        Rows = rows;
        Columns = columns;
        Arguments = null;
        Sections = sections;
        
        int xa = gridPositionA.x;
        int ya = gridPositionA.y;
        int xb = gridPositionB.x;
        int yb = gridPositionB.y;
        GridPositionA = (Math.Min(xa, xb), Math.Min(ya, yb));
        GridPositionB = (Math.Max(xa, xb), Math.Max(ya, yb));

        CheckSections();
    }
    
    public static GridSection New(
        string name, int rows, int columns, 
        (int x, int y) gridPositionA, 
        (int x, int y) gridPositionB,
        params GridSection[] sections) 
        => new(name, rows, columns, gridPositionA, gridPositionB, sections);

    public static GridSection New(
        string name, int rows, int columns, 
        (int x, int y) gridPositionA, 
        (int x, int y) gridPositionB,
        params IGraphicElement[]? arguments) 
        => new(name, rows, columns, gridPositionA, gridPositionB, arguments);

    private static bool Overlaps((int x, int y) A1, (int x, int y) B1, (int x, int y) A2, (int x, int y) B2) 
        => A1.x < B2.x && B1.x > A2.x && A1.y < B2.y && B1.y > A2.y;

    private void CheckSections() {
        if (Sections is null) return;
        
        foreach (var s in Sections) {
            if (s.GridPositionA.x < 0 || s.GridPositionA.x >= Rows) throw new GuiSectionOutOfBoundsException($"Lower X coordinate of section {s.Name} must be greater than or equal to zero and less than {Rows}. Instead it was {s.GridPositionA.x}.");
            if (s.GridPositionA.y < 0 || s.GridPositionA.y >= Rows) throw new GuiSectionOutOfBoundsException($"Lower Y coordinate of section {s.Name} must be greater than or equal to zero and less than {Columns}. Instead it was {s.GridPositionA.y}.");
            if (s.GridPositionB.x < 0 || s.GridPositionA.x >= Columns) throw new GuiSectionOutOfBoundsException($"Higher X coordinate of section {s.Name} must be greater than or equal to zero and less than {Rows}. Instead it was {s.GridPositionB.x}.");
            if (s.GridPositionB.y < 0 || s.GridPositionB.y >= Columns) throw new GuiSectionOutOfBoundsException($"Higher Y coordinate of section {s.Name} must be greater than or equal to zero and less than {Columns}. Instead it was {s.GridPositionB.y}.");
        }

        for (int i = 0; i < Sections.Length; i++) {
            for (int j = 0; j < i - 1; j++) {
                if (Overlaps(Sections[i].GridPositionA, Sections[i].GridPositionB, Sections[j].GridPositionA, Sections[j].GridPositionB))
                    throw new GuiSectionOverlapsException(Sections[i].Name, Sections[j].Name);
            }
        }
    }

    public (int x, int y) GetPositionA() => GridPositionA;
    public (int x, int y) GetPositionB() => GridPositionB;
    public void SetPositionA(int x, int y) => PositionA = (x, y);
    public void SetPositionB(int x, int y) => PositionB = (x, y);
    public bool IsActive() => Active;
    public IGraphicElement[] GetArguments() => Arguments ?? [];
    public IGuiSection[] GetSections() => Sections ?? [];
}