// //
// NeoKolors
// Copyright (c) 2026 KryKom
// //

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using NeoKolors.Console.Input;
using NeoKolors.Extensions;

namespace NeoKolors.Console.Ansi;

public enum AnsiRecordType : byte {
    NONE,
    KEY,
    MOUSE,
    FOCUS,
    PASTE,
    VT_QUERY,
}

/// <summary>
/// Represents a generic record in the ANSI console input/output system.
/// </summary>
/// <remarks>
/// This structure encapsulates various types of ANSI input records,
/// which may represent different event or data types like mouse events, focus changes,
/// text paste operations, or VT queries.
/// The type of the record is determined by the <see cref="Type"/> property, and
/// only the corresponding data accessors are valid for a specific type. For example,
/// if the type is <see cref="AnsiRecordType.MOUSE"/>, the <see cref="Mouse"/> property
/// can be accessed to retrieve the mouse event details.
/// </remarks>
[StructLayout(LayoutKind.Explicit)]
[SuppressMessage("ReSharper", "ReplaceWithFieldKeyword")]
public readonly struct AnsiRecord {

    // type
    [FieldOffset(0)] private readonly AnsiRecordType _type;

    // fields
    [FieldOffset(1)] private readonly KeyEventArgs   _key;
    [FieldOffset(1)] private readonly MouseEventArgs _mouse;
    [FieldOffset(1)] private readonly bool           _focus;
    [FieldOffset(1)] private readonly IntPtr         _paste;
    [FieldOffset(1)] private readonly VTQuery        _query;

    private string GetPastedString() => Marshal.PtrToStringAnsi(_paste) ?? throw new InvalidOperationException();

    public AnsiRecordType Type => _type;
    
    public KeyEventArgs Key =>
        _type == AnsiRecordType.KEY
            ? _key
            : throw new InvalidOperationException();
    
    public MouseEventArgs Mouse =>
        _type == AnsiRecordType.MOUSE 
            ? _mouse 
            : throw new InvalidOperationException();
    
    public bool HasFocus =>
        _type == AnsiRecordType.FOCUS
            ? _focus
            : throw new InvalidOperationException();
    
    public string Pasted => 
        _type == AnsiRecordType.PASTE
            ? GetPastedString()
            : throw new InvalidOperationException();
    
    public VTQuery Query => 
        _type == AnsiRecordType.VT_QUERY
            ? _query
            : throw new InvalidOperationException();
    
    private AnsiRecord(AnsiRecordType type) {
        _type = type;
    }

    public AnsiRecord(KeyEventArgs key) {
        _type = AnsiRecordType.KEY;
        _key  = key;
    }
    
    public AnsiRecord(MouseEventArgs mouse) {
        _type  = AnsiRecordType.MOUSE;
        _mouse = mouse;
    }

    public AnsiRecord(bool focus) {
        _type  = AnsiRecordType.FOCUS;
        _focus = focus;
    }

    public AnsiRecord(string paste) {
        _type  = AnsiRecordType.PASTE;
        _paste = Marshal.StringToHGlobalAnsi(paste);
    }

    public AnsiRecord(VTQuery query) {
        _type  = AnsiRecordType.VT_QUERY;
        _query = query;
    }

    /// <summary>
    /// Represents an empty or undefined ANSI record.
    /// </summary>
    /// <remarks>
    /// This property is used to define a default or neutral state for an <see cref="AnsiRecord"/>.
    /// It indicates that no specific data or event is associated with the record.
    /// Accessing properties of this record without checking the <see cref="Type"/> will typically result
    /// in an exception. To effectively use the <see cref="None"/> property, ensure that the
    /// <see cref="Type"/> is explicitly validated before attempting to access additional details.
    /// </remarks>
    public static AnsiRecord None { get; } = new(AnsiRecordType.NONE);

    private const int MAX_MSG_LEN = 10;
    
    public override string ToString() {
        return _type switch {
            AnsiRecordType.NONE     => "None",
            AnsiRecordType.KEY      => $"Key {{ {_key.ToString()} }}",
            AnsiRecordType.MOUSE    => $"Mouse {{ {Mouse.ToString()} }}",
            AnsiRecordType.FOCUS    => $"Focus {{ {_focus} }}",
            AnsiRecordType.PASTE    => GetPaste(GetPastedString()),
            AnsiRecordType.VT_QUERY => $"VTQuery {{ {_query} }}",
            _                       => throw new ArgumentOutOfRangeException()
        };

        string GetPaste(string pastedString) {
            return $"Paste {{ Length: {pastedString.Length}, Text: '{pastedString.Shrink(MAX_MSG_LEN)}' }}";
        }
    }
}