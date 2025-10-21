// NeoKolors
// Copyright (c) 2025 KryKom

using System.Collections;

namespace NeoKolors.Tui.Fonts.V2;

public class GlyphAlignmentPointCollection : IEnumerable<(char, Point)> {
    
    private readonly Dictionary<char, Point> _points = [];
    
    public void Add(char key, Point value) => _points.Add(key, value);
    public bool ContainsKey(char key) => _points.ContainsKey(key);
    public Point this[char key] => _points[key];
    public int Count => _points.Count;
    
    IEnumerator<(char, Point)> IEnumerable<(char, Point)>.GetEnumerator() 
        => _points.Select(p => (p.Key, p.Value)).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _points.GetEnumerator();
}