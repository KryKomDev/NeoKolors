namespace NeoKolors.ConsoleGraphics.TUI.GridSystem;

public class GridRenderer : IGuiRenderer {
    public static void Render(IGuiBuilder gui) {
        foreach (var section in gui.GetRenderedSections()) {
            (int x, int y) A = ComputeA(((GridBuilder)gui).Columns, ((GridBuilder)gui).Rows, section.GetPositionA().x, section.GetPositionA().y);
            (int x, int y) B = ComputeA(((GridBuilder)gui).Columns, ((GridBuilder)gui).Rows, section.GetPositionB().x, section.GetPositionB().y);
            section.SetPositionA(A.x, A.y);
            section.SetPositionB(B.x, B.y);
            ((GridSection)section).Width = Math.Abs(A.x - B.x);
            ((GridSection)section).Height = Math.Abs(A.y - B.y);
            RenderSection((GridSection)section);
        }
    }

    internal static void RenderGrid(GridBuilder grid) {
        for (int r = 0; r < grid.Rows; r++) {
            for (int c = 0; c < grid.Columns; c++) {
                RenderBorder(ComputeA(grid.Columns, grid.Rows, c, r), ComputeB(grid.Columns, grid.Rows, c, r), $"Nadpisy asdd");
            }
        }
    }

    private static void RenderSection(GridSection section) {
        //… ─ │ ╭ ╮ ╯ ╰
        
        System.Console.SetCursorPosition(section.PositionA.x, section.PositionA.y);
        System.Console.Write($"╭{new string('─', section.Width - 2)}╮");
        
        System.Console.SetCursorPosition(section.PositionA.x + 3, section.PositionA.y);
        System.Console.Write(section.Name.Length > section.Width - 5
            ? $"{section.Name.AsSpan(0, Math.Max(section.Width - 6, 0))}…"
            : section.Name);

        for (int y = section.PositionA.y + 1; y < section.PositionB.y; ++y) {
            System.Console.SetCursorPosition(section.PositionA.x, y);
            System.Console.Write("│");
            System.Console.SetCursorPosition(section.PositionB.x - 1, y);
            System.Console.Write("│");
        }
        
        System.Console.SetCursorPosition(section.PositionA.x, section.PositionB.y);
        System.Console.Write($"╰{new string('─', section.Width - 2)}╯");
    }

    private static (int x, int y) ComputeA(int columns, int rows, int gridX, int gridY) {
        int cw = System.Console.WindowWidth / columns;
        int rh = System.Console.WindowHeight / rows;
        
        return (gridX * cw, gridY * rh);
    }
    
    private static (int x, int y) ComputeB(int columns, int rows, int gridX, int gridY) {
        int cw = System.Console.WindowWidth / columns;
        int rh = System.Console.WindowHeight / rows;
        
        return ((gridX + 1) * cw - 1, (gridY + 1) * rh - 1);
    }

    private static void RenderBorder((int x, int y) A, (int x, int y) B, string name) {
        System.Console.SetCursorPosition(A.x, A.y);
        System.Console.Write($"╭{new string('─', B.x - A.x - 2)}╮");

        for (int y = A.y + 1; y <= B.y - 1; y++) {
            System.Console.SetCursorPosition(A.x, y);
            System.Console.Write($"│{new String(' ', B.x - A.x - 2)}│");
        }
        
        System.Console.SetCursorPosition(A.x, B.y);
        System.Console.Write($"╰{new string('─', B.x - A.x - 2)}╯");

        name = name.Insert(0, " ");
        
        if (name.Length > B.x - A.x - 4) {
            string newName = name.Substring(0, B.x - A.x - 6) + "… ";
            System.Console.SetCursorPosition(A.x + 2, A.y);
            System.Console.Write(newName);
        }
        else {
            name += " ";
            System.Console.SetCursorPosition(A.x + (B.x - A.x) / 2 - name.Length / 2, A.y);
            System.Console.Write(name);
        }
    }
}