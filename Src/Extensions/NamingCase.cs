// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Extensions;

public enum NamingCase {
        
    /// <summary>
    /// Represents a naming convention where words are concatenated without spaces,
    /// and each word starts with an uppercase letter except the first word, which starts with a lowercase letter.
    /// </summary>
    /// <example>helloWorld</example>
    CAMEL,
        
    /// <summary>
    /// Represents a naming convention where words are concatenated without spaces,
    /// and each word starts with an uppercase letter, including the first word.
    /// </summary>
    /// <example>HelloWorld</example> 
    PASCAL,
        
    /// <summary>
    /// Represents a naming convention where words are concatenated using underscores as separators,
    /// with all letters in lowercase.
    /// </summary>
    /// <example>hello_world</example>
    SNAKE,
        
    /// <summary>
    /// Represents a naming convention where words are written in uppercase letters
    /// and separated by underscores, commonly used for constants or enums.
    /// </summary>
    /// <example>HELLO_WORLD</example>
    SCREAMING_SNAKE,

    /// <summary>
    /// Represents a naming convention where words are concatenated using hyphens ('-'),
    /// with all letters typically in lowercase.
    /// </summary>
    /// <example>hello-world</example>
    KEBAB,

    /// <summary>
    /// Represents a naming convention where words are separated by spaces,
    /// and each word starts with an uppercase letter except the first word, which starts with a lowercase letter.
    /// </summary>
    /// <example>Hello World</example>
    SPACED_CAMEL,

    /// <summary>
    /// Represents a naming convention where words are separated by hyphens,
    /// and each word starts with an uppercase letter.
    /// </summary>
    /// <example>HELLO-WORLD</example>
    TRAIN,
}