﻿using NeoKolors.Console;
using NeoKolors.Settings;
using NeoKolors.Settings.Argument;

namespace NeoKolors.Tui.Elements.InteractiveOld;

public class IntegerGraphicElement : IGraphicElement {
    public int GridX { get; set; }
    public int GridY { get; set; }
    public int Width { get; init; }
    public int Height { get; init; } = 1;
    public string Name { get; init; }
    public bool Selected { get; set; }
    
    private readonly IntegerArgument argument;
    private string input = "";
    private bool overflow;
    
    public void Draw(int x, int y) {
        System.Console.SetCursorPosition(x, y);
        
        if (Selected)
            ConsoleColors.PrintColored($"{Name}: ", 0xffffff);
        else
            System.Console.Write($"{Name}: ");

        if (input == "") {
            ConsoleColors.PrintColored("<number>", 0x767676);
        }
        
        if (overflow) {
            ConsoleColors.PrintColored(input, Debug.ErrorColor);
        }
        else {
            System.Console.Write(input);
        }

        System.Console.Write(new string(' ', 11 - input.Length));
    }

    public void Interact(ConsoleKeyInfo keyInfo) {
        switch (keyInfo.Key) {
            case ConsoleKey.UpArrow:
                argument.Set(argument.Get() + 1);
                input = argument.Get().ToString();
                break;
            case ConsoleKey.DownArrow:
                argument.Set(argument.Get() - 1);
                input = argument.Get().ToString();
                break;
            case ConsoleKey.D0 or ConsoleKey.NumPad0:
                input += "0";
                break;
            case ConsoleKey.D1 or ConsoleKey.NumPad1:
                input += "1";
                break;
            case ConsoleKey.D2 or ConsoleKey.NumPad2:
                input += "2";
                break;
            case ConsoleKey.D3 or ConsoleKey.NumPad3:
                input += "3";
                break;
            case ConsoleKey.D4 or ConsoleKey.NumPad4:
                input += "4";
                break;
            case ConsoleKey.D5 or ConsoleKey.NumPad5:
                input += "5";
                break;
            case ConsoleKey.D6 or ConsoleKey.NumPad6:
                input += "6";
                break;
            case ConsoleKey.D7 or ConsoleKey.NumPad7:
                input += "7";
                break;
            case ConsoleKey.D8 or ConsoleKey.NumPad8:
                input += "8";
                break;
            case ConsoleKey.D9 or ConsoleKey.NumPad9:
                input += "9";
                break;
            case ConsoleKey.Subtract or ConsoleKey.OemMinus:
                if (input.Length is 0) {
                    input = "-";
                }
                break;
            case ConsoleKey.Backspace:
                if (input.Length != 0) {
                    input = input.Remove(input.Length - 1, 1);
                }

                if (input.Length == 1) {
                    input = "";
                }
                break;
        }

        try {
            var i = int.Parse(input);
            overflow = false;
            argument.Set(i);
        }
        catch (OverflowException) {
            overflow = true;
            if (input.Length != 0) {
                input = input.Remove(input.Length - 1, 1);
            }
        }
        catch (FormatException) {
            
        }
    }

    public object Get() {
        return argument.Get();
    }

    public IntegerGraphicElement(int x, int y, string name, IntegerArgument? argument = null) {
        GridX = x;
        GridY = y;
        Name = name;
        this.argument = argument ?? Arguments.Integer();
        this.argument.Set(0);
        Width = Name.Length + 13;
    }
}

public class UnsignedIntegerElement : IGraphicElement {
    public int GridX { get; set; }
    public int GridY { get; set; }
    public int Width { get; init; }
    public int Height { get; init; } = 1;
    public string Name { get; init; }
    public bool Selected { get; set; }
    
    private readonly UIntegerArgument argument;
    private string input = "";
    private bool overflow;
    
    public void Draw(int x, int y) {
        System.Console.SetCursorPosition(x, y);
        
        if (Selected)
            ConsoleColors.PrintColored($"{Name}: ", 0xffffff);
        else
            System.Console.Write($"{Name}: ");

        if (input == "") {
            ConsoleColors.PrintColored("<number>", 0x767676);
        }
        
        if (overflow) {
            ConsoleColors.PrintColored(input, Debug.ErrorColor);
        }
        else {
            System.Console.Write(input);
        }

        System.Console.Write(new string(' ', 10 - input.Length));
    }

    public void Interact(ConsoleKeyInfo keyInfo) {
        switch (keyInfo.Key) {
            case ConsoleKey.UpArrow:
                argument.Set(argument.Get() + 1);
                input = argument.Get().ToString();
                break;
            case ConsoleKey.DownArrow:
                argument.Set(argument.Get() - 1);
                input = argument.Get().ToString();
                break;
            case ConsoleKey.D0 or ConsoleKey.NumPad0:
                input += "0";
                break;
            case ConsoleKey.D1 or ConsoleKey.NumPad1:
                input += "1";
                break;
            case ConsoleKey.D2 or ConsoleKey.NumPad2:
                input += "2";
                break;
            case ConsoleKey.D3 or ConsoleKey.NumPad3:
                input += "3";
                break;
            case ConsoleKey.D4 or ConsoleKey.NumPad4:
                input += "4";
                break;
            case ConsoleKey.D5 or ConsoleKey.NumPad5:
                input += "5";
                break;
            case ConsoleKey.D6 or ConsoleKey.NumPad6:
                input += "6";
                break;
            case ConsoleKey.D7 or ConsoleKey.NumPad7:
                input += "7";
                break;
            case ConsoleKey.D8 or ConsoleKey.NumPad8:
                input += "8";
                break;
            case ConsoleKey.D9 or ConsoleKey.NumPad9:
                input += "9";
                break;
            case ConsoleKey.Backspace:
                if (input.Length != 0) {
                    input = input.Remove(input.Length - 1, 1);
                }

                if (input.Length == 1) {
                    input = "";
                }
                break;
        }

        try {
            var i = int.Parse(input);
            overflow = false;
            argument.Set(i);
        }
        catch (OverflowException) {
            overflow = true;
            if (input.Length != 0) {
                input = input.Remove(input.Length - 1, 1);
            }
        }
        catch (FormatException) {
            
        }
    }

    public object Get() {
        return argument.Get();
    }

    public UnsignedIntegerElement(int x, int y, string name, UIntegerArgument? argument = null) {
        GridX = x;
        GridY = y;
        Name = name;
        this.argument = argument ?? Arguments.UInteger();
        this.argument.Set(0);
        Width = Name.Length + 12;
    }
}