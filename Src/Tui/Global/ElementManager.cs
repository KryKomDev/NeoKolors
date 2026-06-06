using NeoKolors.Tui.Elements;

namespace NeoKolors.Tui.Global;

public static class ElementManager {
    private static ISelectableElement? _currentlySelected;

    public static ISelectableElement? CurrentlySelected {
        get => _currentlySelected;
        set {
            if (_currentlySelected == value) return;
            var old = _currentlySelected;
            _currentlySelected = value;
            old?.Deselect();
            _currentlySelected?.Select();
        }
    }
}