# Debug.Exceptions

There are five ways to make an exception fancy:
1. `PrintException`
2. `Throw`
3. `ToFancy`
4. `ToFancyString`
5. enabling exception interruption with `EnableAutoFancy`

## PrintException

```c#
public static void PrintException(Exception e);
```

Makes the exception fancy and writes it to the console.

<note>
    This does not actually throw the exception.
</note>

## Throw

```c#
public static void Throw(Exception e);
```

<warning>Not recommended as it changes the type of the exception.</warning>

Makes the exception a `FancyException<T>` where `T` is the type of the original
exception.

## ToFancy

```c#
public static Exception ToFancy(Exception e);
```

Creates a new `FancyException<>` from the input and returns it.

## ToFancyString

```c#
public static string ToFancyString(Exception e);
```
Returns a fancy string from the exception.

## Enabling `EnableAutoFancy`

If `Debug.EnableAutoFancy` is true, the library interrupts all unhandled 
exceptions and prints them to the console. You don't have to worry about 
the type changing as the exception is unhandled.

## Catching `FancyException<>`

The following example shows how you can catch a fancy exception:

```c#
try 
{
    // some code that can throw a fancy exception of some type
}
catch (FancyException<ArithmeticException>) 
{
    // handling...
}
catch (FacnyException<DivideByZeroException>) 
{
    // handling...
}
// ...
```