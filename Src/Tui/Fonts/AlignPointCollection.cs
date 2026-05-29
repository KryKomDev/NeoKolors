// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Tui.Fonts;

/// <summary>
/// Represents a collection of <see cref="AlignPoint"/> objects, ensuring unique alignment points
/// based on their character property. This class provides a specialized set implementation
/// that uses a custom equality comparer for managing <see cref="AlignPoint"/> elements.
/// </summary>
public class AlignPointCollection : HashSet<AlignPoint> {
    
    private static readonly AlignPointEqualityComparer COMPARER = new(); 

    public AlignPointCollection() : base(COMPARER) { }
    public AlignPointCollection(int capacity) : base(capacity, COMPARER) { }
    public AlignPointCollection(IEnumerable<AlignPoint> collection) : base(collection, COMPARER) { }

    public bool Contains(char c) => Contains(new AlignPoint(c, default));
    public bool TryGetValue(char c, out AlignPoint alignPoint) => TryGetValue(new AlignPoint(c, default), out alignPoint);

    public bool Remove(char c) => Remove(new AlignPoint(c, default));
    
    public void AddRange(IEnumerable<AlignPoint> alignPoints) {
        foreach (var ap in alignPoints) 
            Add(ap);
    }
    
    private class AlignPointEqualityComparer : IEqualityComparer<AlignPoint> {
        public bool Equals(AlignPoint x, AlignPoint y) => x.Character == y.Character;
        public int GetHashCode(AlignPoint obj) => obj.Character.GetHashCode();
    }
}