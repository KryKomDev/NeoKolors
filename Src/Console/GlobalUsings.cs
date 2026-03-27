// NeoKolors
// Copyright (c) 2025 KryKom

global using Stdio = System.Console;

#if NET9_0_OR_GREATER
global using LockObject = System.Threading.Lock;
#else
global using LockObject = object;
#endif