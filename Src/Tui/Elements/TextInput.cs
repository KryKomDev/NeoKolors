// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Console.Input;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

public class TextInput : Text, ISelectableElement<AnsiString> {

    protected new static StyleCollection DefaultStyles { get; } = new(Text.DefaultStyles) {
        Width = Dimension.Chars(20),
        Height = Dimension.Chars(1),
        BackgroundColor = NKColor.Default,
        Overflow = false,
        
        ReadOnly = true
    };
    
    public TextInput(string label, string placeholder = "", string? defaultValue = null) 
        : base(CreateText(label, placeholder, defaultValue), DefaultStyles) 
    {
        Label = label;
        Placeholder = placeholder;
        DefaultValue = defaultValue;
        _value = defaultValue ?? "";
        _cursor = 0;
    }
    
    public override AnsiString Content { 
        get => ComputeRenderedText();
        set {
            _value = value.Plain;
            InvokeElementUpdated();
        }
    }

    private string _value;
    private int _cursor;
    
    public bool IsSelected { get; private set; }
    public bool IsSelectable => true;

    public string Label { get; }
    public string Placeholder { get; }
    public string? DefaultValue { get; }

    public string Value => _value.Length != 0 ? _value : (DefaultValue ?? "");

    private static string CreateText(string label, string placeholder, string? defaultValue) 
        => label + (defaultValue ?? placeholder);

    private void HandleKey(KeyEventArgs keyInfo) {
        switch (keyInfo.Key) {
            case KeyCode.ARROW_LEFT when keyInfo.Modifiers.GetHasCtrl(): {
                JumpCursorLeft();
            } break;
            case KeyCode.ARROW_RIGHT when keyInfo.Modifiers.GetHasCtrl(): {
                JumpCursorRight();
            } break;
            case KeyCode.ARROW_LEFT: {
                MoveCursorLeft();
            } break;
            case KeyCode.ARROW_RIGHT: {
                MoveCursorRight();
            } break;
            case KeyCode.HOME: {
                JumpHome();
            } break;
            case KeyCode.END: {
                JumpEnd();
            } break;
            case KeyCode.BACKSPACE: {
                Delete(false, keyInfo.Modifiers.GetHasCtrl());
            } break;
            case KeyCode.DELETE: {
                Delete(true, keyInfo.Modifiers.GetHasCtrl());
            } break;
            case KeyCode.ENTER: {
                
            } break;
            case KeyCode.SPACE: {
                AddKey(' ');
            } break;
            case var _ when !char.IsControl(keyInfo.Char): {
                AddKey(keyInfo.Char);
            } break;
            default: { } break;
        }

        InvokeElementUpdated();
    }

    private void MoveCursorLeft() {
        if (_cursor > 0) {
            _cursor--;
        }
    }

    private void JumpCursorLeft() {
        if (_cursor > 0 && _value[_cursor - 1] == ' ') {
            _cursor--;
            return;
        }
        
        while (_cursor > 0 && char.IsLetterOrDigit(_value[_cursor - 1])) {
            _cursor--;
        }
    }
    
    private void MoveCursorRight() {
        if (_cursor < _value.Length) {
            _cursor++;
        }
    }
    
    private void JumpCursorRight() {
        if (_cursor < _value.Length && _value[_cursor] == ' ') {
            _cursor++;
            return;
        }
        
        while (_cursor < _value.Length && char.IsLetterOrDigit(_value[_cursor])) {
            _cursor++;
        }
    }

    private void JumpHome() {
        _cursor = 0;
    }

    private void JumpEnd() {
        _cursor = _value.Length;
    }

    private void Delete(bool del, bool jump) {
        if (_value.Length <= 0)
            return;

        if (jump) {
            JumpDelete(del);
            return;
        }

        if (del) {
            if (_cursor <= _value.Length - 1) 
                _value = _value.Remove(_cursor, 1);
        }
        else {
            if (_cursor <= 0) return;
            _value = _value.Remove(_cursor - 1, 1);
            _cursor--;
        }
    }

    private void JumpDelete(bool del) {
        if (del) {
            int n = _cursor;
            
            do {
                n++;
            } 
            while (n < _value.Length && char.IsLetterOrDigit(_value[n]));
            
            _value = _value.Remove(_cursor, n - _cursor);
        }
        else {
            int n = _cursor;

            do {
                n--;
            } 
            while (n > 0 && char.IsLetterOrDigit(_value[n]));
            
            _value = _value.Remove(n, _cursor - n);
            _cursor = n;
        }
    }

    private void AddKey(char c) {
        if (_cursor >= _value.Length) {
            _value += c.ToString();
            _cursor++;
            return;
        }
        
        _value = _value.Insert(_cursor, c.ToString());
        _cursor++;
    }

    private AnsiString ComputeRenderedText() {
        var str = Label + (_value.Length == 0 ? Placeholder : _value) + " ";
        var a = new AnsiString(str, new NKStyle(_style.TextColor, _style.BackgroundColor, _style.TextStyle));

        if (!IsSelected) return a;

        return a.ApplyStyle(
            new NKStyle(NKColor.Inherit, NKColor.Inherit, TextStyles.NEGATIVE),
            (_cursor + Label.Length)..(_cursor + Label.Length + 1)
        );
    }
    
    public void Select() {
        IsSelected = true;
        AppEventBus.KeyEvent += HandleKey;
    }

    public void Deselect() {
        IsSelected = false;
        AppEventBus.KeyEvent -= HandleKey;
    }
}