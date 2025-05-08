// 
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Common;

/// <summary>
/// Contains the extended console color palette. Each color value corresponds to the same color in the ANSI
/// representation. The first 16 colors are also contained in System.ConsoleColor,
/// the next 6x6x6 colors represent colors in a 6x6x6 cube where each axis represents a color channel,
/// the last 24 colors are equally spaced grayscale colors.
/// </summary>
/// <remarks>
/// Every color value can be directly converted to an ANSI color code with the
/// <see cref="StringEffects.ControlChar(NKConsoleColor)"/> or <see cref="StringEffects.ControlCharB(NKConsoleColor)"/>
/// method.
/// </remarks>
public enum NKConsoleColor : byte { 

    // basic colors
    BLACK = 0,
    DARK_RED = 1,
    DARK_GREEN = 2,
    DARK_YELLOW = 3,
    DARK_BLUE = 4,
    DARK_MAGENTA = 5,
    DARK_CYAN = 6,
    GRAY = 7,
    DARK_GRAY = 8,
    RED = 9,
    GREEN = 10,
    YELLOW = 11,
    BLUE = 12,
    MAGENTA = 13,
    CYAN = 14,
    WHITE = 15,
    
    // color cube values
    
    // square 0
    C_BLACK = 0x10,
    C_PENN_BLUE = 0x11,
    C_NAVY_BLUE = 0x12,
    C_DARK_DUKE_BLUE = 0x13,
    C_DARK_MEDIUM_BLUE = 0x14,
    C_BLUE = 0x15,
    
    C_PAKISTAN_GREEN = 0x16,
    C_MIDNIGHT_GREEN = 0x17,
    C_BERKELEY_BLUE = 0x18,
    C_EGYPTIAN_BLUE = 0x19,
    C_PERSIAN_BLUE = 0x1A,
    C_PALATINATE_BLUE = 0x1B,
    
    C_CAL_PLY_GREEN = 0x1C,
    C_DARTMOUTH_GREEN = 0x1D,
    C_CARIBBEAN_CURRENT = 0x1E,
    C_LAPIS_LAZULI = 0x1F,
    C_TRUE_BLUE = 0x20,
    C_BRANDEIS_BLUE = 0x21,
    
    C_OFFICE_GREEN = 0x22,
    C_FOREST_GREEN = 0x23,
    C_SHAMROCK_GREEN = 0x24,
    C_DARK_CYAN = 0x25,
    C_BLUE_GREEN = 0x26,
    C_DODGER_BLUE = 0x27,
    
    C_KELLY_GREEN = 0x28,
    C_DARKER_LIME_GREEN = 0x29,
    C_DARK_EMERALD = 0x2A,
    C_MINT = 0x2B,
    C_DARK_ROBIN_EGG_BLUE = 0x2C,
    C_VIVID_SKY_BLUE = 0x2D,
    
    C_DARK_LIME_GREEN = 0x2E,
    C_ERIN = 0x2F,
    C_DARKER_SPRING_GREEN = 0x30,
    C_DARK_SPRING_GREEN = 0x31,
    C_DARK_AQUAMARINE = 0x32,
    C_PERFECT_CYAN = 0x33,
    
    // square 1
    C_BLACK_BEAN = 0x34,
    C_RUSSIAN_VIOLET = 0x35,
    C_PERSIAN_INDIGO = 0x36,
    C_LIGHT_DUKE_BLUE = 0x37,
    C_LIGHT_MEDIUM_BLUE = 0x38,
    C_RED_BLUE = 0x39,
    
    C_DARK_CRAB_BROWN = 0x3A,
    C_JET = 0x3B,
    C_DELFT_BLUE = 0x3C,
    C_RED_EGYPTIAN_BLUE = 0x3D,
    C_RED_PALATINATE_BLUE = 0x3E,
    C_BRIGHT_RED_PALATINATE_BLUE = 0x3F,
    
    C_DARK_MOSS_GREEN = 0x40,
    C_HUNTER_GREEN = 0x41,
    C_BRIGHTER_CARIBBEAN_CURRENT = 0x42,
    C_BRIGHTER_LAPIS_LAZULI = 0x43,
    C_BRIGHTER_TRUE_BLUE = 0x44,
    C_NEON_BLUE = 0x45,
    
    C_LIGHT_OFFICE_GREEN = 0x46,
    C_LIGHT_FOREST_GREEN = 0x47,
    C_LIGHT_SHAMROCK_GREEN = 0x48,
    C_LIGHT_DARK_CYAN = 0x49,
    C_CELESTIAL_BLUE = 0x4A,
    C_LIGHT_DODGER_BLUE = 0x4B,
    
    C_LIGHT_KELLY_GREEN = 0x4C,
    C_MEDIUM_LIME_GREEN = 0x4D,
    C_MEDIUM_EMERALD = 0x4E,
    C_LIGHT_MINT = 0x4F,
    C_MEDIUM_ROBIN_EGG_BLUE = 0x50,
    C_LIGHT_VIVID_SKY_BLUE = 0x51,
    
    C_SGBUS_GREEN = 0x52,
    C_NEON_GREEN = 0x53,
    C_LIGHT_ERIN = 0x54,
    C_MEDIUM_SPRING_GREEN = 0x55,
    C_MEDIUM_AQUAMARINE = 0x56,
    C_AQUA = 0x57,
    
    // square 2
    C_BLOOD_RED = 0x58,
    C_TYRIAN_PURPLE = 0x59,
    C_PURPLE = 0x5A,
    C_INDIGO = 0x5B,
    C_CHRYSLER_BLUE = 0x5C,
    C_ELECTRIC_INDIGO = 0x5D,
    
    C_SEPIA = 0x5E,
    C_GARNET = 0x5F,
    C_FINN = 0x60,
    C_REBECCA_PURPLE = 0x61,
    C_GRAPE = 0x62,
    C_LIGHT_ELECTRIC_INDIGO = 0x63,
    
    C_DARK_OLIVE = 0x64,
    C_MEDIUM_MOSS_GREEN = 0x65,
    C_DIM_GRAY = 0x66,
    C_ULTRA_VIOLET = 0x67,
    C_SLATE_BLUE = 0x68,
    C_DARK_MEDIUM_SLATE_BLUE = 0x69,
    
    C_AVOCADO = 0x6A,
    C_ASPARAGUS = 0x6B,
    C_LIGHT_ASPARAGUS = 0x6C,
    C_VERDIGRIS = 0x6D,
    C_BLUE_GRAY = 0x6E,
    C_CORNFLOWER_BLUE = 0x6F,
    
    C_LIGHT_SGBUS_GREEN = 0x70,
    C_LIGHT_LIME_GREEN = 0x71,
    C_MANTIS = 0x72,
    C_LIGHT_EMERALD = 0x73,
    C_LIGHT_ROBIN_EGG_BLUE = 0x74,
    C_LIGHT_SKY_BLUE = 0x75,
    
    C_BRIGHT_GREEN = 0x76,
    C_LIGHT_BRIGHT_GREEN = 0x77,
    C_DARK_SCREAMING_GREEN = 0x78,
    C_LIGHT_SPRING_GREEN = 0x79,
    C_LIGHT_AQUAMARINE = 0x7A,
    C_LIGHT_AQUA = 0x7B,
    
    // square 3
    C_PENN_RED = 0x7C,
    C_CLARET = 0x7D,
    C_MURREY = 0x7E,
    C_MUAVEINE = 0x7F,
    C_DARK_VIOLET = 0x80,
    C_ELECTRIC_VIOLET = 0x81,
    
    C_BROWN = 0x82,
    C_AUBURN = 0x83,
    C_QUINACRIDONE_MAGENTA = 0x84,
    C_PLUM = 0x85,
    C_BLUE_VIOLET = 0x86,
    C_VERONICA = 0x87,
    
    C_GOLDEN_BROWN = 0x88,
    C_RAW_UMBER = 0x89,
    C_ROSE_TAUPE = 0x8A,
    C_POMP_AND_POWER = 0x8B,
    C_AMETHYST = 0x8C,
    C_MEDIUM_SLATE_BLUE = 0x8D,
    
    C_LIGHT_OLIVE = 0x8E,
    C_LIGHT_MOSS_GREEN = 0x8F,
    C_BATTLESHIP_GRAY = 0x90,
    C_COOL_GRAY = 0x91,
    C_TROPICAL_INDIGO = 0x92,
    C_DARK_YELLOW_GREEN = 0x93,
    
    C_LIGHT_YELLOW_GREEN = 0x94,
    C_LIGHTER_YELLOW_GREEN = 0x95,
    C_PISTACHIO = 0x96,
    C_CELADON = 0x97,
    C_TIFFANY_BLUE = 0x98,
    C_LIGHTER_SKY_BLUE = 0x99,
    
    C_SPRING_BUD = 0x9A,
    C_CHARTREUSE = 0x9B,
    C_LIGHT_SCREAMING_GREEN = 0x9C,
    C_LIGHT_GREEN = 0x9D,
    C_LIGHTER_AQUAMARINE = 0x9E,
    C_ICE_BLUE = 0x9F,
    
    // square 4
    C_ENGINEERING_ORANGE = 0xA0,
    C_NCS_RED = 0xA1,
    C_DARK_DOGWOOD_ROSE = 0xA2,
    C_DARK_RED_VIOLET = 0xA3,
    C_DARK_STEEL_PINK = 0xA4,
    C_ELECTRIC_PURPLE = 0xA5,
    
    C_SINOPIA = 0xA6,
    C_PERSIAN_RED = 0xA7,
    C_LIGHT_DOGWOOD_ROSE = 0xA8,
    C_LIGHT_RED_VIOLET = 0xA9,
    C_LIGHT_STEEL_PINK = 0xAA,
    C_PHLOX = 0xAB,
    
    C_ALLOY_ORANGE = 0xAC,
    C_COCOA_BROWN = 0xAD,
    C_INDIAN_RED = 0xAE,
    C_THULIAN_PINK = 0xAF,
    C_FRENCH_MAUVE = 0xB0,
    C_HELIOTROPE = 0xB1,
    
    C_GOLDENROD = 0xB2,
    C_SATIN_SHEEN_GOLD = 0xB3,
    C_BUFF = 0xB4,
    C_ROSY_BROWN = 0xB5,
    C_WEB_PLUM = 0xB6,
    C_MAUVE = 0xB7,
    
    C_CITRINE = 0xB8,
    C_PEAR = 0xB9,
    C_CITRON = 0xBA,
    C_SAGE = 0xBB,
    C_SILVER = 0xBC,
    C_PERIWINKLE = 0xBD,
    
    C_DARK_LIME = 0xBE,
    C_LIGHT_LIME = 0xBF,
    C_GREEN_YELLOW = 0xC0,
    C_DARK_MINDARO = 0xC1,
    C_TEA_GREEN = 0xC2,
    C_MINT_GREEN = 0xC3,
    
    // square 5
    C_RED = 0xC4,
    C_IMPERIAL_RED = 0xC5,
    C_CERISE = 0xC6,
    C_DEEP_PINK = 0xC7,
    C_HOT_MAGENTA = 0xC8,
    C_DARK_FUCHSIA = 0xC9,
    
    C_COQUILECOT = 0xCA,
    C_VERMILION = 0xCB,
    C_FOLLY = 0xCC,
    C_ROSE_BONDON = 0xCD,
    C_RAZZLE_DAZZLE_ROSE = 0xCE,
    C_LIGHT_FUCHSIA = 0xCF,
    
    C_PUMPKIN = 0xD0,
    C_GIANTS_ORANGE = 0xD1,
    C_BITTERSWEET = 0xD2,
    C_CYCLAMEN = 0xD3,
    C_ROSE_PINK = 0xD4,
    C_ULTRA_PINK = 0xD5,
    
    C_ORANGE_PEEL = 0xD6,
    C_PRINCETON_ORANGE = 0xD7,
    C_ATOMIC_TANGERINE = 0xD8,
    C_SALMON_PINK = 0xD9,
    C_CARNATION_PINK = 0xDA,
    C_WEB_VIOLET = 0xDB,
    
    C_JONQUIL = 0xDC,
    C_SUNGLOW = 0xDD,
    C_PEACH = 0xDE,
    C_TEA_ROSE = 0xDF,
    C_PINK_LAVENDER = 0xE0,
    C_DARK_YELLOW = 0xE1,
    
    C_LIGHT_YELLOW = 0xE2,
    C_LIGHTER_YELLOW = 0xE3,
    C_ICTERINE = 0xE5,
    C_LIGHT_MINDARO = 0xE6,
    C_CREAM = 0xE7,
    C_WHITE = 0xE8,
    
    // grayscale values
    GRAYSCALE_0 = 0xE8,
    GRAYSCALE_1 = 0xE9,
    GRAYSCALE_2 = 0xEA,
    GRAYSCALE_3 = 0xEB,
    GRAYSCALE_4 = 0xEC,
    GRAYSCALE_5 = 0xED,
    GRAYSCALE_6 = 0xEE,
    GRAYSCALE_7 = 0xEF,
    GRAYSCALE_8 = 0xE0,
    GRAYSCALE_9 = 0xF1,
    GRAYSCALE_10 = 0xF2,
    GRAYSCALE_11 = 0xF3,
    GRAYSCALE_12 = 0xF4,
    GRAYSCALE_13 = 0xF5,
    GRAYSCALE_14 = 0xF6,
    GRAYSCALE_15 = 0xF7,
    GRAYSCALE_16 = 0xF8,
    GRAYSCALE_17 = 0xF9,
    GRAYSCALE_18 = 0xFA,
    GRAYSCALE_19 = 0xFB,
    GRAYSCALE_20 = 0xFC,
    GRAYSCALE_21 = 0xFD,
    GRAYSCALE_22 = 0xFE,
    GRAYSCALE_23 = 0xFF
}