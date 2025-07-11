//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Common.Util;

public static class NameConvertor {
    
    /// <summary>
    /// converts a string from snake to pascal case, e.g. "hello_world" -> "HelloWorld"
    /// </summary>
    public static string SnakeToPascal(this string name) {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        return name.Replace('_', ' ')
                  .Split(' ')
                  .Select(x => x.First().ToString().ToUpper() + x.Substring(1))
                  .Aggregate((a, b) => a + b);
    }
    
    /// <summary>
    /// converts a string to snake case, e.g. "hello_world" -> "hello_world"
    /// </summary>
    public static string PascalToSnake(this string name) {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        return name.Aggregate("", (current, c) => current + (char.IsUpper(c) ? "-" + char.ToLower(c) : c))
                  .Substring(1);
    }
    
    /// <summary>
    /// converts a string from snake case to camel case, e.g. "hello_world" -> "helloWorld"
    /// </summary>
    public static string SnakeToCamel(this string name) {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        return name.Split('_')
                  .Select(x => x.First().ToString().ToUpper() + x.Substring(1))
                  .Aggregate((a, b) => a + b)
                  .DecapitalizeFirst();
    }

    /// <summary>
    /// converts a string from space case to camel case, e.g. "hello world" -> "helloWorld"
    /// </summary>
    public static string SpaceToCamel(this string name) {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        return name.Split(' ')
                   .Select(x => x.First().ToString().ToUpper() + x.Substring(1))
                   .Aggregate((a, b) => a + b)
                   .DecapitalizeFirst();
    }
    
    /// <summary>
    /// converts a string from snake case to kebab case, e.g. "hello_world" -> "hello-world"
    /// </summary>
    public static string SnakeToKebab(this string name) {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        return name.Split('_')
                  .Select(x => x.First().ToString().ToLower() + x.Substring(1))
                  .Aggregate((a, b) => a + '-' + b);
    }

    /// <summary>
    /// Converts a string from snake case to dot case, e.g. "hello_world" -> "hello.world".
    /// </summary>
    public static string ToDotCase(this string name) {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        return name.Split('_')
                   .Select(x => x.First().ToString().ToLower() + x.Substring(1))
                   .Aggregate((a, b) => a + '.' + b);
    }
    
    /// <summary>
    /// converts a string from snake case to space case, e.g. "hello_world" -> "Hello World"
    /// </summary>
    public static string SnakeToSpace(this string name) {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        return name.Split('_')
                   .Select(x => x.First().ToString().ToUpper() + x.Substring(1))
                   .Aggregate((a, b) => a + ' ' + b);
    }

    /// <summary>
    /// Separates a Pascal-cased enum name into individual words separated by spaces, e.g. "EnumValueName" -> "Enum Value Name".
    /// </summary>
    /// <param name="name">The Pascal-cased enum name to be converted.</param>
    /// <returns>A string where individual words are separated by spaces.</returns>
    public static string EnumToSpace(this string name) {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        return name.Split('_')
                   .Select(x => x.ToLower().CapitalizeFirst())
                   .Join(" ");
    }

    /// <summary>
    /// converts a string from camel case to snake case, e.g. "helloWorld" -> "hello-world"
    /// </summary>
    public static string CamelToKebab(this string name) {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        return name.Aggregate("", (current, c) => 
            current + (current.Length != 0 ? (char.IsUpper(c) ? "-" + char.ToLower(c) : c) : char.ToLower(c)));
    }

    /// <summary>
    /// converts a string from camel case to kebab case, e.g. "HelloWorld" -> "hello-world"
    /// </summary>
    public static string PascalToKebab(this string name) {
        if (string.IsNullOrWhiteSpace(name)) return string.Empty;
        return name.Aggregate("", (current, c) => current + (char.IsUpper(c) ? "-" + char.ToLower(c) : c))
                  .Substring(1);
    }
    
    /// <summary>
    /// converts a string from enum case to kebab case, e.g. "HELLO_WORLD" -> "hello-world"
    /// </summary>
    public static string EnumToKebab(this string name) {
        return string.IsNullOrWhiteSpace(name) ? string.Empty : name.Replace('_', '-').ToLower();
    }
}