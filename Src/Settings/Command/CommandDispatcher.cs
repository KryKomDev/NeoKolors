//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Runtime.CompilerServices;
using NeoKolors.Settings.Command.Exception;

namespace NeoKolors.Settings.Command;

public class CommandDispatcher<TResult> : ICommandDispatcher<TResult> {
    
    private readonly CommandNodeCollection<TResult> _nodes = new(allowDefaultNode: false);

    public Context Parse(string input) {
        throw new NotImplementedException();
    }

    public TResult Execute(Context context) {
        throw new NotImplementedException();
    }

    public ICommandDispatcher<TResult> Register(string name, CommandNodeSupplier<TResult> supplier) {
        var res = supplier(new CommandNode<TResult>());
        _nodes.Node(new NodeInfo(name, res));
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? ICommandDispatcher.Execute(Context context) => Execute(context);

    public ICommandDispatcher Register(string name, CommandNodeSupplier supplier) {
        var res = supplier(new CommandNode<TResult>());
        
        if (res is not ICommandNode<TResult>) 
            throw CommandBuilderException.InvalidNodeType();
        
        _nodes.Node(new NodeInfo(name, res));
        return this;
    }

    public Usage[] GetUsages() {
        throw new NotImplementedException();
    }
}