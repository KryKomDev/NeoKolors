using NeoKolors.Common;
using NeoKolors.Console;
using NeoKolors.ConsoleGraphics.Settings;
using NeoKolors.ConsoleGraphics.Settings.ArgumentType;

namespace NeoKolors.ConsoleGraphics.GUI.Elements;

public class StringGraphicElement : IGraphicElement {
    public int GridX { get; set; }
    public int GridY { get; set; }
    public int Width { get; init; }
    public int Height { get; init; } = 1;
    public string Name { get; init; }
    public bool Selected { get; set; }
    private readonly StringArgumentType argument;
    private string input = "";
    private int cursor = 0;
    private int drawCursor = 0;
    private bool error;
    private int inputWidth;
    
    public void Draw(int x, int y) {
        // ▏▎
        System.Console.SetCursorPosition(x, y);
        System.Console.Write($"{Name}: {StringColors.AddColor("[", 0x777777)} {new string(' ', inputWidth)} {StringColors.AddColor("]", 0x777777)}");
        
        string drawn = cursor < input.Length ? input.Insert(cursor - drawCursor, StringColors.AddColor(@"▎", 0xffffff)) : input + StringColors.AddColor(@"▎", 0xffffff);
        drawn = drawn.Replace("\n", StringColors.AddColor("\\n", Debug.Palette[5]));
        drawn = drawn.Replace(" ", StringColors.AddColor("·", Debug.Palette[5]));
        
        System.Console.SetCursorPosition(x + Name.Length + 4, y);
        
        if (error) {
            ConsoleColors.PrintColored(drawn, Debug.ErrorColor);
        }
        else {
            System.Console.Write(drawn);
        }
    }

    public void Interact(ConsoleKeyInfo keyInfo) {

        // turn error off
        if (error && keyInfo.Key == ConsoleKey.Backspace && input.Length >= argument.MinLength) {
            error = false;
            return;
        }
        
        // char deletion
        if (keyInfo.Key == ConsoleKey.Backspace && input.Length > 0) {
            // if (cursor == input.Length) cursor--;
            if (cursor == 0) return;
            
            input = input.Remove(cursor - 1, 1);
            cursor--;
            CheckLength();
            return;
        }

        // cursor left
        if (keyInfo.Key == ConsoleKey.LeftArrow) {
            cursor = Math.Max(0, cursor - 1);
            return;
        }

        // cursor right
        if (keyInfo.Key == ConsoleKey.RightArrow) {
            cursor = Math.Min(input.Length, cursor + 1);
            return;
        }

        if (argument.IsValid(keyInfo.KeyChar == '\r' ? '\n' : keyInfo.KeyChar) && keyInfo.KeyChar != '\b') {
            char added = keyInfo.KeyChar == '\r' ? '\n' : keyInfo.KeyChar;
            input = input.Insert(cursor, added.ToString());
            error = false;
            cursor = Math.Min(input.Length, cursor + 1);
        }
        else {
            error = true;
            return;
        }
        
        CheckLength();
    }

    private void CheckLength() {
        if (input.Length < argument.MinLength) {
            error = true;
        }
        else if (input.Length > argument.MaxLength) {
            error = true;
            input = input.Remove(Math.Min(cursor, input.Length - 1), 1);
            
            if (cursor >= input.Length) cursor = Math.Max(0, cursor - 1);
        }
        else {
            error = false;
            argument.SetValue(input);
        }
    }

    public object GetValue() {
        return input;
    }

    public StringGraphicElement(int x, int y, string name, StringArgumentType? argument = null, int inputWidth = 40) {
        GridX = x;
        GridY = y;
        Name = name;
        this.argument = argument ?? Arguments.String();
        Width = inputWidth + Name.Length + 6;
        this.inputWidth = inputWidth;
    }
}