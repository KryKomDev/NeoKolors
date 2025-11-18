namespace NeoKolors.Extensions.Tests;

using Metriks;

public class EnumerableTests {
    
    #region 2D Array Extensions Tests

    [Fact]
    public void Len0_2DArray_ReturnsFirstDimensionLength() {
        var array = new int[3, 4];
        
        Assert.Equal(3, array.Len0);
    }

    [Fact]
    public void Len1_2DArray_ReturnsSecondDimensionLength() {
        var array = new int[3, 4];
        
        Assert.Equal(4, array.Len1);
    }

    [Fact]
    public void Len0Len1_2DArrayWithDifferentSizes_ReturnsCorrectDimensions() {
        var array1 = new string[1, 1];
        var array2 = new double[10, 20];
        var array3 = new bool[0, 5];

        Assert.Equal(1, array1.Len0);
        Assert.Equal(1, array1.Len1);
        
        Assert.Equal(10, array2.Len0);
        Assert.Equal(20, array2.Len1);
        
        Assert.Equal(0, array3.Len0);
        Assert.Equal(5, array3.Len1);
    }

    #endregion

    #region 3D Array Extensions Tests

    [Fact]
    public void Len0Len1Len2_3DArray_ReturnsCorrectDimensions() {
        var array = new int[2, 3, 4];
        
        Assert.Equal(2, array.Len0);
        Assert.Equal(3, array.Len1);
        Assert.Equal(4, array.Len2);
    }

    [Fact]
    public void Len0Len1Len2_3DArrayWithDifferentSizes_ReturnsCorrectDimensions() {
        var array1 = new char[1, 1, 1];
        var array2 = new float[5, 10, 15];

        Assert.Equal(1, array1.Len0);
        Assert.Equal(1, array1.Len1);
        Assert.Equal(1, array1.Len2);
        
        Assert.Equal(5, array2.Len0);
        Assert.Equal(10, array2.Len1);
        Assert.Equal(15, array2.Len2);
    }

    #endregion

    #region 4D Array Extensions Tests

    [Fact]
    public void Len0Len1Len2Len3_4DArray_ReturnsCorrectDimensions() {
        var array = new int[2, 3, 4, 5];
        
        Assert.Equal(2, array.Len0);
        Assert.Equal(3, array.Len1);
        Assert.Equal(4, array.Len2);
        Assert.Equal(5, array.Len3);
    }

    [Fact]
    public void Len0Len1Len2Len3_4DArrayWithDifferentSizes_ReturnsCorrectDimensions() {
        var array1 = new byte[1, 2, 3, 4];
        var array2 = new long[10, 20, 30, 40];

        Assert.Equal(1, array1.Len0);
        Assert.Equal(2, array1.Len1);
        Assert.Equal(3, array1.Len2);
        Assert.Equal(4, array1.Len3);
        
        Assert.Equal(10, array2.Len0);
        Assert.Equal(20, array2.Len1);
        Assert.Equal(30, array2.Len2);
        Assert.Equal(40, array2.Len3);
    }

    #endregion

    #region AllButFirst Tests

    [Fact]
    public void AllButFirst_AllElementsExceptFirstSatisfyCondition_ReturnsTrue() {
        var numbers = new[] { 1, 2, 4, 6, 8 };
        
        var result = numbers.AllButFirst(x => x % 2 == 0);
        
        Assert.True(result);
    }

    [Fact]
    public void AllButFirst_SomeElementsExceptFirstDoNotSatisfyCondition_ReturnsFalse() {
        var numbers = new[] { 1, 2, 3, 6, 8 };
        
        var result = numbers.AllButFirst(x => x % 2 == 0);
        
        Assert.False(result);
    }

    [Fact]
    public void AllButFirst_SingleElement_ReturnsTrue() {
        var numbers = new[] { 5 };
        
        var result = numbers.AllButFirst(x => x > 10);
        
        Assert.True(result); // No elements after first, so vacuously true
    }

    [Fact]
    public void AllButFirst_EmptyCollection_ReturnsTrue() {
        var numbers = new int[0];
        
        var result = numbers.AllButFirst(x => x > 10);
        
        Assert.True(result); // No elements at all, so vacuously true
    }

    [Fact]
    public void AllButFirst_StringCollection_WorksCorrectly() {
        var strings = new[] { "apple", "banana", "cherry", "date" };
        
        var result = strings.AllButFirst(s => s.Length >= 4);
        
        Assert.True(result); // "banana", "cherry", "date" all have length >= 4
    }

    [Fact]
    public void AllButFirst_FirstElementDoesNotSatisfyButOthersDo_ReturnsTrue() {
        var numbers = new[] { 1, 10, 20, 30 };
        
        var result = numbers.AllButFirst(x => x >= 10);
        
        Assert.True(result); // First element (1) is ignored
    }

    #endregion

    #region FirstAndAll Tests

    [Fact]
    public void FirstAndAll_FirstSatisfiesFirstConditionAndRestSatisfyAllCondition_ReturnsTrue() {
        var numbers = new[] { 1, 2, 4, 6, 8 };
        
        var result = numbers.FirstAndAll(x => x % 2 == 1, x => x % 2 == 0);
        
        Assert.True(result);
    }

    [Fact]
    public void FirstAndAll_FirstDoesNotSatisfyFirstCondition_ReturnsFalse() {
        var numbers = new[] { 2, 4, 6, 8 };
        
        var result = numbers.FirstAndAll(x => x % 2 == 1, x => x % 2 == 0);
        
        Assert.False(result);
    }

    [Fact]
    public void FirstAndAll_SomeRestElementsDoNotSatisfyAllCondition_ReturnsFalse() {
        var numbers = new[] { 1, 2, 3, 6, 8 };
        
        var result = numbers.FirstAndAll(x => x % 2 == 1, x => x % 2 == 0);
        
        Assert.False(result);
    }

    [Fact]
    public void FirstAndAll_SingleElementSatisfiesFirstCondition_ReturnsTrue() {
        var numbers = new[] { 1 };
        
        var result = numbers.FirstAndAll(x => x % 2 == 1, x => x % 2 == 0);
        
        Assert.True(result); // Only first element, rest are vacuously true
    }

    [Fact]
    public void FirstAndAll_SingleElementDoesNotSatisfyFirstCondition_ReturnsFalse() {
        var numbers = new[] { 2 };
        
        var result = numbers.FirstAndAll(x => x % 2 == 1, x => x % 2 == 0);
        
        Assert.False(result);
    }

    [Fact]
    public void FirstAndAll_StringCollection_WorksCorrectly() {
        var strings = new[] { "A", "banana", "cherry", "date" };
        
        var result = strings.FirstAndAll(s => s.Length == 1, s => s.Length >= 4);
        
        Assert.True(result); // "A" has length 1, others have length >= 4
    }

    [Fact]
    public void FirstAndAll_EmptyCollection_ThrowsInvalidOperationException() {
        var numbers = Array.Empty<int>();
        
        Assert.Throws<InvalidOperationException>(() => 
            numbers.FirstAndAll(x => x > 0, x => x < 10));
    }

    #endregion

    #region Select with Position-based Selectors Tests

    [Fact]
    public void Select_WithAllSelector_AppliesAllSelectorToAllElements() {
        var numbers = new[] { 1, 2, 3, 4, 5 };
        
        var result = numbers.Select(x => x * 2).ToArray();
        
        Assert.Equal(new[] { 2, 4, 6, 8, 10 }, result);
    }

    [Fact]
    public void Select_WithFirstAndLastSelectors_AppliesCorrectSelectors() {
        var numbers = new[] { 1, 2, 3, 4, 5 };
        
        var result = numbers.Select(
            allSelector: x => x * 2,
            firstSelector: x => x * 10,
            lastSelector: x => x * 100
        ).ToArray();
        
        Assert.Equal(new[] { 10, 4, 6, 8, 500 }, result);
    }

    [Fact]
    public void Select_WithOnlyFirstSelector_AppliesFirstSelectorToFirstAndAllToRest() {
        var numbers = new[] { 1, 2, 3, 4 };
        
        var result = numbers.Select(
            allSelector: x => x * 2,
            firstSelector: x => x * 10
        ).ToArray();
        
        Assert.Equal(new[] { 10, 4, 6, 8 }, result);
    }

    [Fact]
    public void Select_WithOnlyLastSelector_AppliesLastSelectorToLastAndAllToRest() {
        var numbers = new[] { 1, 2, 3, 4 };
        
        var result = numbers.Select(
            allSelector: x => x * 2,
            lastSelector: x => x * 100
        ).ToArray();
        
        Assert.Equal(new[] { 2, 4, 6, 400 }, result);
    }

    [Fact]
    public void Select_SingleElementWithDefaultTo0_UsesAllSelector() {
        var numbers = new[] { 5 };
        
        var result = numbers.Select(
            allSelector: x => x * 2,
            firstSelector: x => x * 10,
            lastSelector: x => x * 100,
            defaultTo: 0
        ).ToArray();
        
        Assert.Equal(new[] { 10 }, result); // allSelector: 5 * 2 = 10
    }

    [Fact]
    public void Select_SingleElementWithDefaultToMinus1_UsesFirstSelector() {
        var numbers = new[] { 5 };
        
        var result = numbers.Select(
            allSelector: x => x * 2,
            firstSelector: x => x * 10,
            lastSelector: x => x * 100,
            defaultTo: -1
        ).ToArray();
        
        Assert.Equal(new[] { 50 }, result); // firstSelector: 5 * 10 = 50
    }

    [Fact]
    public void Select_SingleElementWithDefaultTo1_UsesLastSelector() {
        var numbers = new[] { 5 };
        
        var result = numbers.Select(
            allSelector: x => x * 2,
            firstSelector: x => x * 10,
            lastSelector: x => x * 100,
            defaultTo: 1
        ).ToArray();
        
        Assert.Equal(new[] { 500 }, result); // lastSelector: 5 * 100 = 500
    }

    [Fact]
    public void Select_SingleElementWithDefaultToMinus1ButNoFirstSelector_UsesAllSelector() {
        var numbers = new[] { 5 };
        
        var result = numbers.Select(
            allSelector: x => x * 2,
            defaultTo: -1
        ).ToArray();
        
        Assert.Equal(new[] { 10 }, result); // fallback to allSelector
    }

    [Fact]
    public void Select_SingleElementWithDefaultTo1ButNoLastSelector_UsesAllSelector() {
        var numbers = new[] { 5 };
        
        var result = numbers.Select(
            allSelector: x => x * 2,
            defaultTo: 1
        ).ToArray();
        
        Assert.Equal(new[] { 10 }, result); // fallback to allSelector
    }

    [Fact]
    public void Select_TwoElements_AppliesFirstAndLastSelectors() {
        var numbers = new[] { 1, 2 };
        
        var result = numbers.Select(
            allSelector: x => x * 2,
            firstSelector: x => x * 10,
            lastSelector: x => x * 100
        ).ToArray();
        
        Assert.Equal(new[] { 10, 200 }, result);
    }

    [Fact]
    public void Select_EmptyCollection_ReturnsEmptyResult() {
        var numbers = new int[0];
        
        var result = numbers.Select(x => x * 2).ToArray();
        
        Assert.Empty(result);
    }

    // [Fact]
    // public void Select_NullSource_ThrowsArgumentNullException() {
    //     IEnumerable<int> numbers = null!;
    //     
    //     Assert.Throws<ArgumentNullException>(() => 
    //         numbers.Select(x => x * 2));
    // }

    [Fact]
    public void Select_StringTransformation_WorksCorrectly() {
        var words = new[] { "first", "middle", "last" };
        
        var result = words.Select(
            allSelector: s => s.ToLower(),
            firstSelector: s => s.ToUpper(),
            lastSelector: s => s.ToUpper()
        ).ToArray();
        
        Assert.Equal(new[] { "FIRST", "middle", "LAST" }, result);
    }

    [Fact]
    public void Select_DifferentTypes_WorksCorrectly() {
        var numbers = new[] { 1, 2, 3, 4 };
        
        var result = numbers.Select(
            allSelector: x => x.ToString(),
            firstSelector: x => $"First: {x}",
            lastSelector: x => $"Last: {x}"
        ).ToArray();
        
        Assert.Equal(new[] { "First: 1", "2", "3", "Last: 4" }, result);
    }
    
    #endregion
}
