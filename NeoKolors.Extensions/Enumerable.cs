// NeoKolors
// Copyright (c) 2025 KryKom

using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace NeoKolors.Extensions;

public static class Enumerable {

    extension<T>(T[,] value) {
        
        /// <summary>
        /// Gets the length of the first dimension of a two-dimensional array.
        /// </summary>
        [Pure]
        public int Len0 => value.GetLength(0);
        
        /// <summary>
        /// Gets the length of the second dimension of a two-dimensional array.
        /// </summary>
        [Pure]
        public int Len1 => value.GetLength(1);
    }

    extension<T>(T[,,] value) {
        
        /// <summary>
        /// Gets the length of the first dimension of a two-dimensional array.
        /// </summary>
        [Pure]
        public int Len0 => value.GetLength(0);
        
        /// <summary>
        /// Gets the length of the second dimension of a two-dimensional array.
        /// </summary>
        [Pure]
        public int Len1 => value.GetLength(1);
        
        /// <summary>
        /// Gets the length of the third dimension of a two-dimensional array.
        /// </summary>
        [Pure]
        public int Len2 => value.GetLength(2);
    }
    
    extension<T>(T[,,,] value) {
        
        /// <summary>
        /// Gets the length of the first dimension of a two-dimensional array.
        /// </summary>
        [Pure]
        public int Len0 => value.GetLength(0);
        
        /// <summary>
        /// Gets the length of the second dimension of a two-dimensional array.
        /// </summary>
        [Pure]
        public int Len1 => value.GetLength(1);
        
        /// <summary>
        /// Gets the length of the third dimension of a two-dimensional array.
        /// </summary>
        [Pure]
        public int Len2 => value.GetLength(2);
        
        /// <summary>
        /// Gets the length of the fourth dimension of a two-dimensional array.
        /// </summary>
        [Pure]
        public int Len3 => value.GetLength(3);
    }
    
    
    /// <summary>
    /// Determines whether all elements, except the first one, in the provided collection
    /// satisfy a specified condition.
    /// </summary>
    /// <param name="strings">The collection of elements to evaluate.</param>
    /// <param name="predicate">A function that defines the condition each element, except the first one, must satisfy.</param>
    /// <typeparam name="TSource">The type of elements in the collection.</typeparam>
    /// <returns>
    /// <c>true</c> if all elements, excluding the first one, satisfy the condition; otherwise, <c>false</c>.
    /// </returns>
    [Pure]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AllButFirst<TSource>(this IEnumerable<TSource> strings, Func<TSource, bool> predicate) =>
        strings.Skip(1).All(predicate);

    /// <summary>
    /// Determines whether all elements, except the first one, in the provided collection
    /// satisfy a specified condition.
    /// </summary>
    /// <param name="strings">The collection of elements to evaluate.</param>
    /// <param name="first">A function that defines the condition the first element must satisfy.</param>
    /// <param name="all">A function that defines the condition each element, except the first one, must satisfy.</param>
    /// <typeparam name="TSource">The type of elements in the collection.</typeparam>
    /// <returns>
    /// <c>true</c> if all elements satisfy their respective condition; otherwise, <c>false</c>.
    /// </returns>
    [Pure]
    public static bool FirstAndAll<TSource>(
        this IEnumerable<TSource> strings,
        Func<TSource, bool> first, 
        Func<TSource, bool> all) 
    {
        var e = strings as TSource[] ?? strings.ToArray();
        return e.Skip(1).All(all) && first(e.First());
    }

    /// <summary>
    /// Selects elements from the given source based on the provided selectors and their positions
    /// (first, middle, last). Allows customization of the selection logic based on position or default options.
    /// </summary>
    /// <param name="source">The source collection to enumerate elements from.</param>
    /// <param name="allSelector">A function to select elements applying to all positions except where overridden.</param>
    /// <param name="firstSelector">An optional function to select the first element. If null, the allSelector is used.</param>
    /// <param name="lastSelector">An optional function to select the last element. If null, the allSelector is used.</param>
    /// <param name="defaultTo">Used when the source enumerable has only 1 element. If -1 uses the firstSelector, if 0 uses the allSelector, if 1 uses the lastSelector.</param>
    /// <typeparam name="TSource">The type of elements in the source collection.</typeparam>
    /// <typeparam name="TResult">The type of elements in the resulting collection.</typeparam>
    /// <returns>An enumerable collection of selected elements based on the provided selectors and logic.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the source or allSelector parameters are null.</exception>
    public static IEnumerable<TResult> Select<TSource, TResult>(
        this IEnumerable<TSource> source,
        Func<TSource, TResult> allSelector,
        Func<TSource, TResult>? firstSelector = null,
        Func<TSource, TResult>? lastSelector = null,
        int defaultTo = 0) 
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (allSelector is null) throw new ArgumentNullException(nameof(allSelector));
        
        var enumerable = source as TSource[] ?? source.ToArray();
        
        if (!enumerable.Any()) {
            yield break;
        }
        
        if (enumerable.Length == 1) {
            yield return defaultTo switch {
                -1 when firstSelector is not null => firstSelector(enumerable.First()),
                1 when lastSelector is not null => lastSelector(enumerable.First()),
                _ => allSelector(enumerable.First())
            };
            yield break;
        }

        // first element
        if (firstSelector is null)
            yield return allSelector(enumerable.First());
        else
            yield return firstSelector(enumerable.First());

        // middle elements
        for (int i = 1; i < enumerable.Length - 1; i++)
            yield return allSelector(enumerable[i]);
        
        // last element
        if (lastSelector is null)
            yield return allSelector(enumerable.Last());
        else
            yield return lastSelector(enumerable.Last());
    }
    
    #if NETSTANDARD2_0

    /// <summary>
    /// Converts the provided collection to a <see cref="HashSet{T}"/> containing the unique elements of the collection.
    /// </summary>
    /// <param name="enumerable">The collection of elements to convert into a <see cref="HashSet{T}"/>.</param>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <returns>A <see cref="HashSet{T}"/> containing unique elements from the collection.</returns>
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable) {
        var hs = new HashSet<T>();

        foreach (var e in enumerable) {
            hs.Add(e);
        }
        
        return hs;
    }

    /// <summary>
    /// Retrieves the value associated with the specified key in the dictionary
    /// or returns a provided default value if the key does not exist.
    /// </summary>
    /// <param name="dictionary">The dictionary to search for the specified key.</param>
    /// <param name="key">The key to locate in the dictionary.</param>
    /// <param name="defaultValue">The value to return if the key is not found.</param>
    /// <returns>
    /// The value associated with the specified key if found; otherwise, the provided default value.
    /// </returns>
    public static TValue GetValueOrDefault<TKey, TValue>(
        this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
            => dictionary.TryGetValue(key, out var value) ? value : defaultValue;

    #endif
}