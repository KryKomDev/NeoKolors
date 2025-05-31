+++
date = '2025-05-16T17:12:28+02:00'
draft = false
title = 'LogFileConfig'
+++

```c# {lineNos=false}
public struct LogFileConfig;
```

Controls the built-in configurations of log files.

---

## Custom

```c# {lineNos=false}
public static LogFileConfig Custom();
```

Creates a new log file configuration that signals that the output stream is defined by the user.

---

## Replace

```c# {lineNos=false}
public static LogFileConfig Replace(string path);
```

Creates a new log file configuration for a file at the specified path.
If the file already exists it will be replaced.

---

## Append

```c# {lineNos=false}
public static LogFileConfig Append(string path);
```

Creates a new log file configuration for a file at the specified path.
If the file already exists the logs will be appended to the end of the file.

---

## NewCount

```c# {lineNos=false}
public static LogFileConfig NewCount(string path);
```

Creates a new log file configuration for a file at the specified formatted path.
As the log files are created they are sequentially numbered.

An example input path could be `"some/path/to/log/log-{0}.log"` and some of the 
created files could be `"log-1.log"` or `"log-23.log"`

---

## NewHash

```c# {lineNos=false}
public static LogFileConfig NewHash(string path);
```

Creates a new log file configuration for a file at the specified formatted path.
As the log files are created they are formatted with the has of the current datetime.

An example input path could be `"some/path/to/log/log-{0}.log"`.

---

## NewDate

```c# {lineNos=false}
public static LogFileConfig NewDate(string path);
```

Creates a new log file configuration for a file at the specified formatted path.
As the log files are created they are formatted with the datetime of their creation.

E.g. if the input path was `"some/path/to/log/log-{0}.log"` a file named 
`"log-25.5.29-13.28.34"` would be created (if the datetime was 29th of May 2025 13:28:34). 

---

## LogFileConfigType

```c# {lineNos=false}
public LogFileConfigType Config { get; set; }
```

Controls what type of log file is currently being used.

---

## Path

```c# {lineNos=false} 
public string Path { get; set; }
```

Contains the path to the log file.