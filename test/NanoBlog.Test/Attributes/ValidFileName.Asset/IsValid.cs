namespace NanoBlog.Test.Attributes.ValidFileName.Asset;

public class IsValid
{
    [Fact]
    public void TestValidFileNames()
    {
        var sut = new NanoBlog.Attributes.ValidFileName.Asset();

        sut.IsValid("a-normal-file.png").Should().BeTrue();
        sut.IsValid("a-normal-file.jpg").Should().BeTrue();
        sut.IsValid("a-normal-file.jpeg").Should().BeTrue();
        sut.IsValid("a-normal-file.gif").Should().BeTrue();
        sut.IsValid("a-normal-file.svg").Should().BeTrue();
    }

    [Fact]
    public void TestInvalidFileTypes()
    {
        var sut = new NanoBlog.Attributes.ValidFileName.Asset();

        sut.IsValid("a-normal-file.txt").Should().BeFalse();
        sut.IsValid("a-normal-file.exe").Should().BeFalse();
        sut.IsValid("a-normal-file.jfif").Should().BeFalse();
        sut.IsValid("a-normal-file.").Should().BeFalse();
    }
}