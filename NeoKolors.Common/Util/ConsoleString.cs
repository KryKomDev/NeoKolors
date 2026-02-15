// NeoKolors
// Copyright (c) 2025 KryKom

using System.Collections;

namespace NeoKolors.Common.Util;

public sealed class ConsoleString : IEnumerable<ConsoleKeyInfo> {
    
    public static ConsoleString Empty { get; } = new();
    
    private readonly List<ConsoleKeyInfo> _str = [];
    
    public ConsoleKeyInfo this[int index] => _str[index];
    
    public IEnumerator<ConsoleKeyInfo> GetEnumerator() => _str.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public static implicit operator string(ConsoleString str) 
        => string.Concat(str._str.Select(i => i.KeyChar).ToArray());

    public static ConsoleString operator +(ConsoleString str1, ConsoleString str2) {
        var s = new ConsoleString();
        s._str.AddRange(str1._str);
        s._str.AddRange(str2._str);
        return s;
    }

    public static ConsoleString operator +(ConsoleString str, ConsoleKeyInfo key) {
        var s = new ConsoleString();
        s._str.AddRange(str._str);
        s._str.Add(key);
        return s;
    }

    public static ConsoleString operator +(ConsoleKeyInfo key, ConsoleString str) {
        var s = new ConsoleString();
        s._str.Add(key);
        s._str.AddRange(str._str);
        return s;
    }
}