namespace NanoBlog.Test.Attributes.ValidFileName.Text;

public class IsValid
{
    [Fact]
    public void TestValidFileName()
    {
        var sut = new NanoBlog.Attributes.ValidFileName.Text();

        sut.IsValid("a-normal-file.txt").Should().BeTrue();
        sut.IsValid("THIS-is--1-tEsT.txt").Should().BeTrue();
    }

    [Fact]
    public void TestInvalidFileName()
    {
        var sut = new NanoBlog.Attributes.ValidFileName.Text();

        sut.IsValid("What\tare whitespaces anyway?.txt").Should().BeFalse();
        sut.IsValid("__.txt").Should().BeFalse();
        sut.IsValid("/../../../etc/passwd.txt").Should().BeFalse();
        sut.IsValid("🏳‍🌈🏳‍🌈🏳‍🌈.txt").Should().BeFalse();
        sut.IsValid("https://raw.githubusercontent.com/neon-JS/NanoBlog/main/src/BlogFiles/Structure/footer.txt")
            .Should()
            .BeFalse();
        sut.IsValid("totally-harmless.exe").Should().BeFalse();
    }

    [Fact]
    public void TestFileNameLength()
    {
        var stringBuilder = new StringBuilder();
        for (var i = 0; i < 96; i++)
        {
            stringBuilder.Append('A');
        }

        stringBuilder.Append(".txt");

        var validFileName = stringBuilder.ToString();
        var invalidFileName = 'A' + stringBuilder.ToString();

        var sut = new NanoBlog.Attributes.ValidFileName.Text();
        sut.IsValid(validFileName).Should().BeTrue();
        sut.IsValid(invalidFileName).Should().BeFalse();
    }
}