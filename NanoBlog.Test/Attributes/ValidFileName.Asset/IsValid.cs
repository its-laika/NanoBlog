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
        sut.IsValid("a-normal-file.jfif").Should().BeFalse();

        sut.IsValid("What\tare whitespaces anyway?.txt").Should().BeFalse();
        sut.IsValid("__.txt").Should().BeFalse();
        sut.IsValid("/../../../etc/passwd.txt").Should().BeFalse();
        sut.IsValid("🏳‍🌈🏳‍🌈🏳‍🌈.txt").Should().BeFalse();
        sut.IsValid("https://raw.githubusercontent.com/its-laika/NanoBlog/main/src/BlogFiles/Structure/footer.txt")
           .Should()
           .BeFalse();
        sut.IsValid("totally-harmless.exe").Should().BeFalse();
        sut.IsValid("a-normal-file.").Should().BeFalse();
        sut.IsValid("~/test.txt").Should().BeFalse();
        sut.IsValid("~.txt").Should().BeFalse(); /* avoid confusion */
    }

    [Fact]
    public void TestFileNameLength()
    {
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < 96; i++)
        {
            stringBuilder.Append('A');
        }

        stringBuilder.Append(".jpg");

        var validFileName = stringBuilder.ToString();
        var invalidFileName = 'A' + stringBuilder.ToString();

        var sut = new NanoBlog.Attributes.ValidFileName.Asset();
        sut.IsValid(validFileName).Should().BeTrue();
        sut.IsValid(invalidFileName).Should().BeFalse();
    }
}