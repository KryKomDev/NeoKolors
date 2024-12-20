using System.Xml.Serialization;

namespace NeoKolors.ConsoleGraphics.TUI.BlockSystem;

public class GuiBuilder {

    public const int MAX_NESTING = 4;

    public GuiRow[] Rows { get; internal init; } = [];
    
    internal GuiBuilder(string xml) {
        XmlSerializer serializer = new XmlSerializer(typeof(GuiBuilder));
        using StringReader reader = new StringReader(xml);
        
        Rows = ((GuiBuilder)serializer.Deserialize(reader)!).Rows;
    }

    private GuiBuilder() {}

    public static GuiBuilder Build(params GuiRow[] rows) {
        return new GuiBuilder {
            Rows = rows
        };
    }
    
    public static float[] Normalize(float[] sizes) {
        float sum = sizes.Sum();

        if (!(Math.Abs(sum - 100) > 0.01)) return sizes;
        
        float factor = 100f / sum;

        for (int i = 0; i < sizes.Length; i++) {
            sizes[i] *= factor;
        }

        return sizes;
    }
}