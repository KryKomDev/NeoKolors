//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Runtime.CompilerServices;
using NeoKolors.Settings.Argument;
using NeoKolors.Settings.Builder.Delegate;
using NeoKolors.Settings.Command.Exception;

namespace NeoKolors.Settings.Command;

public class CommandNode<TResult> : ICommandNode<TResult> {
    
    private readonly CommandSyntax _syntax;
    private ResultBuilder<TResult>? _resultBuilder;
    
    public CommandNode() {
        _syntax = new CommandSyntax();
        _resultBuilder = null;
    }
    
    public Context Parse(string input) {
        throw new NotImplementedException();
    }
    
    public TResult Execute(Context context) {
        if (_resultBuilder is null) 
            throw CommandException.NoResultConstructor();
        return _resultBuilder!(context);
    }
    
    public ICommandNode<TResult> Executes(ResultBuilder<TResult> action) {
        _resultBuilder = action;
        return this;
    }

    public ICommandNode<TResult> Nodes(CommandNodeCollectionSupplier<TResult> supplier) {
        var res = supplier([]);
        _syntax.Add(new SyntaxElement(res.Nodes));
        return this;
    }

    public ICommandNode Nodes(CommandNodeCollectionSupplier supplier) {
        var res = supplier([]);
        
        if (res is not CommandNodeCollection<TResult>) 
            throw CommandBuilderException.InvalidNodeCollectionType();
        
        _syntax.Add(new SyntaxElement(res.Nodes));
        return this;
    }

    public ICommandNode<TResult> Arg(string name, IParsableArgument argument) {
        _syntax.Add(new ArgumentInfo(name, "", argument));
        return this;
    }
  
    public ICommandNode<TResult> Arg(ArgumentInfo argumentInfo) {
        _syntax.Add(argumentInfo);
        return this;
    }
    
    public ICommandNode<TResult> Flag(string name, IParsableArgument argument) {
        _syntax.Add(new FlagInfo(name, argument));
        return this;
    }
    
    public ICommandNode<TResult> Flag(FlagInfo flagInfo) {
        _syntax.Add(flagInfo);
        return this;
    }
    
    public ICommandNode<TResult> Switch(string name) {
        _syntax.Add(new SwitchInfo(name));
        return this;
    }

    public ICommandNode<TResult> Switch(SwitchInfo switchInfo) {
        _syntax.Add(switchInfo);
        return this;
    }

    ICommandNode ICommandNode.Arg(string name, IParsableArgument argument) => Arg(name, argument);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ICommandNode ICommandNode.Arg(ArgumentInfo argumentInfo) => Arg(argumentInfo);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ICommandNode ICommandNode.Flag(string name, IParsableArgument argument) => Flag(name, argument);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ICommandNode ICommandNode.Flag(FlagInfo flagInfo) => Flag(flagInfo);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ICommandNode ICommandNode.Switch(string name) => Switch(name);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ICommandNode ICommandNode.Switch(SwitchInfo switchInfo) => Switch(switchInfo);

    public ICommandNode Executes(ResultBuilder action) {
        TResult ResultBuilder(in Context context) {
            var res = action(context);
            if (res is not TResult tResult) throw CommandBuilderException.InvalidResultType();
            return tResult;
        }

        _resultBuilder = ResultBuilder;
        
        return this;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public CommandSyntax GetSyntax() => _syntax;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    object? ICommandNode.Execute(Context context) => Execute(context);
    
    public Usage GetUsage() {
        throw new NotImplementedException();
    }
}