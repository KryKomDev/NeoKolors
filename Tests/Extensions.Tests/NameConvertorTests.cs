namespace NeoKolors.Extensions.Tests;

public class NameConvertorTests {
    [Theory]
    [InlineData("hello_world", "HelloWorld")]
    [InlineData("simple_test", "SimpleTest")]
    [InlineData("multiple_word_test", "MultipleWordTest")]
    [InlineData("already_PascalCase", "AlreadyPascalCase")]
    [InlineData("single", "Single")]
    public void SnakeToPascal_ConvertsCorrectly(string input, string expected) {
        string result = input.SnakeToPascal();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("HelloWorld", "hello-world")]
    [InlineData("SimpleTest", "simple-test")]
    [InlineData("APIConfig", "a-p-i-config")]
    [InlineData("XML", "x-m-l")]
    [InlineData("Simple", "simple")]
    public void PascalToKebab_ConvertsCorrectly(string input, string expected) {
        string result = input.PascalToKebab();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("hello_world", "helloWorld")]
    [InlineData("simple_test", "simpleTest")]
    [InlineData("multiple_word_test", "multipleWordTest")]
    [InlineData("single", "single")]
    public void SnakeToCamel_ConvertsCorrectly(string input, string expected) {
        string result = input.SnakeToCamel();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("hello_world", "hello-world")]
    [InlineData("simple_test", "simple-test")]
    [InlineData("multiple_word_test", "multiple-word-test")]
    [InlineData("single", "single")]
    public void SnakeToKebab_ConvertsCorrectly(string input, string expected) {
        string result = input.SnakeToKebab();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("hello_world", "hello.world")]
    [InlineData("simple_test", "simple.test")]
    [InlineData("multiple_word_test", "multiple.word.test")]
    [InlineData("single", "single")]
    public void ToDotCase_ConvertsCorrectly(string input, string expected) {
        string result = input.ToDotCase();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("hello_world", "Hello World")]
    [InlineData("simple_test", "Simple Test")]
    [InlineData("multiple_word_test", "Multiple Word Test")]
    [InlineData("single", "Single")]
    public void SnakeToSpace_ConvertsCorrectly(string input, string expected) {
        string result = input.SnakeToSpace();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("helloWorld", "hello-world")]
    [InlineData("simpleTest", "simple-test")]
    [InlineData("multipleWordTest", "multiple-word-test")]
    [InlineData("single", "single")]
    [InlineData("APIConfig", "a-p-i-config")]
    public void KebabFromCamel_ConvertsCorrectly(string input, string expected) {
        string result = input.CamelToKebab();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("HelloWorld", "hello_world")]
    [InlineData("SimpleTest", "simple_test")]
    [InlineData("MultipleWordTest", "multiple_word_test")]
    [InlineData("Single", "single")]
    [InlineData("APIConfig", "a_p_i_config")]
    public void PascalToSnake_ConvertsCorrectly(string input, string expected) {
        string result = input.PascalToSnake();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("HELLO_WORLD", "hello-world")]
    [InlineData("SIMPLE_TEST", "simple-test")]
    [InlineData("MULTIPLE_WORD_TEST", "multiple-word-test")]
    [InlineData("SINGLE", "single")]
    [InlineData("API_CONFIG", "api-config")]
    public void EnumToKebab_ConvertsCorrectly(string input, string expected) {
        string result = input.EnumToKebab();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AllMethods_HandleEmptyAndWhitespaceStrings(string input) {
        // These shouldn't throw exceptions
        _ = input.SnakeToPascal();
        _ = input.PascalToSnake();
        _ = input.SnakeToCamel();
        _ = input.SnakeToKebab();
        _ = input.ToDotCase();
        _ = input.SnakeToSpace();
        _ = input.CamelToKebab();
        _ = input.PascalToKebab();
        _ = input.EnumToKebab();
    }

    [Fact]
    public void ConversionChain_ProducesExpectedResults() {
        const string original = "hello_world_test";
        
        // Snake -> Pascal -> Snake -> Kebab
        string result = original.SnakeToPascal();
        result = result.PascalToSnake();
        result = result.SnakeToKebab();
            
        Assert.Equal("hello-world-test", result);
    }

    [Theory]
    [InlineData("hello world", "helloWorld")]
    [InlineData("simple test case", "simpleTestCase")]
    [InlineData("single", "single")]
    public void SpaceToCamel_ConvertsCorrectly(string input, string expected) {
        Assert.Equal(expected, input.SpaceToCamel());
    }

    [Theory]
    [InlineData("HELLO_WORLD", "Hello World")]
    [InlineData("SIMPLE_TEST", "Simple Test")]
    [InlineData("SINGLE", "Single")]
    public void EnumToSpace_ConvertsCorrectly(string input, string expected) {
        Assert.Equal(expected, input.EnumToSpace());
    }

    [Theory]
    [InlineData("helloWorld", NamingCase.CAMEL, NamingCase.PASCAL, "HelloWorld")]
    [InlineData("HelloWorld", NamingCase.PASCAL, NamingCase.SNAKE, "hello_world")]
    [InlineData("hello_world", NamingCase.SNAKE, NamingCase.SCREAMING_SNAKE, "HELLO_WORLD")]
    [InlineData("HELLO_WORLD", NamingCase.SCREAMING_SNAKE, NamingCase.KEBAB, "hello-world")]
    [InlineData("hello-world", NamingCase.KEBAB, NamingCase.SPACED_CAMEL, "hello World")]
    [InlineData("hello World", NamingCase.SPACED_CAMEL, NamingCase.TRAIN, "HELLO-WORLD")]
    [InlineData("HELLO-WORLD", NamingCase.TRAIN, NamingCase.CAMEL, "helloWorld")]
    public void NameConvertor_Convert_WorksBetweenAllCases(string input, NamingCase source, NamingCase target, string expected) {
        Assert.Equal(expected, NameConvertor.Convert(input, source, target));
    }
}