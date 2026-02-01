# Drivers.md

The NeoKolors.Console.Driver namespace contains the drivers for reading from and writing to the VT.

## Structure

The drivers are separated into input drivers and output drivers.

### Input Drivers

The `IInputDriver` interface defines the methods and properties for the input drivers.
There will be an `InputDriver` class that combines the individual drivers for different 
platforms.

The Driver will then invoke events in NKConsole.