// NeoKolors
// Copyright (c) KryKom 2026

using NeoKolors.Console;

namespace NeoKolors.Tui.Fonts.Serialization;

public class NKFontDeserializationResult {
    public NKFont? Font { get; }
    public IReadOnlyList<string> Errors { get; }
    public IReadOnlyList<string> Warnings { get; }
    public IReadOnlyList<string> Infos { get; }

    public bool Success => Font != null && Errors.Count == 0;

    public NKFontDeserializationResult(NKFont? font, IReadOnlyList<string> errors, IReadOnlyList<string> warnings, IReadOnlyList<string> infos) {
        Font = font;
        Errors = errors;
        Warnings = warnings;
        Infos = infos;
    }
}

internal class DeserializationContext {
    private static readonly AsyncLocal<DeserializationContext?> CURRENT_CONTEXT = new();

    public static DeserializationContext? Current => CURRENT_CONTEXT.Value;

    public static IDisposable Enter(DeserializationContext context) {
        var previous = CURRENT_CONTEXT.Value;
        CURRENT_CONTEXT.Value = context;
        return new DisposableAction(() => CURRENT_CONTEXT.Value = previous);
    }

    public List<string> Errors { get; } = new();
    public List<string> Warnings { get; } = new();
    public List<string> Infos { get; } = new();

    private class DisposableAction : IDisposable {
        private readonly Action _action;
        public DisposableAction(Action action) => _action = action;
        public void Dispose() => _action();
    }
}

internal class FontSerializerLogger {
    private readonly NKLogger _logger = NKDebug.GetLogger("NKFontSerializer");

    public void Error(string message) {
        if (DeserializationContext.Current != null) {
            DeserializationContext.Current.Errors.Add(message);
        } else {
            _logger.Error(message);
        }
    }

    public void Error(string format, params object[] args) {
        string message = string.Format(format, args);
        if (DeserializationContext.Current != null) {
            DeserializationContext.Current.Errors.Add(message);
        } else {
            _logger.Error(message);
        }
    }

    public void Warn(string message) {
        if (DeserializationContext.Current != null) {
            DeserializationContext.Current.Warnings.Add(message);
        } else {
            _logger.Warn(message);
        }
    }

    public void Warn(string format, params object[] args) {
        string message = string.Format(format, args);
        if (DeserializationContext.Current != null) {
            DeserializationContext.Current.Warnings.Add(message);
        } else {
            _logger.Warn(message);
        }
    }

    public void Info(string message) {
        if (DeserializationContext.Current != null) {
            DeserializationContext.Current.Infos.Add(message);
        } else {
            _logger.Info(message);
        }
    }

    public void Info(string format, params object[] args) {
        string message = string.Format(format, args);
        if (DeserializationContext.Current != null) {
            DeserializationContext.Current.Infos.Add(message);
        } else {
            _logger.Info(message);
        }
    }

    public void Crit(string message) {
        if (DeserializationContext.Current != null) {
            DeserializationContext.Current.Errors.Add("CRITICAL: " + message);
        } else {
            _logger.Crit(message);
        }
    }

    public void Crit(string format, params object[] args) {
        string message = string.Format(format, args);
        if (DeserializationContext.Current != null) {
            DeserializationContext.Current.Errors.Add("CRITICAL: " + message);
        } else {
            _logger.Crit(message);
        }
    }
}

public static partial class NKFontSerializer {
    /// <summary>
    /// Functional XML font deserializer that avoids direct side-effects during execution and returns all collected diagnostic logs as values.
    /// </summary>
    public static NKFontDeserializationResult TryDeserializeXml(string path) {
        var context = new DeserializationContext();
        using (DeserializationContext.Enter(context)) {
            NKFont? font = null;
            try {
                font = DeserializeXml(path);
            }
            catch (Exception ex) {
                context.Errors.Add($"Unhandled exception during deserialization: {ex.Message}\n{ex.StackTrace}");
            }
            return new NKFontDeserializationResult(font, context.Errors, context.Warnings, context.Infos);
        }
    }

    /// <summary>
    /// Functional XML font deserializer that avoids direct side-effects during execution and returns all collected diagnostic logs as values.
    /// </summary>
    public static async Task<NKFontDeserializationResult> TryDeserializeXmlAsync(string path) {
        var context = new DeserializationContext();
        using (DeserializationContext.Enter(context)) {
            NKFont? font = null;
            try {
                font = await DeserializeXmlAsync(path);
            }
            catch (Exception ex) {
                context.Errors.Add($"Unhandled exception during deserialization: {ex.Message}\n{ex.StackTrace}");
            }
            return new NKFontDeserializationResult(font, context.Errors, context.Warnings, context.Infos);
        }
    }

    /// <summary>
    /// Functional XML font archive deserializer that avoids direct side-effects during execution and returns all collected diagnostic logs as values.
    /// </summary>
    public static NKFontDeserializationResult TryDeserializeXmlArchive(Stream archive) {
        var context = new DeserializationContext();
        using (DeserializationContext.Enter(context)) {
            NKFont? font = null;
            try {
                font = DeserializeXmlArchive(archive);
            }
            catch (Exception ex) {
                context.Errors.Add($"Unhandled exception during deserialization: {ex.Message}\n{ex.StackTrace}");
            }
            return new NKFontDeserializationResult(font, context.Errors, context.Warnings, context.Infos);
        }
    }

    /// <summary>
    /// Functional XML font archive deserializer that avoids direct side-effects during execution and returns all collected diagnostic logs as values.
    /// </summary>
    public static async Task<NKFontDeserializationResult> TryDeserializeXmlArchiveAsync(Stream archive) {
        var context = new DeserializationContext();
        using (DeserializationContext.Enter(context)) {
            NKFont? font = null;
            try {
                font = await DeserializeXmlArchiveAsync(archive);
            }
            catch (Exception ex) {
                context.Errors.Add($"Unhandled exception during deserialization: {ex.Message}\n{ex.StackTrace}");
            }
            return new NKFontDeserializationResult(font, context.Errors, context.Warnings, context.Infos);
        }
    }
}
