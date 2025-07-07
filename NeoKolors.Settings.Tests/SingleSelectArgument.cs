//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Argument.Exception;
using static NeoKolors.Settings.Tests.DummyEnum;

namespace NeoKolors.Settings.Tests;

public class SingleSelectArgumentTests {

    [Theory]
    [InlineData(new[] {A, B, C, D}, 3)]
    [InlineData(new[] {A, B}, 0)]
    public void Ctor_IndexDefault_ShouldInitializeProperly(DummyEnum[] values, int index) {
        var a = new SingleSelectArgument<DummyEnum>(values, index);
        Assert.Equal(values, a.Options);
        Assert.Equal(index, a.Index);
        Assert.Equal(values[index], a.Value);
    }

    [Theory]
    [InlineData(new[] {A, B, C, D}, B)]
    [InlineData(new[] {A, B}, A)]
    public void Ctor_ValueDefault_ShouldInitializeProperly(DummyEnum[] values, DummyEnum @default) {
        var a = new SingleSelectArgument<DummyEnum>(values, @default);
        Assert.Equal(values, a.Options);
        Assert.Equal(@default, a.Value);
    }

    [Theory]
    [InlineData(new[] {A, C, D}, B)]
    [InlineData(new DummyEnum[] { }, A)]
    public void Ctor_ValueNotContained_Throws(DummyEnum[] values, DummyEnum @default) {
        Assert.Throws<InvalidArgumentInputException>(() => new SingleSelectArgument<DummyEnum>(values, @default));
    }

    [Fact]
    public void FromEnum_ShouldWorkProperly() {
        var a = SingleSelectArgument<DummyEnum>.FromEnum<DummyEnum>();
        Assert.Equal([A, B, C, D], a.Options);
    }

    [Theory]
    [InlineData(A)]
    [InlineData(B)]
    [InlineData(C)]
    [InlineData(D)]
    public void Set_T_ShouldWorkProperly(DummyEnum value) {
        var a = SingleSelectArgument<DummyEnum>.FromEnum<DummyEnum>();
        a.Set(value);
        Assert.Equal(value, a.Value);
    }

    [Theory]
    [InlineData(A, B)]
    [InlineData(A, A)]
    [InlineData(D, C)]
    public void Reset_ShouldWorkProperly(DummyEnum value, DummyEnum @default) {
        var a = SingleSelectArgument<DummyEnum>.FromEnum(@default);
        Assert.Equal(@default, a.Value);
        a.Set(value);
        Assert.Equal(value, a.Value);
        a.Reset();
        Assert.Equal(@default, a.Value);
    }
}