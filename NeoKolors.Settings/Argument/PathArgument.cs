//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Exception;

namespace NeoKolors.Settings.Argument;

/// <summary>
/// argument for a path to a file or directory
/// </summary>
public class PathArgument : IArgument<string> {
    public string Value { get; private set; }
    public string DefaultValue { get; }
    public bool MustExist { get; }
    public bool PointsToFile { get; }
    public bool PointsToDirectory => !PointsToFile;
    public bool AllowAny { get; }
    
    public PathArgument(string defaultValue = ".", bool mustExist = true, bool allowAny = true, bool pointsToFile = true) {
        DefaultValue = defaultValue;
        Value = DefaultValue;
        MustExist = mustExist;
        AllowAny = allowAny;
        PointsToFile = pointsToFile;
    }

    void IArgument.Set(object value) => Set(value);
    public string Get() => Value;
    public void Reset() => Value = DefaultValue;

    public IArgument<string> Clone() => (IArgument<string>)MemberwiseClone();

    public void Set(object value) {
        if (value is string s) {
            Set(s);
        }
        else if (value is PathArgument p) {
            Set(p.Value);
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(string), value.GetType());
        }
    }

    public void Set(string path) {
        if (MustExist && !Directory.Exists(path) && !File.Exists(path)) 
            throw new InvalidArgumentInputException("Path does not exist.");

        if (!AllowAny) {
            bool isFile = Path.HasExtension(path);
            if (PointsToFile && !isFile) throw new InvalidArgumentInputException("Path must point to a file.");
            if (PointsToDirectory && isFile) throw new InvalidArgumentInputException("Path must point to a directory.");
        }
            
        Value = path;
    }

    object IArgument.Get() => Get();
    void IArgument.Reset() => Reset();
    IArgument IArgument.Clone() => Clone();
    public bool Equals(IArgument? other) {
        return other is PathArgument p &&
               Value == p.Value &&
               DefaultValue == p.DefaultValue &&
               AllowAny == p.AllowAny &&
               MustExist == p.MustExist &&
               PointsToDirectory == p.PointsToDirectory &&
               PointsToFile == p.PointsToFile;
    }
}