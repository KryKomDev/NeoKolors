namespace NeoKolors.Tui.Style;

public class FlexGrowProperty : IStyleProperty<FlexGrowProperty.FlexGrowData> {
    public FlexGrowData Value { get; }
    public static string GetStaticName() => "flex-grow";
    public string GetName() => GetStaticName();
    public static FlexGrowData GetStaticDefault() => new(1);
    public FlexGrowData GetDefault() => GetStaticDefault();

    public class FlexGrowData {
        public uint Grow { get; }

        public FlexGrowData(uint grow) {
            Grow = grow;
        }
        
        public static implicit operator FlexGrowData(uint grow) => new(grow);
        public static implicit operator uint(FlexGrowData grow) => grow.Grow;
    }
}