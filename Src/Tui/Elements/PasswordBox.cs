// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Console.Input;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Global;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A secured stateful editable single-line input control.
/// Displays mask characters (e.g. •) for entered text.
/// </summary>
public class PasswordBox : Control<string>, ISelectableElement<string>, IMouseInteractableElement<string> {
    
    private string _password = string.Empty;
    private int _cursor;
    private char _passwordChar = '•';

    public string Password {
        get => _password;
        set {
            var val = value ?? string.Empty;
            if (_password == val) return;
            _password = val;
            _cursor = Math.Clamp(_cursor, 0, _password.Length);
            InvokeElementUpdated();
        }
    }

    public char PasswordChar {
        get => _passwordChar;
        set {
            if (_passwordChar == value) return;
            _passwordChar = value;
            InvokeElementUpdated();
        }
    }

    public string Placeholder { get; set; } = string.Empty;

    public bool IsSelected { get; private set; }

    public bool IsSelectable => true;

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Width = Dimension.Chars(20),
        Height = Dimension.Chars(1),
        BackgroundColor = NKColor.Default,
        ReadOnly = true
    };

    public PasswordBox() : base(DefaultStyles) {
        OnClick += HandleClick;
    }

    protected override Size MeasureOverride(Size availableSize) {
        var length = Math.Max(_password.Length, Placeholder.Length);
        return new Size(length, 1);
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;

        // Clear NEGATIVE style from the content region first
        for (int x = 0; x < RenderLayout.Content.Width; x++) {
            var cp = pos + RenderLayout.Content.Lower + new Point(x, 0);
            var relativeCp = cp - pos;
            if (RenderLayout.Content.Contains(relativeCp.X, relativeCp.Y)) {
                var cell = canvas[cp.X, cp.Y];
                cell.Style = cell.Style with { Styles = cell.Style.Styles & ~NeoKolors.Common.TextStyles.NEGATIVE };
            }
        }

        string renderedText = (_password.Length == 0 && !IsSelected) ? Placeholder : new string(PasswordChar, _password.Length);
        canvas.Place(renderedText, pos + RenderLayout.Content.Lower, RenderLayout.Content.Width, HorizontalAlign.LEFT);

        if (IsSelected) {
            var cursorPoint = pos + RenderLayout.Content.Lower + new Point(_cursor, 0);
            var relativeCursor = cursorPoint - pos;
            if (RenderLayout.Content.Contains(relativeCursor.X, relativeCursor.Y)) {
                var cell = canvas[cursorPoint.X, cursorPoint.Y];
                if (cell.Char == null || cell.Char == '\0') {
                    cell.Char = ' ';
                }
                cell.Style = cell.Style with { Styles = cell.Style.Styles | NeoKolors.Common.TextStyles.NEGATIVE };
            }
        }
    }

    public override ElementInfo Info { get; } = new();

    public void Select() {
        if (IsSelected) return;
        IsSelected = true;
        IsFocused = true;
        AppEventBus.KeyEvent += HandleKey;
        InvokeElementUpdated();

        if (ElementManager.CurrentlySelected != this) {
            ElementManager.CurrentlySelected = this;
        }
    }

    public void Deselect() {
        if (!IsSelected) return;
        IsSelected = false;
        IsFocused = false;
        AppEventBus.KeyEvent -= HandleKey;
        InvokeElementUpdated();

        if (ElementManager.CurrentlySelected == this) {
            ElementManager.CurrentlySelected = null;
        }
    }

    private void HandleKey(KeyEventArgs keyInfo) {
        switch (keyInfo.Key) {
            case KeyCode.ARROW_LEFT:
                if (_cursor > 0) _cursor--;
                break;
            case KeyCode.ARROW_RIGHT:
                if (_cursor < _password.Length) _cursor++;
                break;
            case KeyCode.HOME:
                _cursor = 0;
                break;
            case KeyCode.END:
                _cursor = _password.Length;
                break;
            case KeyCode.BACKSPACE:
                if (_cursor > 0 && _password.Length > 0) {
                    _password = _password.Remove(_cursor - 1, 1);
                    _cursor--;
                }
                break;
            case KeyCode.DELETE:
                if (_cursor < _password.Length && _password.Length > 0) {
                    _password = _password.Remove(_cursor, 1);
                }
                break;
            case KeyCode.SPACE:
                AddChar(' ');
                break;
            default:
                if (!char.IsControl(keyInfo.Char)) {
                    AddChar(keyInfo.Char);
                }
                break;
        }
        InvokeElementUpdated();
    }

    private void AddChar(char c) {
        _password = _password.Insert(_cursor, c.ToString());
        _cursor++;
    }

    public override string GetChildNode() => _password;

    public override void SetChildNode(string childNode) {
        Password = childNode;
    }

    public event Action<MouseButton> OnClick = delegate { };
    public event Action<MouseButton> OnRelease = delegate { };
    public event Action OnHover = delegate { };
    public event Action OnHoverOut = delegate { };

    public void Click(MouseButton button) => OnClick(button);
    public void Release(MouseButton button) => OnRelease(button);
    public void Hover() => OnHover();
    public void HoverOut() => OnHoverOut();

    private void HandleClick(MouseButton button) {
        if (!IsEnabled) return;
        Select();
    }
}
