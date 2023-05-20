namespace NanoBlog.Extensions;

public static class FileInfoExtensions
{
    private const UnixFileMode _FILE_MODE =
        UnixFileMode.UserRead
        | UnixFileMode.UserWrite
        | UnixFileMode.GroupRead
        | UnixFileMode.OtherRead;

    public static FileInfo EnsureFileMode(this FileInfo fileInfo)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            fileInfo.UnixFileMode = _FILE_MODE;
        }

        return fileInfo;
    }
}