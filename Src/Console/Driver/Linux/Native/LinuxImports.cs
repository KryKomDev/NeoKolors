//
// NeoKolors
// Copyright (c) 2026 KryKom
//

#if NK_ENABLE_NATIVE_IO

#if NET7_0_OR_GREATER
#define NK_LIBIMPORT
#endif

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace NeoKolors.Console;

[SuppressMessage("ReSharper", "IdentifierTypo")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static partial class LinuxImports {
    private const string LibC = "libc";

    #region Constants

    // Standard file descriptors
    internal const int STDIN_FILENO = 0;
    internal const int STDOUT_FILENO = 1;
    internal const int STDERR_FILENO = 2;

    // ioctl requests
    internal const nuint TIOCGWINSZ = 0x5413;

    // tcsetattr actions
    internal const int TCSANOW = 0;
    internal const int TCSADRAIN = 1;
    internal const int TCSAFLUSH = 2;

    // poll events
    internal const short POLLIN = 0x0001;
    internal const short POLLPRI = 0x0002;
    internal const short POLLOUT = 0x0004;
    internal const short POLLERR = 0x0008;
    internal const short POLLHUP = 0x0010;
    internal const short POLLNVAL = 0x0020;

    // c_iflag
    internal const uint IGNBRK = 0x0001;
    internal const uint BRKINT = 0x0002;
    internal const uint IGNPAR = 0x0004;
    internal const uint PARMRK = 0x0008;
    internal const uint INPCK = 0x0010;
    internal const uint ISTRIP = 0x0020;
    internal const uint INLCR = 0x0040;
    internal const uint IGNCR = 0x0080;
    internal const uint ICRNL = 0x0100;
    internal const uint IUCLC = 0x0200;
    internal const uint IXON = 0x0400;
    internal const uint IXANY = 0x0800;
    internal const uint IXOFF = 0x1000;
    internal const uint IMAXBEL = 0x2000;
    internal const uint IUTF8 = 0x4000;

    // c_oflag
    internal const uint OPOST = 0x0001;
    internal const uint OLCUC = 0x0002;
    internal const uint ONLCR = 0x0004;
    internal const uint OCRNL = 0x0008;
    internal const uint ONOCR = 0x0010;
    internal const uint ONLRET = 0x0020;

    // c_cflag
    internal const uint CSIZE = 0x0030;
    internal const uint CS8 = 0x0030;
    internal const uint CSTOPB = 0x0040;
    internal const uint CREAD = 0x0080;
    internal const uint PARENB = 0x0100;

    // c_lflag
    internal const uint ISIG = 0x0001;
    internal const uint ICANON = 0x0002;
    internal const uint ECHO = 0x0008;
    internal const uint ECHOE = 0x0010;
    internal const uint ECHOK = 0x0020;
    internal const uint ECHONL = 0x0040;
    internal const uint NOFLSH = 0x0080;
    internal const uint TOSTOP = 0x0100;
    internal const uint IEXTEN = 0x8000;

    // c_cc control character indices
    internal const int VINTR = 0;
    internal const int VQUIT = 1;
    internal const int VERASE = 2;
    internal const int VKILL = 3;
    internal const int VEOF = 4;
    internal const int VTIME = 5;
    internal const int VMIN = 6;
    internal const int VSWTC = 7;
    internal const int VSTART = 8;
    internal const int VSTOP = 9;
    internal const int VSUSP = 10;
    internal const int VEOL = 11;
    internal const int VREPRINT = 12;
    internal const int VDISCARD = 13;
    internal const int VWERASE = 14;
    internal const int VLNEXT = 15;
    internal const int VEOL2 = 16;
    internal const int NCCS = 32;

    // fcntl commands and flags
    internal const int F_GETFL = 3;
    internal const int F_SETFL = 4;
    internal const int O_NONBLOCK = 0x800;

    #endregion

    #region Structures

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct Termios {
        public uint c_iflag;
        public uint c_oflag;
        public uint c_cflag;
        public uint c_lflag;
        public byte c_line;
        public fixed byte c_cc[32];
        public uint c_ispeed;
        public uint c_ospeed;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct WinSize {
        public ushort ws_row;
        public ushort ws_col;
        public ushort ws_xpixel;
        public ushort ws_ypixel;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PollFd {
        public int fd;
        public short events;
        public short revents;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct PipeFds {
        public int ReadFd;
        public int WriteFd;
    }

    #endregion

    #region P/Invoke Declarations

    #if NK_LIBIMPORT
    [LibraryImport(LibC, SetLastError = true)]
    internal static partial int tcgetattr(int fd, out Termios termios);

    [LibraryImport(LibC, SetLastError = true)]
    internal static partial int tcsetattr(int fd, int optional_actions, in Termios termios);

    [LibraryImport(LibC, SetLastError = true)]
    internal static partial int ioctl(int fd, nuint request, out WinSize ws);

    [LibraryImport(LibC, SetLastError = true)]
    internal static unsafe partial int poll(PollFd* fds, nuint nfds, int timeout);

    [LibraryImport(LibC, SetLastError = true)]
    internal static unsafe partial nint read(int fd, byte* buf, nuint count);

    [LibraryImport(LibC, SetLastError = true)]
    internal static unsafe partial nint write(int fd, byte* buf, nuint count);

    [LibraryImport(LibC, SetLastError = true)]
    internal static partial int isatty(int fd);

    [LibraryImport(LibC, SetLastError = true)]
    internal static partial int pipe(out PipeFds pipefd);

    [LibraryImport(LibC, SetLastError = true)]
    internal static partial int close(int fd);

    [LibraryImport(LibC, SetLastError = true)]
    internal static partial int fcntl(int fd, int cmd, int arg);
    
    #else
    
    [DllImport(LibC, SetLastError = true)]
    internal static extern int tcgetattr(int fd, out Termios termios);

    [DllImport(LibC, SetLastError = true)]
    internal static extern int tcsetattr(int fd, int optional_actions, in Termios termios);

    [DllImport(LibC, SetLastError = true)]
    internal static extern int ioctl(int fd, nuint request, out WinSize ws);

    [DllImport(LibC, SetLastError = true)]
    internal static unsafe extern int poll(PollFd* fds, nuint nfds, int timeout);

    [DllImport(LibC, SetLastError = true)]
    internal static unsafe extern nint read(int fd, byte* buf, nuint count);

    [DllImport(LibC, SetLastError = true)]
    internal static unsafe extern nint write(int fd, byte* buf, nuint count);

    [DllImport(LibC, SetLastError = true)]
    internal static extern int isatty(int fd);

    [DllImport(LibC, SetLastError = true)]
    internal static extern int pipe(out PipeFds pipefd);

    [DllImport(LibC, SetLastError = true)]
    internal static extern int close(int fd);

    [DllImport(LibC, SetLastError = true)]
    internal static extern int fcntl(int fd, int cmd, int arg);
    #endif

    #endregion
}

#endif