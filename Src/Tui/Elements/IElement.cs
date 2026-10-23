using Metriks;
using NeoKolors.Tui.Core;

namespace NeoKolors.Tui.Elements;

public interface IElement {
     public void Render(RenderContext context, Size2D parent);
     public Size2D Measure(LayoutContext context);
}