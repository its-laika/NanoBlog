namespace NanoBlog.Test.Attributes.ValidFileName.All;

public class IsValid
{
    [Fact]
    public void TestValidFileName()
    {
        var sut = new NanoBlog.Attributes.ValidFileName.All();

        Assert.True(sut.IsValid("a-normal-file.txt"));
        Assert.True(sut.IsValid("a-normal-file.png"));
        Assert.True(sut.IsValid("a-normal-file.jpg"));
        Assert.True(sut.IsValid("a-normal-file.jpeg"));
        Assert.True(sut.IsValid("a-normal-file.gif"));
        Assert.True(sut.IsValid("a-normal-file.svg"));
        Assert.True(sut.IsValid("index.html"));
    }

    [Fact]
    public void TestInvalidFileTypes()
    {
        var sut = new NanoBlog.Attributes.ValidFileName.All();

        Assert.False(sut.IsValid("What\tare whitespaces anyway?.txt"));
        Assert.False(sut.IsValid("__.txt"));
        Assert.False(sut.IsValid("/../../../etc/passwd.txt"));
        Assert.False(sut.IsValid("🏳‍🌈🏳‍🌈🏳‍🌈.txt"));
        Assert.False(sut.IsValid("https://raw.githubusercontent.com/its-laika/NanoBlog/main/src/BlogFiles/Structure/footer.txt"));
        Assert.False(sut.IsValid("totally-harmless.exe"));
        Assert.False(sut.IsValid("a-normal-file."));
        Assert.False(sut.IsValid("~/test.txt"));
        Assert.False(sut.IsValid("~.txt")); /* avoid confusion */
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
        Assert.True(sut.IsValid(validFileName));
        Assert.False(sut.IsValid(invalidFileName));
    }
}