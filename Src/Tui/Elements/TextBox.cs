// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Console.Input;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Global;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A stateful editable single-line input control.
/// Replaces the legacy TextInput element.
/// </summary>
public class TextBox : Control<string>, ISelectableElement<string>, IMouseInteractableElement<string> {
    
    private string _text = string.Empty;
    private int _cursor;
    private int _scrollOffset;

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

    public TextBox() : base(DefaultStyles) {
        OnClick += HandleClick;
    }

    public TextBox(string? initialText) : base(DefaultStyles) {
        _text = initialText ?? string.Empty;
        _cursor = _text.Length;
        OnClick += HandleClick;
    }

    protected override Size MeasureOverride(Size availableSize) {
        var length = Math.Max(_text.Length, Placeholder.Length);
        return new Size(length, 1);
    }

    private void KeepCursorInView(int contentWidth) {
        if (contentWidth <= 0) {
            _scrollOffset = 0;
            return;
        }

        if (_cursor < _scrollOffset) {
            _scrollOffset = _cursor;
        }
        else if (_cursor >= _scrollOffset + contentWidth) {
            _scrollOffset = _cursor - contentWidth + 1;
        }

        var maxScroll = Math.Max(0, _text.Length - contentWidth + 1);
        _scrollOffset = Math.Clamp(_scrollOffset, 0, maxScroll);
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;
        var contentWidth = RenderLayout.Content.Width;

        KeepCursorInView(contentWidth);

        // Clear NEGATIVE style from the content region first
        for (int x = 0; x < contentWidth; x++) {
            var cp = pos + RenderLayout.Content.Lower + new Point(x, 0);
            var relativeCp = cp - pos;

            if (!RenderLayout.Content.Contains(relativeCp.X, relativeCp.Y)) continue;
            if (cp.X < 0 || cp.X >= canvas.Width || cp.Y < 0 || cp.Y >= canvas.Height) continue;

            var clearCell = canvas[cp.X, cp.Y];
            clearCell.Style = clearCell.Style with { Styles = clearCell.Style.Styles & ~TextStyles.NEGATIVE };
        }

        string renderedText;
        if (_text.Length == 0 && !IsSelected) {
            renderedText = Placeholder;
        } else {
            int start = Math.Clamp(_scrollOffset, 0, _text.Length);
            int length = Math.Min(contentWidth, _text.Length - start);
            renderedText = length > 0 ? _text.Substring(start, length) : string.Empty;
        }

        canvas.Place(renderedText, pos + RenderLayout.Content.Lower, contentWidth, HorizontalAlign.LEFT);

        if (!IsSelected) return;

        int relativeCursorX = _cursor - _scrollOffset;
        if (relativeCursorX >= 0 && relativeCursorX < contentWidth) {
            var cursorPoint = pos + RenderLayout.Content.Lower + new Point(relativeCursorX, 0);
            var relativeCursor = cursorPoint - pos;

            if (RenderLayout.Content.Contains(relativeCursor.X, relativeCursor.Y)) {
                if (cursorPoint.X >= 0 && cursorPoint.X < canvas.Width && cursorPoint.Y >= 0 && cursorPoint.Y < canvas.Height) {
                    var cursorCell = canvas[cursorPoint.X, cursorPoint.Y];
                    if (cursorCell.Char is null or '\0') {
                        cursorCell.Char = ' ';
                    }
                    cursorCell.Style = cursorCell.Style with { Styles = cursorCell.Style.Styles | TextStyles.NEGATIVE };
                }
            }
        }
    }

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
        if (keyInfo.Up)
            return;
        
        switch (keyInfo.Key) {
            case KeyCode.ARROW_LEFT when keyInfo.Modifiers.GetHasCtrl(): {
                if (_cursor > 0 && _text[_cursor - 1] == ' ') {
                    while (_cursor > 0 && _text[_cursor - 1] == ' ') 
                        _cursor--;
                    
                    break;
                }
                
                while (_cursor > 0 && _text[_cursor - 1] != ' ') 
                    _cursor--;
                
                break;
            }
            case KeyCode.ARROW_LEFT: {
                if (_cursor > 0) _cursor--;
                break;
            }
            case KeyCode.ARROW_RIGHT when keyInfo.Modifiers.GetHasCtrl(): {
                if (_cursor < _text.Length && _text[_cursor + 1] == ' ') {
                    while (_cursor == _text.Length - 1 || (_cursor < _text.Length && _text[_cursor + 1] == ' ')) 
                        _cursor++;
                    
                    break;
                }
                
                while (_cursor == _text.Length - 1 || (_cursor < _text.Length && _text[_cursor + 1] != ' ')) 
                    _cursor++;

                break;
            }
            case KeyCode.ARROW_RIGHT: {
                if (_cursor < _text.Length) _cursor++;
                break;
            }
            case KeyCode.HOME: {
                _cursor = 0;
                break;
            }
            case KeyCode.END: {
                _cursor = _text.Length;
                break;
            }
            case KeyCode.BACKSPACE when keyInfo.Modifiers.GetHasCtrl(): {
                NKDebug.Debug("Backspace!");

                var i = _cursor;
                
                if (i > 0 && _text[i - 1] == ' ') {
                    while (i > 0 && _text[i - 1] == ' ') 
                        i--;
                }
                else {
                    while (i > 0 && _text[i - 1] != ' ') 
                        i--;
                }
                
                _text = _text.Remove(i, _cursor - i);
                _cursor = i;
                
                break;
            }
            case KeyCode.BACKSPACE: {
                if (_cursor > 0 && _text.Length > 0) {
                    _text = _text.Remove(_cursor - 1, 1);
                    _cursor--;
                }
                
                break;
            }
            case KeyCode.DELETE: {
                if (_cursor < _text.Length && _text.Length > 0) {
                    _text = _text.Remove(_cursor, 1);
                }
                break;
            }
            case KeyCode.SPACE: {
                AddChar(' ');
                break;
            }
            default: {
                if (!char.IsControl(keyInfo.Char)) {
                    AddChar(keyInfo.Char);
                }
                break;
            }
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
