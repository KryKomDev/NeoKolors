// NeoKolors
// Copyright (c) 2026 KryKom

namespace NeoKolors.Tui.Elements;

/// <summary>
/// Defines the visual frame sequence and default animation interval for a terminal spinner.
/// </summary>
public record SpinnerStyle(IReadOnlyList<string> Frames, TimeSpan Interval) {
    /// <summary>
    /// Classic braille dots spinner.
    /// </summary>
    public static SpinnerStyle Dots { get; } = new(
        ["⠋", "⠙", "⠹", "⠸", "⠼", "⠴", "⠦", "⠧", "⠇", "⠏"],
        TimeSpan.FromMilliseconds(80)
    );

    /// <summary>
    /// Rotating line / slash spinner.
    /// </summary>
    public static SpinnerStyle Line { get; } = new(
        ["-", "\\", "|", "/"],
        TimeSpan.FromMilliseconds(100)
    );

    /// <summary>
    /// Rotating circle quarter quadrants.
    /// </summary>
    public static SpinnerStyle Circle { get; } = new(
        ["◐", "◓", "◑", "◒"],
        TimeSpan.FromMilliseconds(120)
    );

    /// <summary>
    /// Smooth pulsating vertical bar.
    /// </summary>
    public static SpinnerStyle Pulse { get; } = new(
        ["█", "▉", "▊", "▋", "▌", "▍", "▎", "▏", "▎", "▍", "▌", "▋", "▊", "▉"],
        TimeSpan.FromMilliseconds(70)
    );

    /// <summary>
    /// Bouncing bar inside brackets.
    /// </summary>
    public static SpinnerStyle BouncingBar { get; } = new(
        ["[    ]", "[=   ]", "[==  ]", "[=== ]", "[====]", "[ ===]", "[  ==]", "[   =]", "[    ]"],
        TimeSpan.FromMilliseconds(80)
    );

    /// <summary>
    /// Rotating arc quadrants.
    /// </summary>
    public static SpinnerStyle Arc { get; } = new(
        ["◜", "◠", "◝", "◞", "◡", "◟"],
        TimeSpan.FromMilliseconds(100)
    );

    /// <summary>
    /// Eight-stage braille pattern spinner.
    /// </summary>
    public static SpinnerStyle Braille { get; } = new(
        ["⣾", "⣽", "⣻", "⢿", "⡿", "⣟", "⣯", "⣷"],
        TimeSpan.FromMilliseconds(80)
    );

    /// <summary>
    /// Standard 7-bit ASCII spinner.
    /// </summary>
    public static SpinnerStyle Ascii { get; } = new(
        ["|", "/", "-", "\\"],
        TimeSpan.FromMilliseconds(100)
    );

    /// <summary>
    /// Clock face emoji spinner.
    /// </summary>
    public static SpinnerStyle Clock { get; } = new(
        ["🕐", "🕑", "🕒", "🕓", "🕔", "🕕", "🕖", "🕗", "🕘", "🕙", "🕚", "🕛"],
        TimeSpan.FromMilliseconds(100)
    );

    /// <summary>
    /// Rotating compass arrows spinner.
    /// </summary>
    public static SpinnerStyle Arrow { get; } = new(
        ["←", "↖", "↑", "↗", "→", "↘", "↓", "↙"],
        TimeSpan.FromMilliseconds(100)
    );
}