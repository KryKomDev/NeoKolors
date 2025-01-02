//
// NeoKolors
// Copyright (c) 2024 KryKom
//

namespace NeoKolors.Settings;

public interface IOperatorSettable<T> {
    
    /// <summary>
    /// sets the value of the implementing class using the value parameter
    /// </summary>
    /// <example>
    /// <b>example implementation:</b>
    /// <code>
    /// public class C : IOperatorSettable&lt;int&gt; {
    ///     public int Value { get; set; }
    ///
    ///     public static C operator &lt;&lt;(C self, int value) {
    ///         self.Value = value;
    ///     }
    /// }
    /// </code>
    ///
    /// <b>example usage:</b>
    /// <code>
    /// variable &lt;&lt;= 123;
    /// </code>
    /// </example>
    public static abstract IOperatorSettable<T> operator <<(IOperatorSettable<T> self, T value);
}