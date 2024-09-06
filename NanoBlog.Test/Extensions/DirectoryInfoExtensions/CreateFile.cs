using NanoBlog.Extensions;

namespace NanoBlog.Test.Extensions.DirectoryInfoExtensions;

public class CreateFile
{
    [Fact]
    public void TestInvalidFileNames()
    {
        var directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());

        Assert.Throws<ArgumentException>(() => directoryInfo.CreateFile("What\tare whitespaces anyway?.txt"));
        Assert.Throws<ArgumentException>(() => directoryInfo.CreateFile("../passwd.txt"));
        Assert.Throws<ArgumentException>(() => directoryInfo.CreateFile("/etc/passwd.txt"));
        Assert.Throws<ArgumentException>(() => directoryInfo.CreateFile("🏳‍🌈🏳‍🌈🏳‍🌈.txt"));
        Assert.Throws<ArgumentException>(() => directoryInfo.CreateFile(
            "https://raw.githubusercontent.com/its-laika/NanoBlog/main/src/BlogFiles/Structure/footer.txt"
        ));
        Assert.Throws<ArgumentException>(() => directoryInfo.CreateFile("totally-harmless.exe"));
        Assert.Throws<ArgumentException>(() => directoryInfo.CreateFile("../test.txt"));
        Assert.Throws<ArgumentException>(() => directoryInfo.CreateFile("~/test.txt"));
        Assert.Throws<ArgumentException>(() => directoryInfo.CreateFile("~.txt"));
    }
}