namespace NanoBlog.Services.FileSystemSecurity;

public interface IFileSystemSecurityService
{
    void EnsureSecureMode(DirectoryInfo directoryInfo);
    void EnsureSecureMode(FileInfo fileInfo);
}