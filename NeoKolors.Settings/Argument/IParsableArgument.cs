//
// NeoKolors
// Copyright (c) 2025 KryKom
//

namespace NeoKolors.Settings.Argument;

public interface IParsableArgument : IArgument {
    
    /// <summary>
    /// sets the <see cref="IArgument{T}.Value"/> using a string
    /// </summary>
    /// <param name="value"></param>
    public void Set(string value);
}