namespace NanoBlog.Services.FileSystemSecurity;

#pragma warning disable CA1416
public class UnixFileSystemSecurityService : IFileSystemSecurityService
{
    private const UnixFileMode _FILE_MODE =
        UnixFileMode.UserRead
        | UnixFileMode.UserWrite
        | UnixFileMode.GroupRead
        | UnixFileMode.OtherRead;

    private const UnixFileMode _DIRECTORY_MODE =
        _FILE_MODE
        | UnixFileMode.UserExecute
        | UnixFileMode.GroupExecute
        | UnixFileMode.OtherExecute;

    public void EnsureSecureMode(DirectoryInfo directoryInfo)
    {
        directoryInfo.UnixFileMode = _DIRECTORY_MODE;
    }

    public void EnsureSecureMode(FileInfo fileInfo)
    {
        fileInfo.UnixFileMode = _FILE_MODE;
    }
}
#pragma warning restore CA1416