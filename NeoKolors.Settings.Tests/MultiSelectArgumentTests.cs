//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Exception;
using static NeoKolors.Settings.Tests.MultiSelectArgumentTests.TestEnum;

namespace NeoKolors.Settings.Tests;

public class MultiSelectArgumentTests {

    [Fact]
    public void ShouldInitializeProperly() {
        MultiSelectArgument<int> a = new MultiSelectArgument<int>(1, 2, 3, 4);
        Assert.Equal([
            new KeyValuePair<int, int>(1, 0), 
            new KeyValuePair<int, int>(2, 1), 
            new KeyValuePair<int, int>(3, 2), 
            new KeyValuePair<int, int>(4, 3)], 
            a.Choices.ToArray());
    }

    [Fact]
    public void ShouldInitializeProperlyWithDefault() {
        MultiSelectArgument<int> a = new MultiSelectArgument<int>([1, 2, 3, 4], 1, 2);
        Assert.Equal([
                new KeyValuePair<int, int>(1, 0), 
                new KeyValuePair<int, int>(2, 1), 
                new KeyValuePair<int, int>(3, 2), 
                new KeyValuePair<int, int>(4, 3)], 
            a.Choices.ToArray());
        
        Assert.Equal([1, 2], a.Value);
        
        Assert.Throws<InvalidArgumentInputException>(() => {
            MultiSelectArgument<int> b = new MultiSelectArgument<int>([1, 2, 3, 4], 1, 2, 5);
        });
        
        Assert.Throws<InvalidArgumentInputException>(() => {
            MultiSelectArgument<int> b = new MultiSelectArgument<int>([1, 2, 3, 4], 1, 2, 3, 4, 4);
        });
    }

    [Fact]
    public void FromEnum_ShouldWorkProperly() {
        var a = MultiSelectArgument<TestEnum>.FromEnum<TestEnum>();
        Assert.Equal([
                new KeyValuePair<TestEnum, int>(A, 0), 
                new KeyValuePair<TestEnum, int>(B, 1), 
                new KeyValuePair<TestEnum, int>(C, 2), 
                new KeyValuePair<TestEnum, int>(D, 3)], 
            a.Choices.ToArray());
    }
    
    internal enum TestEnum { A, B, C, D }
    
    [Fact]
    public void Select_ShouldWorkProperly() {
        var a = MultiSelectArgument<TestEnum>.FromEnum<TestEnum>();
        a.Select(A, B);
        Assert.Equal([A, B], a.Value);
    }
    
    [Fact]
    public void Deselect_ShouldWorkProperly() {
        var a = MultiSelectArgument<TestEnum>.FromEnum(A, B, C, D);
        a.Deselect(A, B);
        Assert.Equal([C, D], a.Value);
    }

    [Fact]
    public void Set_TArr_ShouldWorkProperly() {
        var a = MultiSelectArgument<TestEnum>.FromEnum<TestEnum>();
        a.Set(A, B);
        Assert.Equal([A, B], a.Value);
        a.Set(B, C);
        Assert.Equal([A, C], a.Value);
    }

    [Fact]
    public void Set_T_ShouldWorkProperly() {
        var a = MultiSelectArgument<TestEnum>.FromEnum<TestEnum>();
        a.Set(A);
        Assert.Equal([A], a.Value);
        a.Set(A);
        Assert.Equal([], a.Value);
    }

    [Fact]
    public void Set_ShouldWorkProperly() {
        var a = MultiSelectArgument<TestEnum>.FromEnum<TestEnum>();
        a.Set((object)A);
        Assert.Equal([A], a.Value);
        TestEnum[] ar = [A, B];
        ICollection<TestEnum> c = ar;
        a.Set(c);
        Assert.Equal([B], a.Value);
        var b = MultiSelectArgument<TestEnum>.FromEnum(B, C, D);
        a.Set(b);
        Assert.Equal([B, C, D], a.Value);
        bool[] bl = [true, false, true];
        a.Set(bl);
        Assert.Equal([A, C, D], a.Value);
        var d = new MultiSelectArgument<int>(0, 1, 2);
        var e = new MultiSelectArgument<int>(0, 1, 2, 3);
        Assert.Throws<InvalidArgumentInputException>(() => d.Set(e));
    }
}

