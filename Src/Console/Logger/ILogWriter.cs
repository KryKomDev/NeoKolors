// NeoKolors
// Copyright (c) krystof 2026

namespace NeoKolors.Console;

public interface ILogWriter : IDisposable {
    public void Write(NKLogRecord record);
}