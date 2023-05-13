using NanoBlog.Services.FileSystemSecurity;

namespace NanoBlog.Services.FileStorages.Export;

public class ExportFileStorage : AbstractFileStorage, IExportFileStorage
{
    private readonly IFileSystemSecurityService _fileSystemSecurityService;

    public ExportFileStorage(IFileSystemSecurityService fileSystemSecurityService) : base(
        fileSystemSecurityService,
        new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Export"))
    )
    {
        _fileSystemSecurityService = fileSystemSecurityService;
    }

    public async Task WriteContentsAsync(IDictionary<string, Stream> pageMapping, CancellationToken cancellationToken)
    {
        foreach (var (fileName, content) in pageMapping)
        {
            var targetFileInfo = new FileInfo(Path.Combine(BaseFolder.FullName, fileName));

            await using var targetFileStream = targetFileInfo.Exists
                ? targetFileInfo.Open(FileMode.Truncate, FileAccess.Write)
                : targetFileInfo.Create();

            _fileSystemSecurityService.EnsureSecureMode(targetFileInfo);

            await content.CopyToAsync(targetFileStream, cancellationToken);
        }

        foreach (var outdatedFileName in GetFileNames().Except(pageMapping.Keys, StringComparer.InvariantCulture))
        {
            Delete(outdatedFileName);
        }
    }
}
