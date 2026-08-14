// NeoKolors
// Copyright (c) krystof 2026

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Polyfills;

namespace NeoKolors.Console;

/// <summary>
/// Extension methods for configuring NeoKolors logging with <see cref="ILoggingBuilder"/> and <see cref="ILoggerFactory"/>.
/// </summary>
public static class NKLoggingBuilderExtensions {

    /// <summary>
    /// Adds NeoKolors logging provider to the <see cref="ILoggingBuilder"/>.
    /// </summary>
    public static ILoggingBuilder AddNeoKolors(this ILoggingBuilder builder) {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, NKLoggerProvider>());

        return builder;
    }

    /// <summary>
    /// Adds NeoKolors logging provider to the <see cref="ILoggingBuilder"/> with custom logger options.
    /// </summary>
    public static ILoggingBuilder AddNeoKolors(this ILoggingBuilder builder, Action<NKLoggerOptions> configure) {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(configure);

        var options = new NKLoggerOptions();
        configure(options);

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, NKLoggerProvider>(
                _ => new NKLoggerProvider(
                    options
                )
            )
        );

        return builder;
    }

    /// <summary>
    /// Adds NeoKolors logging provider to the <see cref="ILoggingBuilder"/> with custom logger options instance.
    /// </summary>
    public static ILoggingBuilder AddNeoKolors(this ILoggingBuilder builder, NKLoggerOptions options) {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(options);

        builder.Services.TryAddEnumerable(
            ServiceDescriptor.Singleton<ILoggerProvider, NKLoggerProvider>(
                _ => new NKLoggerProvider(
                    options
                )
            )
        );

        return builder;
    }

    /// <summary>
    /// Adds NeoKolors logging provider to the <see cref="ILoggingBuilder"/> with custom ANSI logger configuration.
    /// </summary>
    public static ILoggingBuilder AddNeoKolors(this ILoggingBuilder builder, Action<AnsiLoggerConfig> configureAnsi) {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        if (configureAnsi == null)
            throw new ArgumentNullException(nameof(configureAnsi));

        var ansiConfig = new AnsiLoggerConfig();
        configureAnsi(ansiConfig);

        var options = new NKLoggerOptions {
            Writer = new AnsiLogWriter(ansiConfig)
        };

        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, NKLoggerProvider>(_ => new NKLoggerProvider(options)));

        return builder;
    }

    /// <summary>
    /// Adds NeoKolors logging provider to the <see cref="ILoggerFactory"/>.
    /// </summary>
    public static ILoggerFactory AddNeoKolors(this ILoggerFactory factory, NKLoggerOptions? options = null) {
        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        factory.AddProvider(new NKLoggerProvider(options ?? new NKLoggerOptions()));

        return factory;
    }

    /// <summary>
    /// Adds NeoKolors logging provider to the <see cref="ILoggerFactory"/> with custom ANSI logger configuration.
    /// </summary>
    public static ILoggerFactory AddNeoKolors(this ILoggerFactory factory, Action<AnsiLoggerConfig> configureAnsi) {
        if (factory == null)
            throw new ArgumentNullException(nameof(factory));

        if (configureAnsi == null)
            throw new ArgumentNullException(nameof(configureAnsi));

        var ansiConfig = new AnsiLoggerConfig();
        configureAnsi(ansiConfig);

        factory.AddProvider(
            new NKLoggerProvider(
                new NKLoggerOptions {
                    Writer = new AnsiLogWriter(ansiConfig)
                }
            )
        );

        return factory;
    }

    /// <summary>
    /// Adds NeoKolors logging provider configured to write to a log file.
    /// </summary>
    public static ILoggingBuilder AddNeoKolorsFile(this ILoggingBuilder builder, LogFileConfig fileConfig, TextLoggerConfig? textConfig = null) {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        return builder.AddNeoKolors(options => options.UseFileLogging(fileConfig, textConfig));
    }

    /// <summary>
    /// Adds NeoKolors logging provider configured to write to a log file path.
    /// </summary>
    public static ILoggingBuilder AddNeoKolorsFile(this ILoggingBuilder builder, string filePath, bool append = true) {
        if (builder == null)
            throw new ArgumentNullException(nameof(builder));

        return builder.AddNeoKolors(options => options.UseFileLogging(filePath, null, append));
    }
}