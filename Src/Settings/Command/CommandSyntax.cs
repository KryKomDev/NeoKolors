//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Command;

public class CommandSyntax {
    
    private readonly List<SyntaxElement> _syntax;
    private readonly List<FlagInfo> _flags;
    private readonly List<SwitchInfo> _switches;
    
    public CommandSyntax() {
        _syntax = [];
        _flags = [];
        _switches = [];
    }

    public SyntaxElement[] Syntax => _syntax.ToArray();
    public FlagInfo[] Flags => _flags.ToArray();
    public SwitchInfo[] Switches => _switches.ToArray();

    public void Add(SyntaxElement syntax) => _syntax.Add(syntax);
    public void Add(FlagInfo flag) => _flags.Add(flag);
    public void Add(SwitchInfo switchInfo) => _switches.Add(switchInfo);
}