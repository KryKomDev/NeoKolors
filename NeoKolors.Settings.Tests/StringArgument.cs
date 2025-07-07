//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Argument.Exception;
using static NeoKolors.Settings.Argument.StringFeatures;

namespace NeoKolors.Settings.Tests;

public class StringArgumentTests {

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("asd")]
    public void Ctor_ShouldInitializeProperly(string @default) {
        var s = new StringArgument(defaultValue: @default, allowedStringFeatures: ALL);
    }

    [Theory]
    [InlineData("a", NO_LOWER)]
    [InlineData("A", LOWER)]
    [InlineData("a", UPPER)]
    [InlineData("A", NO_UPPER)]
    [InlineData("s", SPACES)]
    [InlineData(" ", NO_SPACES)]
    [InlineData("n", NEWLINES)]
    [InlineData("\n", NO_NEWLINES)]
    [InlineData("!", NO_SPECIAL)]
    [InlineData("@", NO_SPECIAL)]
    [InlineData("#", NO_SPECIAL)]
    [InlineData("$", NO_SPECIAL)]
    [InlineData("%", NO_SPECIAL)]
    [InlineData("^", NO_SPECIAL)]
    [InlineData("&", NO_SPECIAL)]
    [InlineData("*", NO_SPECIAL)]
    [InlineData("(", NO_SPECIAL)]
    [InlineData(")", NO_SPECIAL)]
    [InlineData("-", NO_SPECIAL)]
    [InlineData("_", NO_SPECIAL)]
    [InlineData("+", NO_SPECIAL)]
    [InlineData("=", NO_SPECIAL)]
    [InlineData("[", NO_SPECIAL)]
    [InlineData("]", NO_SPECIAL)]
    [InlineData("{", NO_SPECIAL)]
    [InlineData("}", NO_SPECIAL)]
    [InlineData("/", NO_SPECIAL)]
    [InlineData("\\", NO_SPECIAL)]
    [InlineData(";", NO_SPECIAL)]
    [InlineData(",", NO_SPECIAL)]
    [InlineData(".", NO_SPECIAL)]
    [InlineData("<", NO_SPECIAL)]
    [InlineData(">", NO_SPECIAL)]
    [InlineData("?", NO_SPECIAL)]
    [InlineData(":", NO_SPECIAL)]
    [InlineData("\"", NO_SPECIAL)]
    [InlineData("'", NO_SPECIAL)]
    [InlineData("|", NO_SPECIAL)]
    [InlineData("~", NO_SPECIAL)]
    [InlineData("s", SPECIAL)]
    [InlineData("0", NO_NUMBERS)]
    [InlineData("1", NO_NUMBERS)]
    [InlineData("2", NO_NUMBERS)]
    [InlineData("3", NO_NUMBERS)]
    [InlineData("4", NO_NUMBERS)]
    [InlineData("5", NO_NUMBERS)]
    [InlineData("6", NO_NUMBERS)]
    [InlineData("7", NO_NUMBERS)]
    [InlineData("8", NO_NUMBERS)]
    [InlineData("9", NO_NUMBERS)]
    [InlineData("n", NUMBERS)]
    public void Set_InvalidInput_Feature_Throws(string value, StringFeatures features) {
        Assert.Throws<InvalidArgumentInputException>(() => 
            new StringArgument(defaultValue: value, allowedStringFeatures: features));
        
        var a = new StringArgument(allowedStringFeatures: features);
        Assert.Throws<InvalidArgumentInputException>(() => a.Set(value));
    }

    [Theory]
    [InlineData("ab", 0, 1)]
    [InlineData("a", 2, 3)]
    public void Set_InvalidInput_Length_Throws(string value, uint min, uint max) {
        Assert.Throws<InvalidArgumentInputException>(() => 
            new StringArgument(defaultValue: value, minLength: min, maxLength: max, allowedStringFeatures: ALL));
        
        var a = new StringArgument(minLength: min, maxLength: max, allowedStringFeatures: ALL);
        Assert.Throws<InvalidArgumentInputException>(() => a.Set(value));
    }
}