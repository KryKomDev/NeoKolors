//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using System.Diagnostics.Contracts;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

namespace NeoKolors.Console;

/// <summary>
/// Represents the configuration for logging files, including file path, name, and handling behavior.
/// </summary>
public struct LogFileConfig {
    private const string CONFIG_FILE_NAME = ".nklog";
    
    public LogFileConfigType Config { get; set; }
    public string Path { get; set; }
   
    /// <summary>
    /// Represents a configuration with a custom <see cref="TextWriter"/> for the logger.
    /// </summary>
    public static LogFileConfig Custom() => new(LogFileConfigType.CUSTOM, "");

    /// <summary>
    /// Creates a log file configuration that replaces the content of the file at the specified path.
    /// </summary>
    /// <param name="path">The full file path of the log file to be replaced.</param>
    /// <returns>
    /// A <see cref="LogFileConfig"/> instance with configuration type set to replace the file.
    /// </returns>
    public static LogFileConfig Replace(string path) => new(LogFileConfigType.REPLACE, path);

    /// <summary>
    /// Creates a log file configuration that appends new log content
    /// to the existing file at the specified path.
    /// </summary>
    /// <param name="path">
    /// The full file path of the log file where new content will be appended.
    /// </param>
    /// <returns>
    /// A <see cref="LogFileConfig"/> instance with configuration
    /// type set to append content to the file.
    /// </returns>
    public static LogFileConfig Append(string path) => new(LogFileConfigType.APPEND, path);

    /// <summary>
    /// Creates a log file configuration that generates a new
    /// file with a sequential count appended to the file name at the specified path.
    /// </summary>
    /// <param name="path">
    /// The full file path for the new log file containing the sequential count.
    /// </param>
    /// <returns>
    /// A <see cref="LogFileConfig"/> instance with configuration
    /// type set to create files with a sequential count.
    /// </returns>
    public static LogFileConfig NewCount(string path) => new(LogFileConfigType.NEW_COUNT, path);

    /// <summary>
    /// Creates a log file configuration that generates a new log
    /// file with the current datetime appended to the specified path.
    /// </summary>
    /// <param name="path">
    /// The base file path to which the current datetime will be appended for the new log file.
    /// </param>
    /// <returns>
    /// A <see cref="LogFileConfig"/> instance with configuration
    /// type set to generate a datetime-based file name.
    /// </returns>
    public static LogFileConfig NewDatetime(string path) => new(LogFileConfigType.NEW_DATETIME, path);

    /// <summary>
    /// Creates a log file configuration that generates a new file
    /// using a hash and the current date-time in the filename.
    /// </summary>
    /// <param name="path">The base file path where the new log file will be created.</param>
    /// <returns>
    /// A <see cref="LogFileConfig"/> instance with configuration
    /// type set to create a new file using a hash and timestamp.
    /// </returns>
    public static LogFileConfig NewHash(string path) => new(LogFileConfigType.NEW_HASH_DATETIME, path);
    
    private LogFileConfig(LogFileConfigType config, string path) {
        Config = config;
        Path = path;
    }

    /// <summary>
    /// Creates and returns a TextWriter instance based on the configuration type
    /// specified in the LogFileConfig. This determines how the log file is created
    /// and written to, including behaviors for replacing, appending, and creating
    /// new files with various naming schemes.
    /// </summary>
    /// <returns>
    /// A TextWriter instance for writing to the configured log file.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when the directory for new file creation is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when an invalid or unsupported configuration type is provided.
    /// </exception>
    public TextWriter CreateOutput() {
        FileStream? fs;
        switch (Config) {
            case LogFileConfigType.REPLACE:
                fs = new FileStream(Path, FileMode.Create);
                break;
            case LogFileConfigType.APPEND:
                fs = new FileStream(Path, FileMode.Append);
                break;
            case LogFileConfigType.NEW_COUNT:
                string? directory = System.IO.Path.GetDirectoryName(Path);
                
                if (directory is null) 
                    throw new ArgumentNullException(nameof(directory));

                int n = 0;
                if (!Directory.Exists(directory)) 
                    CreateConfig(directory);                    
                else 
                    n = UpdateConfig(directory);

                fs = new FileStream(string.Format(Path, n), FileMode.Create);
                break;
            case LogFileConfigType.NEW_DATETIME:
                fs = new FileStream(string.Format(Path, $"{DateTime.Now:yy.MM.dd-hh.mm.ss}"), FileMode.Create);
                break;
            case LogFileConfigType.NEW_HASH_DATETIME:
                fs = new FileStream(string.Format(Path, DateTime.Now.GetHashCode()), FileMode.Create);
                break;
            case LogFileConfigType.CUSTOM:
            default: 
                throw new ArgumentOutOfRangeException();
        }
        
        return new StreamWriter(fs);
    }

    private static void CreateConfig(string directory, int val = -1) {
        if (!Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }

        string file = System.IO.Path.Combine(directory, CONFIG_FILE_NAME);
        var fs = new FileStream(file, FileMode.OpenOrCreate);
        File.SetAttributes(file, FileAttributes.Hidden);
        var jw = JsonReaderWriterFactory.CreateJsonWriter(fs);
        jw.WriteStartElement("root");
        jw.WriteValue(val);
        jw.WriteEndElement();
        jw.Flush();
        jw.Close();
        fs.Close();
    }

    [Pure]
    private static int LoadConfig(string directory) {
        if (!Directory.Exists(directory)) {
            return 0; 
        }
        
        var filePath = System.IO.Path.Combine(directory, CONFIG_FILE_NAME);
        
        if (!File.Exists(filePath))
            return 0;
        
        var js = new JsonSerializer();
        var sr = new StreamReader(filePath);
        var jsr = new JsonTextReader(sr);
        var res = js.Deserialize<int>(jsr);
        sr.Close();
        jsr.Close();
        return res;
    }

    
    private static int UpdateConfig(string directory) {
        int n = LoadConfig(directory) + 1;
        CreateConfig(directory, n);
        return n;
    }
}