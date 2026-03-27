//
// NeoKolors
// Copyright (c) 2026 KryKom
//

namespace NeoKolors.Console.Ansi;

/// <summary>
/// Specifies the different types of queries supported by the Virtual Terminal (VT) within
/// the <see cref="VTQueryType"/> enumeration.
/// </summary>
/// <remarks>
/// Each value in this enumeration represents a distinct query type that can be used to interact
/// with or retrieve information from the VT or Terminal environment. These query types
/// facilitate functionalities such as terminal state inspection, modifying appearance settings,
/// and fetching terminal capabilities.
/// </remarks>
public enum VTQueryType : byte {
    
    /// <summary>
    /// Represents an invalid or unrecognized query type within the <see cref="VTQueryType"/> enum.
    /// </summary>
    /// <remarks>
    /// This value is used to indicate that the provided query type is either undefined
    /// or does not correspond to a valid query type supported by the system.
    /// It often acts as a sentinel or fallback for error handling scenarios.
    /// </remarks>
    INVALID = 0,

    /// <summary>
    /// Represents a DEC (Device Control) query type within the <see cref="VTQueryType"/> enum.
    /// </summary>
    /// <remarks>
    /// This value signifies a query related to DEC (Digital Equipment Corporation) control sequences,
    /// which are used to manage device-specific behavior in terminal emulations. It typically targets
    /// operations involving device attributes and functionalities supported by DEC-compatible systems.
    /// </remarks>
    DEC,

    /// <summary>
    /// Represents an OSC (Operating System Command) query type within the <see cref="VTQueryType"/> enum.
    /// </summary>
    /// <remarks>
    /// This value is used to indicate queries related to Operating System Command (OSC) sequences.
    /// OSC sequences are control sequences in terminal emulators that modify or query aspects of
    /// the terminal environment, such as window titles or clipboard content.
    /// </remarks>
    OSC,

    /// <summary>
    /// Represents a window manipulation query type within the <see cref="VTQueryType"/> enum.
    /// </summary>
    /// <remarks>
    /// This value is associated with queries or commands related to window management in terminal environments.
    /// It facilitates operations that involve adjusting or retrieving window properties such as size, position,
    /// or state.
    /// </remarks>
    WIN,

    /// <summary>
    /// Represents the query type for getting the current window state in the <see cref="VTQueryType"/> enum.
    /// </summary>
    /// <remarks>
    /// This value is specific to queries related to the state of the terminal window, such as its visibility,
    /// maximized state, or other attributes that define its current operational status. It is often used
    /// in contexts where window management information is relevant.
    /// </remarks>
    WIN_STATE,

    /// <summary>
    /// Represents a query type used to retrieve or interact with the title of the window
    /// within the <see cref="VTQueryType"/> enum.
    /// </summary>
    /// <remarks>
    /// This value is specifically associated with operations or queries
    /// that involve getting or manipulating the text displayed in the window's title bar.
    /// It is primarily relevant in environments where window title customization is supported.
    /// </remarks>
    WIN_TITLE,

    /// <summary>
    /// Specifies a query type within the <see cref="VTQueryType"/> enum related to the title of an application icon.
    /// </summary>
    /// <remarks>
    /// This value is used to request or interpret operations involving the icon title metadata
    /// associated with a console or terminal application. It plays a role in enhancing the visual
    /// or contextual representation of the application window.
    /// </remarks>
    ICON_TITLE,

    /// <summary>
    /// Represents a query type for requesting the primary device attributes (DA) within the
    /// <see cref="VTQueryType"/> enum.
    /// </summary>
    /// <remarks>
    /// This value is used to initiate a query to retrieve information about the primary device attributes.
    /// It is commonly used to get details regarding the terminal's capabilities or identification.
    /// </remarks>
    PRIMARY_DA,

    /// <summary>
    /// Represents a secondary device attributes (DA) query type within the <see cref="VTQueryType"/> enum.
    /// </summary>
    /// <remarks>
    /// This value is used to identify queries related to the secondary device attributes
    /// of a terminal or virtual terminal. It provides additional information about the
    /// terminal's characteristics, often supplementing the primary device attributes.
    /// </remarks>
    SECONDARY_DA,
}