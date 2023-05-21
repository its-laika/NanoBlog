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

    /// <summary>
    /// Opens writeable FileStream for given FileInfo.
    /// Truncates file beforehand.
    /// </summary>
    public static FileStream OpenWriteStream(this FileInfo fileInfo)
    {
        return fileInfo.Open(FileMode.Truncate, FileAccess.Write);
    }
}