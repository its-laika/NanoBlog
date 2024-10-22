namespace NanoBlog.Extensions;

public static class FileInfoExtensions
{
    private const UnixFileMode FileMode =
        UnixFileMode.UserRead
        | UnixFileMode.UserWrite
        | UnixFileMode.GroupRead
        | UnixFileMode.OtherRead;

    public static FileInfo EnsureFileMode(this FileInfo fileInfo)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            fileInfo.UnixFileMode = FileMode;
        }

        return fileInfo;
    }

    /// <summary>
    /// Opens writeable FileStream for given FileInfo.
    /// Truncates file beforehand.
    /// </summary>
    public static FileStream OpenWriteStream(this FileInfo fileInfo)
    {
        return fileInfo.Open(System.IO.FileMode.Truncate, FileAccess.Write);
    }
}