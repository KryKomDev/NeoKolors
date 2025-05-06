//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Text;

namespace NeoKolors.Tui;

public partial class ConsoleScreen : TextWriter {

    private string _output = "";
    
    public override void Write(string? value) => _output += value;
    public override void Write(char value) => _output += value;

    private void UpdateConsole() {
        StdOut.Write(_output);
        _output = "";
    }

    public override Encoding Encoding => Encoding.UTF8;
}