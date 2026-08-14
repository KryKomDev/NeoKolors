// NeoKolors
// Copyright (c) 2025 KryKom

using static System.IO.Path;
using System.Text;
using System.Text.RegularExpressions;

namespace NeoKolors.Console;

/// <summary>
/// Represents the configuration for logging files, including file path, naming, and creation strategies.
/// </summary>
public readonly record struct LogFileConfig {

    public LogFileConfigType Config { get; init; }
    public string            Path   { get; init; }

    public LogFileConfig(LogFileConfigType config, string path) {
        Config = config;
        Path   = path ?? string.Empty;
    }

    /// <summary>
    /// Represents a configuration with a custom <see cref="TextWriter"/> for the logger.
    /// </summary>
    public static LogFileConfig Custom() => new(LogFileConfigType.CUSTOM, string.Empty);

    /// <summary>
    /// Creates a log file configuration that replaces the content of the file at the specified path.
    /// </summary>
    /// <param name="path">The full file path of the log file to be replaced.</param>
    public static LogFileConfig Replace(string path) => new(LogFileConfigType.REPLACE, path);

    /// <summary>
    /// Creates a log file configuration that appends new log content to the existing file at the specified path.
    /// </summary>
    /// <param name="path">The full file path of the log file where new content will be appended.</param>
    public static LogFileConfig Append(string path) => new(LogFileConfigType.APPEND, path);

    /// <summary>
    /// Creates a log file configuration that generates a new file with a sequential count appended to the file name.
    /// </summary>
    /// <param name="path">The file path template (e.g. "log_{0}.txt" or "app.log").</param>
    public static LogFileConfig NewCount(string path) => new(LogFileConfigType.NEW_COUNT, path);

    /// <summary>
    /// Creates a log file configuration that generates a new log file with the current datetime appended to the file name.
    /// </summary>
    /// <param name="path">The base file path or template.</param>
    public static LogFileConfig NewDatetime(string path) => new(LogFileConfigType.NEW_DATETIME, path);

    /// <summary>
    /// Creates a log file configuration that generates a new file using a hash and timestamp in the filename.
    /// </summary>
    /// <param name="path">The base file path or template.</param>
    public static LogFileConfig NewHash(string path) => new(LogFileConfigType.NEW_HASH_DATETIME, path);

    /// <summary>
    /// Creates and returns a <see cref="FileStream"/> based on the configured file mode and path.
    /// </summary>
    public FileStream CreateStream() {
        if (Config == LogFileConfigType.CUSTOM || string.IsNullOrWhiteSpace(Path)) {
            throw new InvalidOperationException("Cannot create a file stream for a Custom or empty LogFileConfig.");
        }

        string  targetPath = ResolveResolvedPath();
        string? directory  = GetDirectoryName(targetPath);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) {
            Directory.CreateDirectory(directory);
        }

        var mode = Config switch {
            LogFileConfigType.APPEND => FileMode.Append,
            _                        => FileMode.Create
        };

        return new FileStream(targetPath, mode, FileAccess.Write, FileShare.Read);
    }

    /// <summary>
    /// Creates and returns a <see cref="TextWriter"/> instance based on the configuration.
    /// </summary>
    public TextWriter CreateOutput() {
        var fs = CreateStream();

        return new StreamWriter(fs, Encoding.UTF8) { AutoFlush = true };
    }

    /// <summary>
    /// Resolves the actual target file path based on the configuration strategy.
    /// </summary>
    public string ResolveResolvedPath() {
        return Config switch {
            LogFileConfigType.REPLACE           => Path,
            LogFileConfigType.APPEND            => Path,
            LogFileConfigType.NEW_COUNT         => FormatPathWithSuffix(GetNextCount().ToString()),
            LogFileConfigType.NEW_DATETIME      => FormatPathWithSuffix($"{DateTime.Now:yyyy.MM.dd-HH.mm.ss}"),
            LogFileConfigType.NEW_HASH_DATETIME => FormatPathWithSuffix($"{DateTime.Now:yyyyMMdd_HHmmss}_{Path.GetHashCode():X8}"),
            _                                   => Path
        };
    }

    private string FormatPathWithSuffix(string suffix) {
        if (string.IsNullOrEmpty(Path))
            return suffix;

        if (Path.Contains("{0}")) {
            return string.Format(Path, suffix);
        }

        string dir           = GetDirectoryName(Path) ?? string.Empty;
        string fileNameNoExt = GetFileNameWithoutExtension(Path);
        string ext           = GetExtension(Path);

        string formattedFileName = $"{fileNameNoExt}_{suffix}{ext}";

        return string.IsNullOrEmpty(dir) ? formattedFileName : Combine(dir, formattedFileName);
    }

    private int GetNextCount() {
        string dir = GetDirectoryName(Path) ?? string.Empty;

        if (string.IsNullOrEmpty(dir)) {
            dir = Directory.GetCurrentDirectory();
        }

        if (!Directory.Exists(dir)) {
            return 1;
        }

        string prefix;
        string suffix;

        if (Path.Contains("{0}")) {
            string fileName = GetFileName(Path);
            int    idx      = fileName.IndexOf("{0}", StringComparison.Ordinal);
            prefix = fileName[..idx];
            suffix = fileName[(idx + 3)..];
        }
        else {
            string fileNameNoExt = GetFileNameWithoutExtension(Path);
            string ext           = GetExtension(Path);
            prefix = fileNameNoExt + "_";
            suffix = ext;
        }

        string pattern = $"^{Regex.Escape(prefix)}(\\d+){Regex.Escape(suffix)}$";
        var    regex   = new Regex(pattern, RegexOptions.IgnoreCase);

        int maxCount = 0;

        try {
            var files = Directory.GetFiles(dir);

            foreach (var filePath in files) {
                string fileName = GetFileName(filePath);
                var    match    = regex.Match(fileName);

                if (!match.Success || !int.TryParse(match.Groups[1].Value, out int count)) {
                    continue;
                }

                if (count > maxCount) {
                    maxCount = count;
                }
            }
        }
        catch {
            // Ignore directory inspection failures
        }

        return maxCount + 1;
    }
}