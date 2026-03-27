namespace NeoKolors.Extensions.Tests;

using NeoKolors.Extensions;

public class UniversalNamingConvertorTests {

    [Theory]
    [InlineData("helloWorld", UniversalNamingConvertor.NamingCase.CAMEL, UniversalNamingConvertor.NamingCase.PASCAL, "HelloWorld")]
    [InlineData("HelloWorld", UniversalNamingConvertor.NamingCase.PASCAL, UniversalNamingConvertor.NamingCase.SNAKE, "hello_world")]
    [InlineData("hello_world", UniversalNamingConvertor.NamingCase.SNAKE, UniversalNamingConvertor.NamingCase.SCREAMING_SNAKE, "HELLO_WORLD")]
    [InlineData("HELLO_WORLD", UniversalNamingConvertor.NamingCase.SCREAMING_SNAKE, UniversalNamingConvertor.NamingCase.KEBAB, "hello-world")]
    [InlineData("hello-world", UniversalNamingConvertor.NamingCase.KEBAB, UniversalNamingConvertor.NamingCase.SPACED_CAMEL, "hello World")]
    [InlineData("hello World", UniversalNamingConvertor.NamingCase.SPACED_CAMEL, UniversalNamingConvertor.NamingCase.TRAIN, "HELLO-WORLD")]
    [InlineData("HELLO-WORLD", UniversalNamingConvertor.NamingCase.TRAIN, UniversalNamingConvertor.NamingCase.CAMEL, "helloWorld")]
    public void Convert_WorksBetweenCases(string input, UniversalNamingConvertor.NamingCase source, UniversalNamingConvertor.NamingCase target, string expected) {
        Assert.Equal(expected, input.Convert(source, target));
    }
}
