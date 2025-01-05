namespace NeoKolors.ConsoleGraphics.TUI.Style;

public class StyleBlock {
    public string Selector { get; }
    public SelectorType Type { get; }
    public IStyleProperty<object>[] Properties { get; private set; }

    public StyleBlock(string selector, params IStyleProperty<object>[] properties) {
        if (selector.Length > 0 && selector[0] == '.') {
            selector = selector.Substring(1);
            Selector = selector;
            Type = SelectorType.CLASS;
        }
        else if (selector.Length > 0 && selector[0] == '#') {
            selector = selector.Substring(1);
            Selector = selector;
            Type = SelectorType.ID;
        }
        else if (selector == "*") {
            Selector = "";
            Type = SelectorType.EVERYTHING;
        }
        else {
            Selector = selector;
            Type = SelectorType.TAG;
        }
        
        Properties = properties;
    }
    
    public enum SelectorType {
        CLASS,
        ID,
        TAG,
        EVERYTHING
    }

    public object GetProperty<T>() where T : IStyleProperty<object> {
        foreach (var p in Properties) {
            if (typeof(T) == p.GetType()) {
                return p.Value;
            }
        }
        
        return T.GetStaticDefault();
    }

    public void Merge(StyleBlock block) {
        foreach (var outer in block.Properties) {
            bool found = false;
            
            foreach (var inner in Properties) {
                if (outer.GetName() == inner.GetName()) {
                    found = true;
                    break;
                }
            }
            
            if (found) continue;

            IStyleProperty<object>[] props = new IStyleProperty<object>[Properties.Length + 1];

            for (int i = 0; i < Properties.Length; i++) {
                props[i] = Properties[i];
            }
            
            props[Properties.Length] = outer;
            
            Properties = props;
        }
    }
}