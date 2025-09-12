// NeoKolors
// Copyright (c) 2025 KryKom

namespace NeoKolors.Console.Events;

public readonly struct DecReqResponseArgs {
    
    public int Mode { get; }
    public DecReqResponseType Response { get; }

    public DecReqResponseArgs(int mode, DecReqResponseType response) {
        Mode = mode;
        Response = response;
    }
}