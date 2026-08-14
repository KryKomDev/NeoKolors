# Log File Configuration

**[LogFileConfig](file:///C:/Users/krystof/Desktop/projects/Libs/NeoKolors/Src/Console/Logger/LogFileConfig.cs)** controls how the log output is saved to the filesystem, configuring path mapping and file rotation models.

---

## 1. Stream Types and Constructors

You instantiate a `LogFileConfig` using one of the following static factory methods:

### 1.1 `Custom()`
Configures the logger to write to the user-supplied `Output` stream (usually stdout). This is the default.

### 1.2 `Replace(string path)`
Creates or overwrites a single file at the specified path. Any existing logs in that file are cleared.
```csharp
myLogger.Config.FileConfig = LogFileConfig.Replace("./app.log");
```

### 1.3 `Append(string path)`
Writes to a single file at the specified path. If the file already exists, new log entries are appended to the end.

---

## 2. Dynamic Rotation Models

To prevent files from becoming too large, you can configure paths to automatically generate unique filenames using one of these rotation models. The path parameter must contain a `{0}` format marker.

### 2.1 Sequential Count (`NewCount`)
As log files are created, they are sequentially numbered starting from 1:
```csharp
// Generates: "logs/app-1.log", "logs/app-2.log", etc.
myLogger.Config.FileConfig = LogFileConfig.NewCount("./logs/app-{0}.log");
```

### 2.2 Date/Time Stamps (`NewDate`)
Injects the creation timestamp into the filename:
```csharp
// Generates: "logs/app-2026-07-18_20-13-45.log"
myLogger.Config.FileConfig = LogFileConfig.NewDate("./logs/app-{0}.log");
```

### 2.3 Hash Signatures (`NewHash`)
Injects a hashed date/time signature into the filename.
