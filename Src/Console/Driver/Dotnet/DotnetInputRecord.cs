// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using NeoKolors.Console.Input;

namespace NeoKolors.Console.Driver.Dotnet;

internal enum DotnetInputRecordType : byte {
    KEYBOARD,
    MOUSE,
    FOCUS,
    PASTE,
}

[StructLayout(LayoutKind.Explicit)]
[SuppressMessage("ReSharper", "ReplaceWithFieldKeyword")]
internal readonly struct DotnetInputRecord {
    
    // the type
    [field: FieldOffset(0)] private readonly DotnetInputRecordType _type;
    
    // the values
    [field: FieldOffset(1)] private readonly KeyEventArgs   _key;
    [field: FieldOffset(1)] private readonly MouseEventArgs _mouse;
    [field: FieldOffset(1)] private readonly bool           _focus;
    [field: FieldOffset(1)] private readonly string?        _paste;

    public DotnetInputRecordType Type => _type;
    
    public KeyEventArgs Key => 
        _type == DotnetInputRecordType.KEYBOARD 
            ? _key 
            : throw new InvalidOperationException();
    
    public MouseEventArgs Mouse => 
        _type == DotnetInputRecordType.MOUSE 
            ? _mouse 
            : throw new InvalidOperationException();
    
    public bool HasFocus => 
        _type == DotnetInputRecordType.FOCUS 
            ? _focus
            : throw new InvalidOperationException();
    
    public string PastedText =>
        _type == DotnetInputRecordType.PASTE 
            ? _paste! 
            : throw new InvalidOperationException();
    
    public DotnetInputRecord(KeyEventArgs key) {
        _type = DotnetInputRecordType.KEYBOARD;
        _key  = key;
    }

    public DotnetInputRecord(MouseEventArgs mouse) {
        _type  = DotnetInputRecordType.MOUSE;
        _mouse = mouse;
    }
    
    public DotnetInputRecord(bool hasFocus) {
        _type  = DotnetInputRecordType.FOCUS;
        _focus = hasFocus;
    }

    public DotnetInputRecord(string pastedText) {
        _type  = DotnetInputRecordType.PASTE;
        _paste = pastedText;
    }

    public override string ToString() {
        return _type switch {
            DotnetInputRecordType.KEYBOARD => $"KeyEvent {{ {Key.ToString()} }}",
            DotnetInputRecordType.MOUSE    => $"MouseEvent {{ {Mouse.ToString()} }}",
            DotnetInputRecordType.FOCUS    => $"FocusEvent {{ {(_focus ? "In" : "Out")} }}",
            DotnetInputRecordType.PASTE    => $"PasteEvent {{ Length: {_paste!.Length}, {CreatePastedStr(_paste)} }}",
            _                              => throw new ArgumentOutOfRangeException()
        };
    }

    private const int MAX_PASTED_LENGTH = 10;

    private static string CreatePastedStr(string pasted) {
        if (pasted.Length <= MAX_PASTED_LENGTH) 
            return pasted;

        const int partLen = MAX_PASTED_LENGTH / 2;
        
        return pasted[..partLen] + "…" + pasted[^(MAX_PASTED_LENGTH - partLen)..];
    }
}