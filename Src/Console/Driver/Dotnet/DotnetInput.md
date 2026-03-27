# Dotnet Input Driver

The Dotnet input driver should use only the available `System.Console` methods
and should work only by handling characters and escape sequences.

## Explanation 

The driver has the same intercept thread loop as the other drivers.

When it receives, it checks if it is greater or equal to 64. In that case
the character can be directly translated and the corresponding event can 
be invoked [^1]. Otherwise, 64 is added to the character and it is interpreted
as an event with the control key active. If the character is 27 (escape), however,
an ANSI parser will take over and try to parse the escape sequence.

## ANSI Parsing

The ANSI parser

[^1]: If the interception is made using the `Console.ReadKey` method, no
translation is needed.