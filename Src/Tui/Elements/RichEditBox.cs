// NeoKolors
// Copyright (c) 2026 KryKom

using NeoKolors.Console.Input;
using NeoKolors.Tui.Core;
using NeoKolors.Tui.Events;
using NeoKolors.Tui.Styles;
using NeoKolors.Tui.Global;

namespace NeoKolors.Tui.Elements;

/// <summary>
/// A comprehensive stateful interactive multi-line text editing control.
/// </summary>
public class RichEditBox : Control<string>, ISelectableElement<string>, IMouseInteractableElement<string> {
    
    private readonly List<string> _lines = [string.Empty];
    private int _cursorX;
    private int _cursorY;

    public string Text {
        get => string.Join("\n", _lines);
        set {
            _lines.Clear();
            var val = value ?? string.Empty;
            var parts = val.Replace("\r", "").Split('\n');
            foreach (var part in parts) {
                _lines.Add(part);
            }
            if (_lines.Count == 0) {
                _lines.Add(string.Empty);
            }
            _cursorY = Math.Clamp(_cursorY, 0, _lines.Count - 1);
            _cursorX = Math.Clamp(_cursorX, 0, _lines[_cursorY].Length);
            InvokeElementUpdated();
        }
    }

    public string Placeholder { get; set; } = string.Empty;

    public bool IsSelected { get; private set; }

    public bool IsSelectable => true;

    public static StyleCollection DefaultStyles { get; } = new(AbstractElement.DefaultStyle) {
        Width = Dimension.Chars(40),
        Height = Dimension.Chars(6),
        Border = BorderStyle.GetNormal(),
        BackgroundColor = NKColor.Default,
        ReadOnly = true
    };

    public RichEditBox() : base(DefaultStyles) {
        OnClick += HandleClick;
    }

    protected override Size MeasureOverride(Size availableSize) {
        int maxLen = Placeholder.Length;
        foreach (var line in _lines) {
            maxLen = Math.Max(maxLen, line.Length);
        }
        return new Size(Math.Max(maxLen, 10), Math.Max(_lines.Count, 3));
    }

    protected override void RenderCore(ICharCanvas canvas) {
        var pos = RenderBounds.Lower;
        var contentPos = pos + RenderLayout.Content.Lower;
        var contentWidth = RenderLayout.Content.Width;
        var contentHeight = RenderLayout.Content.Height;

        // Clear NEGATIVE style from the entire content region first
        for (int y = 0; y < contentHeight; y++) {
            for (int x = 0; x < contentWidth; x++) {
                var cp = contentPos + new Point(x, y);
                var relativeCp = cp - pos;
                if (RenderLayout.Content.Contains(relativeCp.X, relativeCp.Y)) {
                    var cell = canvas[cp.X, cp.Y];
                    cell.Style = cell.Style with { Styles = cell.Style.Styles & ~NeoKolors.Common.TextStyles.NEGATIVE };
                }
            }
        }

        if (_lines.Count == 1 && _lines[0].Length == 0 && !string.IsNullOrEmpty(Placeholder) && !IsSelected) {
            canvas.Place(Placeholder, contentPos, contentWidth, HorizontalAlign.LEFT);
        }
        else {
            for (int y = 0; y < _lines.Count && y < contentHeight; y++) {
                canvas.Place(_lines[y], contentPos + new Point(0, y), contentWidth, HorizontalAlign.LEFT);
            }
        }

        if (IsSelected) {
            var cursorPoint = contentPos + new Point(_cursorX, _cursorY);
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
                if (_cursorX > 0) {
                    _cursorX--;
                }
                else if (_cursorY > 0) {
                    _cursorY--;
                    _cursorX = _lines[_cursorY].Length;
                }
                break;
            case KeyCode.ARROW_RIGHT:
                if (_cursorX < _lines[_cursorY].Length) {
                    _cursorX++;
                }
                else if (_cursorY < _lines.Count - 1) {
                    _cursorY++;
                    _cursorX = 0;
                }
                break;
            case KeyCode.ARROW_UP:
                if (_cursorY > 0) {
                    _cursorY--;
                    _cursorX = Math.Min(_cursorX, _lines[_cursorY].Length);
                }
                break;
            case KeyCode.ARROW_DOWN:
                if (_cursorY < _lines.Count - 1) {
                    _cursorY++;
                    _cursorX = Math.Min(_cursorX, _lines[_cursorY].Length);
                }
                break;
            case KeyCode.HOME:
                _cursorX = 0;
                break;
            case KeyCode.END:
                _cursorX = _lines[_cursorY].Length;
                break;
            case KeyCode.ENTER:
                var currentLineContent = _lines[_cursorY];
                var leftPart = currentLineContent.Substring(0, _cursorX);
                var rightPart = currentLineContent.Substring(_cursorX);
                _lines[_cursorY] = leftPart;
                _lines.Insert(_cursorY + 1, rightPart);
                _cursorY++;
                _cursorX = 0;
                break;
            case KeyCode.BACKSPACE:
                if (_cursorX > 0) {
                    _lines[_cursorY] = _lines[_cursorY].Remove(_cursorX - 1, 1);
                    _cursorX--;
                }
                else if (_cursorY > 0) {
                    var prevLine = _lines[_cursorY - 1];
                    _cursorX = prevLine.Length;
                    _lines[_cursorY - 1] = prevLine + _lines[_cursorY];
                    _lines.RemoveAt(_cursorY);
                    _cursorY--;
                }
                break;
            case KeyCode.DELETE:
                if (_cursorX < _lines[_cursorY].Length) {
                    _lines[_cursorY] = _lines[_cursorY].Remove(_cursorX, 1);
                }
                else if (_cursorY < _lines.Count - 1) {
                    _lines[_cursorY] = _lines[_cursorY] + _lines[_cursorY + 1];
                    _lines.RemoveAt(_cursorY + 1);
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
        _lines[_cursorY] = _lines[_cursorY].Insert(_cursorX, c.ToString());
        _cursorX++;
    }

    public override ElementInfo Info { get; } = new();

    public override string GetChildNode() => Text;

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
