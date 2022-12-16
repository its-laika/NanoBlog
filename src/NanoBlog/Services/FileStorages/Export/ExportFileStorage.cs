using NanoBlog.Services.FileSystemSecurity;

namespace NanoBlog.Services.FileStorages.Export;

public class ExportFileStorage : AbstractFileStorage, IExportFileStorage
{
    private readonly IFileSystemSecurityService _fileSystemSecurityService;
    private const string _EXPORT_FILE_NAME = "index.html";

    public ExportFileStorage(IFileSystemSecurityService fileSystemSecurityService) : base(
        fileSystemSecurityService,
        new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Export"))
    )
    {
        _fileSystemSecurityService = fileSystemSecurityService;
    }

    public async Task WriteContentAsync(Stream content, CancellationToken cancellationToken)
    {
        var targetFileInfo = new FileInfo(Path.Combine(BaseFolder.FullName, _EXPORT_FILE_NAME));

        await using var targetFileStream = targetFileInfo.Exists
            ? targetFileInfo.Open(FileMode.Truncate, FileAccess.Write)
            : targetFileInfo.Create();

        _fileSystemSecurityService.EnsureSecureMode(targetFileInfo);

        await content.CopyToAsync(targetFileStream, cancellationToken);
    }
}