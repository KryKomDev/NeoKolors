// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Console.Input;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Styles;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A stateful editable single-line input control.
/// Replaces the legacy TextInput element.
/// </summary>
public class TextBox : Control<string>, ISelectableElement<string> {
    
    private string _text = string.Empty;
    private int _cursor;

    public string Text {
        get => _text;
        set {
            var textVal = value ?? string.Empty;
            if (_text == textVal) return;
            _text = textVal;
            _cursor = Math.Clamp(_cursor, 0, _text.Length);
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

    public TextBox() : base(DefaultStyles) { }

    public TextBox(string initialText) : base(DefaultStyles) {
        _text = initialText ?? string.Empty;
        _cursor = _text.Length;
    }

    protected override Size MeasureOverride(Size availableSize) {
        var length = Math.Max(_text.Length, Placeholder.Length);
        return new Size(length, 1);
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;

        var renderedText = _text.Length == 0 ? Placeholder : _text;
        canvas.Place(renderedText, pos + RenderLayout.Content.Lower, RenderLayout.Content.Width, HorizontalAlign.LEFT);

        if (IsSelected) {
            var cursorPoint = pos + RenderLayout.Content.Lower + new Point(_cursor, 0);
            var relativeCursor = cursorPoint - pos;
            if (RenderLayout.Content.Contains(relativeCursor.X, relativeCursor.Y)) {
                canvas.StyleBackground(new Rectangle(cursorPoint, Size.One), _style.TextColor);
            }
        }
    }

    public void Select() {
        if (IsSelected) return;
        IsSelected = true;
        IsFocused = true;
        AppEventBus.KeyEvent += HandleKey;
        InvokeElementUpdated();
    }

    public void Deselect() {
        if (!IsSelected) return;
        IsSelected = false;
        IsFocused = false;
        AppEventBus.KeyEvent -= HandleKey;
        InvokeElementUpdated();
    }

    private void HandleKey(KeyEventArgs keyInfo) {
        switch (keyInfo.Key) {
            case KeyCode.ARROW_LEFT:
                if (_cursor > 0) _cursor--;
                break;
            case KeyCode.ARROW_RIGHT:
                if (_cursor < _text.Length) _cursor++;
                break;
            case KeyCode.HOME:
                _cursor = 0;
                break;
            case KeyCode.END:
                _cursor = _text.Length;
                break;
            case KeyCode.BACKSPACE:
                if (_cursor > 0 && _text.Length > 0) {
                    _text = _text.Remove(_cursor - 1, 1);
                    _cursor--;
                }
                break;
            case KeyCode.DELETE:
                if (_cursor < _text.Length && _text.Length > 0) {
                    _text = _text.Remove(_cursor, 1);
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
        _text = _text.Insert(_cursor, c.ToString());
        _cursor++;
    }

    public override ElementInfo Info { get; } = new();

    public override string GetChildNode() => _text;

    public override void SetChildNode(string childNode) {
        Text = childNode;
    }
}
