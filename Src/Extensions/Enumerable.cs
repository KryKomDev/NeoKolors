// NeoKolors
// Copyright (c) 2025 KryKom

using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Metriks;

namespace NeoKolors.Extensions;

public static class Enumerable {

    /// <param name="strings">The collection of elements to evaluate.</param>
    /// <typeparam name="TSource">The type of elements in the collection.</typeparam>
    extension<TSource>(IEnumerable<TSource> strings) {
        
        /// <summary>
        /// Determines whether all elements, except the first one, in the provided collection
        /// satisfy a specified condition.
        /// </summary>
        /// <param name="predicate">A function that defines the condition each element, except the first one, must satisfy.</param>
        /// <returns>
        /// <c>true</c> if all elements, excluding the first one, satisfy the condition; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool AllButFirst(Func<TSource, bool> predicate) =>
            strings.Skip(1).All(predicate);

        /// <summary>
        /// Determines whether all elements, except the first one, in the provided collection
        /// satisfy a specified condition.
        /// </summary>
        /// <param name="first">A function that defines the condition the first element must satisfy.</param>
        /// <param name="all">A function that defines the condition each element, except the first one, must satisfy.</param>
        /// <returns>
        /// <c>true</c> if all elements satisfy their respective condition; otherwise, <c>false</c>.
        /// </returns>
        [Pure]
        public bool FirstAndAll(Func<TSource, bool> first, 
            Func<TSource, bool> all) 
        {
            var e = strings as TSource[] ?? strings.ToArray();
            return e.Skip(1).All(all) && first(e.First());
        }

        /// <summary>
        /// Selects elements from the given source based on the provided selectors and their positions
        /// (first, middle, last). Allows customization of the selection logic based on position or default options.
        /// </summary>
        /// <param name="allSelector">A function to select elements applying to all positions except where overridden.</param>
        /// <param name="firstSelector">An optional function to select the first element. If null, the allSelector is used.</param>
        /// <param name="lastSelector">An optional function to select the last element. If null, the allSelector is used.</param>
        /// <param name="defaultTo">Used when the source enumerable has only 1 element. If -1 uses the firstSelector, if 0 uses the allSelector, if 1 uses the lastSelector.</param>
        /// <typeparam name="TResult">The type of elements in the resulting collection.</typeparam>
        /// <returns>An enumerable collection of selected elements based on the provided selectors and logic.</returns>
        /// <exception cref="ArgumentNullException">Thrown if the source or allSelector parameters are null.</exception>
        public IEnumerable<TResult> Select<TResult>(Func<TSource, TResult> allSelector,
            Func<TSource, TResult>? firstSelector = null,
            Func<TSource, TResult>? lastSelector = null,
            int defaultTo = 0) 
        {
            if (strings is null) throw new ArgumentNullException(nameof(strings));
            if (allSelector is null) throw new ArgumentNullException(nameof(allSelector));
        
            var enumerable = strings as TSource[] ?? strings.ToArray();
        
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

        /// <summary>
        /// Extracts a range of elements from the given enumerable collection starting from the specified
        /// start index to the specified end index, inclusive of the start index but exclusive of the end index.
        /// </summary>
        /// <param name="startIndex">The zero-based starting index of the range to extract.</param>
        /// <param name="endIndex">The zero-based ending index of the range to extract (exclusive).</param>
        /// <returns>An enumerable containing the elements from the specified range.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown if <paramref name="startIndex"/> or <paramref name="endIndex"/> is out of the array's bounds,
        /// or if <paramref name="startIndex"/> is greater than <paramref name="endIndex"/>.
        /// </exception>
        public IEnumerable<TSource> InRange(int startIndex, int endIndex) {
            var array = strings as TSource[] ?? strings.ToArray();
            if (startIndex < 0 || startIndex >= array.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index is out of range.");
            if (endIndex < 0 || endIndex >= array.Length)  
                throw new ArgumentOutOfRangeException(nameof(endIndex), "End index is out of range.");
            if (startIndex > endIndex) 
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Start index is larger than the end index");

            for (int i = startIndex; i < endIndex; i++) {
                yield return array[i];
            }
        }

        /// <summary>
        /// Splits the collection into subcollections based on the provided predicate.
        /// </summary>
        public IEnumerable<IEnumerable<TSource>> Split(Predicate<TSource> predicate) {
            int lastIndex = 0;
            var array = strings as TSource[] ?? strings.ToArray();
        
            for (int i = 0; i < array.Length; i++) {
                if (!predicate(array[i])) continue;
            
                yield return array[lastIndex..i];
                lastIndex = i + 1;
            }
        }
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
    
    /// <summary>
    /// Resizes a two-dimensional array to the specified dimensions. If the new dimensions are
    /// larger, the new elements are initialized with the default value for the type. If the dimensions
    /// are smaller, the resulting array contains only the values that fit within the new bounds.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The reference to the array to be resized. Can be null, in which case a new array is created.</param>
    /// <param name="newSizeDim0">The new size for the first dimension of the array. Must be a non-negative value.</param>
    /// <param name="newSizeDim1">The new size for the second dimension of the array. Must be a non-negative value.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="newSizeDim0"/> or <paramref name="newSizeDim1"/> is less than 0.
    /// </exception>
    public static void Resize<T>(ref T[,]? array, int newSizeDim0, int newSizeDim1) {
        if (newSizeDim0 < 0)
            throw new ArgumentOutOfRangeException(nameof(newSizeDim0), "Must be a non-negative number.");

        if (newSizeDim1 < 0) 
            throw new ArgumentOutOfRangeException(nameof(newSizeDim1), "Must be a non-negative number.");
        
        // local copy
        var lArray = array;

        if (lArray is null) {
            array = new T[newSizeDim0, newSizeDim1];
            return;
        }

        // if the new size is the same as the old size, do nothing
        if (lArray.GetLength(0) == newSizeDim0 || lArray.GetLength(1) == newSizeDim1) {
            Debug.Assert(array is not null);
            return;
        }
        
        // copy the actual array
        var newArray = new T[newSizeDim0, newSizeDim1];

        for (int i0 = 0; i0 < Math.Min(newSizeDim0, lArray.GetLength(0)); i0++) {
            for (int i1 = 0; i1 < Math.Min(newSizeDim1, lArray.GetLength(1)); i1++) {
                newArray[i0, i1] = lArray[i0, i1];
            }
        }
        
        // overwrite the old array
        array = newArray;
        Debug.Assert(array is not null);
    }

    /// <summary>
    /// Fills a two-dimensional array with the specified value, overwriting all existing elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array to be filled. Must not be null.</param>
    /// <param name="value">The value to assign to every element in the array.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="array"/> is null.
    /// </exception>
    public static void Fill<T>(T[,]? array, T value) {
        if (array is null)
            throw new ArgumentNullException(nameof(array));

        // when Span2D is available, use the System.Array-like Fill method implementation 
        
        for (int i0 = 0; i0 < array.GetLength(0); i0++) {
            for (int i1 = 0; i1 < array.GetLength(1); i1++) {
                array[i0, i1] = value;
            }
        }
    }

    /// <summary>
    /// Fills a specified rectangular region of a two-dimensional array with a provided value.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array to be filled. Must not be null.</param>
    /// <param name="value">The value to assign to the specified region of the array.</param>
    /// <param name="xStart">The starting index for the first dimension. Must be a non-negative value and within the bounds of the array.</param>
    /// <param name="yStart">The starting index for the second dimension. Must be a non-negative value and within the bounds of the array.</param>
    /// <param name="xCount">The number of elements to fill in the first dimension. Must be a non-negative value and must fit within the array bounds starting from <paramref name="xStart"/>.</param>
    /// <param name="yCount">The number of elements to fill in the second dimension. Must be a non-negative value and must fit within the array bounds starting from <paramref name="yStart"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="array"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="xStart"/>, <paramref name="yStart"/>, <paramref name="xCount"/>, or <paramref name="yCount"/> is invalid
    /// (e.g., negative or out of range for the specified array).
    /// </exception>
    public static void FillCount<T>(T[,]? array, T value, int xStart, int yStart, int xCount, int yCount) {
        if (array is null)
            throw new ArgumentNullException(nameof(array));

        // negative checks
        if (xStart < 0) throw new ArgumentOutOfRangeException(nameof(xStart), "Must be a non-negative number.");
        if (yStart < 0) throw new ArgumentOutOfRangeException(nameof(yStart), "Must be a non-negative number.");
        if (xCount < 0) throw new ArgumentOutOfRangeException(nameof(xCount), "Must be a non-negative number.");
        if (yCount < 0) throw new ArgumentOutOfRangeException(nameof(yCount), "Must be a non-negative number.");
        
        // out of bounds checks
        if ((uint)xStart >= (uint)array.GetLength(0))
            throw new ArgumentOutOfRangeException(nameof(xStart), "Must be a less or equal to array dim0 length.");
        
        if ((uint)yStart >= (uint)array.GetLength(1))
            throw new ArgumentOutOfRangeException(nameof(yStart), "Must be a less or equal to array dim1 length.");
        
        if ((uint)xCount > (uint)(array.GetLength(0) - xStart)) 
            throw new ArgumentOutOfRangeException(nameof(xCount), 
                "Must be less or equal to array dim0 length minus dim0Count.");
        
        if ((uint)yCount > (uint)(array.GetLength(1) - yStart)) 
            throw new ArgumentOutOfRangeException(nameof(yCount), 
                "Must be less or equal to array dim1 length minus dim1Count.");
        
        // when Span2D is available, use the System.Array-like Fill method implementation 
        
        for (int i0 = xStart; i0 < xStart + xCount; i0++) {
            for (int i1 = yStart; i1 < yStart + yCount; i1++) {
                array[i0, i1] = value;
            }
        }
    }

    /// <summary>
    /// Fills a specified subregion within a two-dimensional array with a given value. The subregion is defined
    /// by the provided coordinates, and the method assigns the value to all elements in the specified range.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The two-dimensional array to be filled. Cannot be null.</param>
    /// <param name="value">The value to assign to each element within the specified subregion.</param>
    /// <param name="xStart">The starting index for the first dimension of the array. Must be a non-negative value
    /// and within the bounds of the array.</param>
    /// <param name="yStart">The starting index for the second dimension of the array. Must be a non-negative value
    /// and within the bounds of the array.</param>
    /// <param name="xEnd">The ending index for the first dimension of the array. Must be a non-negative value
    /// and within the bounds of the array.</param>
    /// <param name="yEnd">The ending index for the second dimension of the array. Must be a non-negative value
    /// and within the bounds of the array.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="array"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <paramref name="xStart"/>, <paramref name="yStart"/>, <paramref name="xEnd"/>, or <paramref name="yEnd"/>
    /// is negative or when these values exceed the respective dimensions of <paramref name="array"/>.
    /// </exception>
    public static void Fill<T>(T[,]? array, T value, int xStart, int yStart, int xEnd, int yEnd) {
        if (array is null)
            throw new ArgumentNullException(nameof(array));

        // negative checks
        if (xStart < 0) throw new ArgumentOutOfRangeException(nameof(xStart), "Must be a non-negative number.");
        if (yStart < 0) throw new ArgumentOutOfRangeException(nameof(yStart), "Must be a non-negative number.");
        if (xEnd < 0) throw new ArgumentOutOfRangeException(nameof(xEnd), "Must be a non-negative number.");
        if (yEnd < 0) throw new ArgumentOutOfRangeException(nameof(yEnd), "Must be a non-negative number.");
        
        // out of bounds checks
        if ((uint)xStart >= (uint)array.GetLength(0))
            throw new ArgumentOutOfRangeException(nameof(xStart), "Must be a less or equal to array dim0 length.");
        
        if ((uint)yStart >= (uint)array.GetLength(1))
            throw new ArgumentOutOfRangeException(nameof(yStart), "Must be a less or equal to array dim1 length.");
        
        if ((uint)xEnd >= (uint)array.GetLength(0))
            throw new ArgumentOutOfRangeException(nameof(xEnd), "Must be a less or equal to array dim0 length.");
        
        if ((uint)yEnd >= (uint)array.GetLength(1))
            throw new ArgumentOutOfRangeException(nameof(yEnd), "Must be a less or equal to array dim1 length.");
        
        // switch start and end if necessary
        (xStart, xEnd) = xStart < xEnd ? (xStart, xEnd) : (xEnd, xStart);
        (yStart, yEnd) = yStart < yEnd ? (yStart, yEnd) : (yEnd, yStart);
        
        // when Span2D is available, use the System.Array-like Fill method implementation 
        
        for (int i0 = xStart; i0 <= xEnd; i0++) {
            for (int i1 = yStart; i1 <= yEnd; i1++) {
                array[i0, i1] = value;
            }
        }
    }

    /// <summary>
    /// Safely fills a specified 2D array region with the given value, ensuring the region bounds are clamped within the array's dimensions.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The target 2D array to be filled.</param>
    /// <param name="value">The value used to fill the specified region of the array.</param>
    /// <param name="xStart">The starting index of the fill range in the first dimension (rows).</param>
    /// <param name="yStart">The starting index of the fill range in the second dimension (columns).</param>
    /// <param name="xEnd">The ending index of the fill range in the first dimension (rows).</param>
    /// <param name="yEnd">The ending index of the fill range in the second dimension (columns).</param>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="array"/> is null.</exception>
    public static void SafeFill<T>(T[,]? array, T value, int xStart, int yStart, int xEnd, int yEnd) {
        if (array is null)
            throw new ArgumentNullException(nameof(array));

        xStart = xStart.Clamp(0, array.Len0 - 1);
        xEnd = xEnd.Clamp(0, array.Len0 - 1);
        yStart = yStart.Clamp(0, array.Len1 - 1);
        yEnd = yEnd.Clamp(0, array.Len1 - 1);

        // switch start and end if necessary
        (xStart, xEnd) = xStart < xEnd ? (xStart, xEnd) : (xEnd, xStart);
        (yStart, yEnd) = yStart < yEnd ? (yStart, yEnd) : (yEnd, yStart);

        // when Span2D is available, use the System.Array-like Fill method implementation 

        for (int i0 = xStart; i0 <= xEnd; i0++) {
            for (int i1 = yStart; i1 <= yEnd; i1++) {
                array[i0, i1] = value;
            }
        }
    }

    /// <summary>
    /// Creates and returns a new empty two-dimensional array of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <returns>A new two-dimensional array with zero rows and zero columns.</returns>
    public static T[,] Empty<T>() => new T[0, 0];
    
    public static T[][] ToJagged<T>(T[,] array) {
        int s0 = array.GetLength(0);
        int s1 = array.GetLength(1);
        var jagged = new T[s0][];
        for (int i = 0; i < s0; i++) {
            jagged[i] = new T[s1];
            for (int j = 0; j < s1; j++) {
                jagged[i][j] = array[i, j];
            }
        }
        return jagged;
    }
    
    public static T[][] Split<T>(this T[] array, Predicate<T> predicate) {
        int lastIndex = 0;

        List<T[]> list = [];
        
        for (int i = 0; i < array.Length; i++) {
            if (!predicate(array[i]))
                continue;
            
            list.Add(array[lastIndex..i]);
            lastIndex = i + 1;
        }
        
        return list.ToArray();
    }
}