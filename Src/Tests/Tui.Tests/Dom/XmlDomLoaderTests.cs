using NeoKolors.Tui.Dom;
using NeoKolors.Tui.Elements;
using NeoKolors.Common;
using NeoKolors.Tui.Styles;
using Xunit.Sdk;

namespace NeoKolors.Tui.Tests.Dom;

public class XmlDomLoaderTests {
    
    [Fact]
    public void Load_SimpleStructure_CreatesDom() {
        const string xml = 
            """
            <Div Width='100%' Height='50px' BackgroundColor='Red'>
                <Text TextColor='#00FF00'>Hello World</Text>
                <Button Id="some-id" Class="some-class">Click Me</Button>
                <Div Width='50%'>
                    <Text>Inner</Text>
                </Div>
            </Div>
            """;
    
        var loader = new XmlDomLoader();
        var dom = loader.Load(xml);
        
        Assert.NotNull(dom.BaseElement);
        Assert.IsType<Div>(dom.BaseElement);
        
        var root = (Div)dom.BaseElement;
        Assert.Equal(Dimension.Percent(100), root.Style.GetWidth());
        Assert.Equal(Dimension.Pixels(50), root.Style.GetHeight());
        Assert.Equal(new NKColor(ConsoleColor.Red), root.Style.GetBackgroundColor());
        
        var children = root.GetChildNode();
        if (children is null)
            throw FailException.ForFailure("Expected children to be of type IElement[]");
        
        Assert.Equal(3, children.Length);
        
        Assert.IsType<Text>(children[0]);
        Assert.Equal("Hello World", ((Text)children[0]).Content.String);
        Assert.Equal(NKColor.FromRgb(0, 255, 0), ((Text)children[0]).Style.GetTextColor());
        
        Assert.IsType<Button>(children[1]);
        var b = (Button)children[1];
        Assert.Equal("Click Me", b.Content);
        Assert.Equal("some-id", b.Info.Id);
        Assert.Equal(["some-class"], b.Info.Classes);
        
        Assert.IsType<Div>(children[2]);
        var innerDiv = (Div)children[2];
        Assert.Equal(Dimension.Percent(50), innerDiv.Style.GetWidth());
        
        if (innerDiv.GetChildNode() is not { } innerChildren)
            throw FailException.ForFailure("Expected innerChildren to be of type IElement[]");

        Assert.Single(innerChildren);
        Assert.Equal("Inner", ((Text)innerChildren[0]).Content);
    }

    [Fact]
    public void Load_AttributesCaseInsensitive() {
         const string xml = "<Div Width='100px' backgroundColor='Blue' />";
         
         var loader = new XmlDomLoader();
         var dom = loader.Load(xml);
         
         var div = (Div)dom.BaseElement;
         Assert.Equal(Dimension.Pixels(100), div.Style.GetWidth());
         Assert.Equal(new NKColor(ConsoleColor.Blue), div.Style.GetBackgroundColor());
    }
}
