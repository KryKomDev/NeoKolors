//
// NeoKolors
// Copyright (c) 2025 KryKom
// 

using NeoKolors.Settings.Exception;
using static NeoKolors.Settings.Tests.DummyEnum;

namespace NeoKolors.Settings.Tests;

public class SelectionListArgumentTests {

    [Fact]
    public void Ctor_ShouldInitializeProperly() {
        var a = new SelectionListArgument<int>(1, 2 ,3);
        Assert.Equal([1, 2, 3], a.Choices);
    }

    [Fact]
    public void Ctor_ShouldInitializeProperly_DefaultValues() {
        var a = new SelectionListArgument<int>([1, 2, 3], 1, 2);
        Assert.Equal([1, 2, 3], a.Choices);
        Assert.Equal([1, 2], a.DefaultSelected);
        Assert.Equal([1, 2], a.Value);

        Assert.Throws<InvalidArgumentInputException>(() => {
            var b = new SelectionListArgument<int>([1, 2, 3], 4);
        });
    }

    [Fact]
    public void Ctor_ShouldInitializeProperly_FromEnum() {
        var a = SelectionListArgument<int>.FromEnum<DummyEnum>();
        Assert.Equal([A, B, C, D], a.Choices);
        a = SelectionListArgument<int>.FromEnum(A, B);
        Assert.Equal([A, B, C, D], a.Choices);
        Assert.Equal([A, B], a.DefaultSelected);
        Assert.Equal([A, B], a.Value);
    }

    [Theory]
    [InlineData(new[] {A, B}, new[] {A, B, C, D})]
    [InlineData(new[] {A, B, C, D, A, B}, new DummyEnum[] { })]
    [InlineData(new DummyEnum[] { }, new[] {A, B})]
    internal void Reset_ShouldWorkProperly(DummyEnum[] values, DummyEnum[] defaultValues) {
        var a = SelectionListArgument<int>.FromEnum(defaultValues);
        a.Set(values);
        var l = new List<DummyEnum>();
        l.AddRange(defaultValues);
        l.AddRange(values);
        Assert.Equal(l.ToArray(), a.Value);
        a.Reset();
        Assert.Equal(defaultValues, a.Value);
    }

    [Fact]
    public void Set_ShouldWorkProperly() {
        var a = new SelectionListArgument<int>(1, 2, 3);
        a.Set(1);
        a.Set(2);
        Assert.Equal([1, 2], a.Value);
        a.Reset();
    }
}