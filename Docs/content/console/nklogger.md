+++
date = '2025-05-16T17:12:28+02:00'
draft = false
title = 'NKLogger'
+++

```c# {lineNos=false}
public class NKLogger : IDisposable;
```

`NKLogger` class provides logger capabilities and configurations.

---

## Log methods

```c# {lineNos=false}
// basic string input
public void Fatal(string s);
public void Error(string s);
public void Warn(string s);
public void Log(string s);
public void Debug(string s);
public void Trace(string s);
    
// object to string input
public void Fatal(object s);
public void Error(object s);
public void Warn(object s);
public void Log(object s);
public void Debug(object s);
public void Trace(object s);

// format input. e.g. Log("{0} world", "Hello") would print "Hello World"
public void Fatal([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args);
public void Error([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args);
public void Warn([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args);
public void Info([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args);
public void Debug([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args);
public void Trace([StringSyntax(StringSyntaxAttribute.CompositeFormat)] string s, params object[] args);
```

`Fatal`, `Error`, `Warn`, `Log`, `Debug` and `Trace` are methods for 
printing messages of different log levels to the log output stream.

![logs](/images/logs.png)

---

## Configuration

```c# {lineNos=false}
public LoggerConfig Config { get; set; }
```

All configuration is stored in the `Config` property. 
All the properties of [`LoggerConfig`](/console/loggerconfig) are also delegated 
in the `NKLogger`.