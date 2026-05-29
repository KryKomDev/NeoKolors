//
// NeoKolors
// Copyright (c) 2025 KryKom
//

using NeoKolors.Settings.Argument.Exception;
using static NeoKolors.Settings.Argument.AllowedPathType;

namespace NeoKolors.Settings.Tests;

public class PathArgumentTests {

    [Theory]
    [InlineData("/path/")]  
    [InlineData("C:/path/")]
    [InlineData(@"\path\")]
    [InlineData(@"C:\path\")]
    public void Set_PointsToFile_Throws(string path) {
        var p = new PathArgument(allowedPathType: FILE | NOT_EXISTING);
        Assert.Throws<InvalidArgumentInputException>(() => p.Set(path));
    }

    [Theory]
    [InlineData("/path/a.a")]
    [InlineData("C:/path/a.a")]
    [InlineData(@"\path\a.a")]
    [InlineData(@"C:\path\a.a")]
    public void Set_PointsToFile_WorksProperly(string path) {
        var p = new PathArgument(allowedPathType: FILE | NOT_EXISTING);
        p.Set(path);
        Assert.Equal(p.Get(), path);
    }

    [Theory]
    [InlineData("/path/a.a")]
    [InlineData("C:/path/a.a")]
    [InlineData(@"\path\a.b")]
    [InlineData(@"C:\path\a.a")]
    public void Set_PointsToDirectory_Throws(string path) {
        var p = new PathArgument(allowedPathType: DIRECTORY | NOT_EXISTING);
        Assert.Throws<InvalidArgumentInputException>(() => p.Set(path));
    }
    
    [Theory]
    [InlineData("/path/")]  
    [InlineData("C:/path/")]
    [InlineData(@"\path\")]
    [InlineData(@"C:\path\")]
    public void Set_PointsToDirectory_WorksProperly(string path) {
        var p = new PathArgument(allowedPathType: DIRECTORY | NOT_EXISTING);
        p.Set(path);
        Assert.Equal(p.Get(), path);
    }
}