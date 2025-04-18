//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Text;

namespace NeoKolors.Tui;

public partial class ConsoleScreen : TextWriter {

    private string output = "";
    
    public override void Write(string? value) => output += value;
    public override void Write(char value) => output += value;

    private void UpdateConsole() {
        StandardOutput.Write(output);
        output = "";
    }

    public override Encoding Encoding => Encoding.UTF8;
}