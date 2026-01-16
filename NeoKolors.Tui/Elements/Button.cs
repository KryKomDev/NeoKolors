// NeoKolors
// Copyright (c) 2025 KryKom

using NeoKolors.Tui.Styles.Properties;

namespace NeoKolors.Tui.Elements;

public class Button : Text {
    // private static readonly NKLogger LOGGER = NKDebug.GetLogger<Button>();
    // private static bool LOGGED_SRC_ERR = false;
    //
    // private Rectangle _lastRender = Rectangle.Zero;
    //
    // private void InvokeMouseAction(MouseEventArgs a) {
    //     if (!_lastRender.Contains(a.Position.X, a.Position.Y)) 
    //         return;
    //     
    //     if (a is { Move: false, Release: false }) 
    //         OnClick.Invoke(a.Button);
    //     else if (a is {Move: false, Release: true})
    //         OnRelease.Invoke(a.Button);
    // }
    //
    // public event Action<MouseButton> OnClick = _ => { };
    // public event Action<MouseButton> OnRelease = _ => { };
    //
    // public Button(string text) : base(text) {
    //     if (!AppEventBus.IsSourceSet) {
    //         if (LOGGED_SRC_ERR) return;
    //         
    //         LOGGER.Error("Source application not set! Cannot register");
    //         LOGGED_SRC_ERR = true;
    //
    //         return;
    //     }
    //     
    //     AppEventBus.SubscribeToMouseEvent(InvokeMouseAction);
    // }
    //
    // public override void Render(ICharCanvas canvas, Rectangle rect) {
    //     base.Render(canvas, rect);
    //
    //     var pos = new Point(
    //         Position.AbsoluteX ? Position.X.ToScalar(rect.Width) : rect.LowerX + Position.X.ToScalar(rect.Width), 
    //         Position.AbsoluteY ? Position.Y.ToScalar(rect.Width) : rect.LowerY + Position.Y.ToScalar(rect.Height)
    //     );
    //     
    //     _lastRender = ComputeRenderLayout(rect).Border + pos;
    // }
    
    public Button(string text) : base(text) { }

    protected override WidthProperty DefaultWidth => new(Dimension.MinContent);
}