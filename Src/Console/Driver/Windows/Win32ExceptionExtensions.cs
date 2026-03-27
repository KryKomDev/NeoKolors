//
// NeoKolors
// Copyright (c) 2026 KryKom
//

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace NeoKolors.Console.Driver.Windows;

internal static class Win32ExceptionExtensions {
    extension(Win32Exception) {
        
        [DoesNotReturn]
        public static void Throw(uint errorCode) {
            throw new Win32Exception(unchecked((int)errorCode));    
        }

        [DoesNotReturn]
        public static void ThrowLast() {
            throw new Win32Exception(unchecked((int)WinImports.GetLastError()));
        }
    }
}