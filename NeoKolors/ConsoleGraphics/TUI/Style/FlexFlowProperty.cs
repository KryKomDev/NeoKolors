namespace NeoKolors.ConsoleGraphics.TUI.Style;

public class FlexFlowProperty : IStyleProperty<FlexFlowProperty.FlexFlowData> {

    public FlexFlowData Value { get; }
    public static string GetStaticName() => "flex-flow";
    public string GetName() => GetStaticName();
    public static FlexFlowData GetStaticDefault() => new();
    public FlexFlowData GetDefault() => GetStaticDefault();

    public FlexFlowProperty(FlexFlowData value) => Value = value;
    public FlexFlowProperty(FlexDirection flexDirection = FlexDirection.ROW, bool wrap = true) 
        => Value = new FlexFlowData(flexDirection, wrap);

    public class FlexFlowData {
        public FlexDirection Direction { get; }
        public bool Wrap { get; }

        public FlexFlowData(FlexDirection direction = FlexDirection.ROW, bool wrap = true) {
            Direction = direction;
            Wrap = wrap;
        }
    }

    public enum FlexDirection {
        ROW,
        ROW_REVERSE,
        COLUMN,
        COLUMN_REVERSE,
    }
}