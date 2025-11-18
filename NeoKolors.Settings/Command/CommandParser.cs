//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Command.Exception;
using static NeoKolors.Settings.Attributes.DisplayTypeAttribute;

namespace NeoKolors.Settings.Command;

/// <summary>
/// Parses and processes commands from a given input string.
/// </summary>
public class CommandParser {

    private readonly string _command;
    private int _index;

    public CommandParser(string command) {
        _command = command;
        _index = 0;
    }

    public void Parse(CommandSyntax syntax, in Context context) {
        var switches = syntax.Switches.ToList();
        var flags = syntax.Flags.ToList();
        
        Parse(syntax, in context, switches, flags);
        
        ParseOptions(in context, flags, switches);
    }

    private void Parse(CommandSyntax syntax, in Context context, in List<SwitchInfo> switches, in List<FlagInfo> flags) {
        
        // propagate switches and flags to the context
        switches.AddRange(syntax.Switches);
        flags.AddRange(syntax.Flags);
        
        foreach (var s in syntax.Syntax) {
            if (s.IsArgument) {
                ParseArg(s.AsArg, context);
            }
            else if (s.IsNodes) {
                ParseNodes(s.AsNodes, context, switches, flags);
            }
        }
    }

    private void ParseArg(ArgumentInfo arg, in Context context) {
        var t = ReadNext();
                
        if (t.Type is TokenType.FLAG || 
            t.Type is TokenType.STRING && arg.Argument is not IArgument<string>) 
        {
            throw CommandParserException.UnexpectedToken(GetName(arg.Argument.GetType()), t, _index - t.Value.Length);
        }

        arg.Argument.Set(t.Value);
                
        context.Add(arg.Name, arg.Argument);
    }

    private void ParseNodes(NodeInfo[] nodes, in Context context, in List<SwitchInfo> switches, in List<FlagInfo> flags) {
        var t = ReadNext();

        // if the next token is a flag or string literal, throw
        if (t.Type is not TokenType.ETC)
            throw CommandParserException.UnexpectedToken("literal", t, _index - t.Value.Length);
        
        var selection = from n in nodes
            where n.Name == t.Value
            select n;

        var ns = selection as NodeInfo[] ?? selection.ToArray();

        // if there is a node with the same name as the literal, parse its arguments
        if (ns.Length != 0) {
            Parse(ns.First().Node.GetSyntax(), context, switches, flags);
            return;
        }

        // if there is no node with the same name as the literal, parse the default node
        NodeInfo def;

        try {
            def = nodes.First(n => n.Name is "");
        }
        catch (InvalidOperationException) {
            throw CommandParserException.InvalidNodeIdentifier(t.Value, _index - t.Value.Length);
        }
        
        Parse(def.Node.GetSyntax(), context, switches, flags);
    }

    private void ParseOptions(in Context context, List<FlagInfo> flags, List<SwitchInfo> switches) {
        
    }
    
    private void ParseFlag(in Context context, List<FlagInfo> flags) {
        var st = ReadNext();
        var nd = ReadNext();
    }

    private void ParseSwitch(in Context context, List<SwitchInfo> switches) {
        
    }

    internal bool IsEol => _index >= _command.Length;

    private void SkipToNext() {
        while (!IsEol && _command[_index] is ' ') 
            _index++;
    }
    
    private Token ReadNext() {
        if (IsEol) 
            throw CommandParserException.IsEol();
        
        SkipToNext();

        return _command[_index] switch {
            '\"' => new Token(TokenType.STRING, ReadUntil('\"', true)),
            '\'' => new Token(TokenType.STRING, ReadUntil('\'', true)),
            '-' => new Token(TokenType.FLAG, ReadUntil(' ')),
            _ => new Token(TokenType.ETC, ReadUntil(' '))
        };
    }
    
    private string ReadUntil(char c, bool quotes = false) {
        int start = _index;
        
        if (quotes && !IsEol) 
            _index++;
        
        while (!IsEol && _command[_index] != c) 
            _index++;
        
        if (quotes && !IsEol) 
            _index++;

        return _command.Substring(start, _index - start);
    }
    
    internal struct Token {
        public TokenType Type { get; }
        public string Value { get; }
        
        public Token(TokenType type, string value) {
            Type = type;
            Value = value;
        }
    }

    internal enum TokenType {
        STRING,
        FLAG,
        ETC
    }
}