//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Command.Exception;
using OneOf;

namespace NeoKolors.Settings.Command;

public readonly struct SyntaxElement {
    private OneOf<ArgumentInfo, NodeInfo[]> Content { get; }
    
    public bool IsArgument => Content.IsT0;
    public bool IsNodes => Content.IsT1;
    
    public ArgumentInfo AsArg => Content.AsT0;
    public NodeInfo[] AsNodes => Content.AsT1;
    
    public SyntaxElement(OneOf<ArgumentInfo, NodeInfo[]> content) {
        if (content.IsT1) {
            var res = Validate(content.AsT1);
           
            if (res is "") {
                throw CommandBuilderException.DuplicateDefaultNode();
            }

            if (res is not null) {
                throw CommandBuilderException.DuplicateNode(res);
            }
        }
        
        Content = content;
    }

    private static string? Validate(NodeInfo[] nodes) {
        HashSet<string> names = [];

        for (var i = 0; i < nodes.Length; i++) {
            var node = nodes[i];
            if (!names.Add(node.Name)) {
                return node.Name;
            }
        }

        return null;
    }
    
    public static implicit operator SyntaxElement(ArgumentInfo info) => new(info);
    public static implicit operator SyntaxElement(NodeInfo[] nodes) => new(nodes);
}