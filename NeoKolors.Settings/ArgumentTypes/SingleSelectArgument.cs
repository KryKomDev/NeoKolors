//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Exceptions;

namespace NeoKolors.Settings.ArgumentTypes;

public class SingleSelectArgument : IArgument {
    public string[] Options { get; }
    public int Index { get; private set; }
    public int DefaultIndex { get; }

    public SingleSelectArgument(string[] options, int defaultValue = 0) {
        Options = options;
        DefaultIndex = Math.Min(Math.Max(0, defaultValue), Options.Length - 1);
        Index = DefaultIndex;
    }
    
    public void Set(object value) {
        if (value is string s) {
            for (int i = 0; i < Options.Length; i++) {
                if (Options[i] != s) continue;
                Index = i;
                return;
            }

            throw new InvalidArgumentInputException("Inputted string is not a valid option.");
        }
        else if (value is int i) {
            if (i < 0 || i >= Options.Length)
                throw new InvalidArgumentInputException($"Option index is out of range. Must be between 0 and {Options.Length - 1} inclusive.");
            
            Index = i;
        }
        else if (value is SingleSelectArgument a) {
            Set(a.Index);
        }
        else {
            throw new InvalidArgumentInputTypeException(typeof(string[]), value.GetType());
        }
    }

    public void Set(int index) {
        if (index < 0 || index >= Options.Length) 
            throw new InvalidArgumentInputException($"Option index is out of range. Must be between 0 and {Options.Length - 1} inclusive.");

        Index = index;
    }
    public object Get() => Options[Index];
    public int GetIndex() => Index;
    public void Reset() => Index = DefaultIndex;
    public IArgument Clone() => (IArgument)MemberwiseClone();
    public override string ToString() =>
        $"{{\"type\": \"single-select\", " +
        $"\"value\": \"{Options[Index]}\", " +
        $"\"index\": {Index}, " +
        $"\"default-index\": {DefaultIndex}, " +
        $"\"options\": [{string.Join(", ", Options.Select(o => $"\"{o}\""))}]}}";
}