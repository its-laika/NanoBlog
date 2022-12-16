namespace NanoBlog.Services.FileSystemSecurity;

public class WindowsFileSystemSecurityService : IFileSystemSecurityService
{
    private readonly ILogger<WindowsFileSystemSecurityService> _logger;

    public WindowsFileSystemSecurityService(ILogger<WindowsFileSystemSecurityService> logger)
    {
        _logger = logger;
    }

    public void EnsureSecureMode(DirectoryInfo directoryInfo)
    {
        // TODO
        _logger.LogWarning("Could not set secure file mode because this is a Windows environment");
    }

    public void EnsureSecureMode(FileInfo fileInfo)
    {
        // TODO
        _logger.LogWarning("Could not set secure file mode because this is a Windows environment");
    }
}