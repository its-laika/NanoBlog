namespace NanoBlog.Test.Attributes.ValidFileName.All;

public class IsValid
{
    [Fact]
    public void TestValidFileName()
    {
        var sut = new NanoBlog.Attributes.ValidFileName.All();

        sut.IsValid("a-normal-file.txt").Should().BeTrue();
        sut.IsValid("a-normal-file.png").Should().BeTrue();
        sut.IsValid("a-normal-file.jpg").Should().BeTrue();
        sut.IsValid("a-normal-file.jpeg").Should().BeTrue();
        sut.IsValid("a-normal-file.gif").Should().BeTrue();
        sut.IsValid("a-normal-file.svg").Should().BeTrue();
        sut.IsValid("index.html").Should().BeTrue();
    }

    [Fact]
    public void TestInvalidFileTypes()
    {
        var sut = new NanoBlog.Attributes.ValidFileName.All();

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

        var sut = new NanoBlog.Attributes.ValidFileName.All();
        sut.IsValid(validFileName).Should().BeTrue();
        sut.IsValid(invalidFileName).Should().BeFalse();
    }
}