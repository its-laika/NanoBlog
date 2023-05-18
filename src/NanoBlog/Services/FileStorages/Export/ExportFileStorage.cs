namespace NanoBlog.Services.FileStorages.Export;

public class ExportFileStorage : AbstractFileStorage, IExportFileStorage
{
    private readonly IFileSystemSecurityService _fileSystemSecurityService;

    public ExportFileStorage(
        IFileSystemSecurityService fileSystemSecurityService
    ) : base(
        fileSystemSecurityService,
        Configuration.GetExportDirectoryInfo()
    )
    {
        _fileSystemSecurityService = fileSystemSecurityService;
    }

    public async Task WriteMainPageContentAsync(Stream content, CancellationToken cancellationToken)
    {
        var targetFileInfo = new FileInfo(Path.Combine(BaseDirectory.FullName, IConfiguration.INDEX_FILE_NAME));

        await using var targetFileStream = targetFileInfo.Exists
            ? targetFileInfo.Open(FileMode.Truncate, FileAccess.Write)
            : targetFileInfo.Create();

        _fileSystemSecurityService.EnsureSecureMode(targetFileInfo);

        await content.CopyToAsync(targetFileStream, cancellationToken);
    }

    public async Task WriteArchivePageContentAsync(
        Stream content,
        int archiveIndex,
        CancellationToken cancellationToken
    )
    {
        var archiveDirectoryPath = Path.Combine(
            BaseDirectory.FullName,
            IConfiguration.ARCHIVE_DIRECTORY_NAME
        );

        var archivePageDirectoryPath = Path.Combine(
            archiveDirectoryPath,
            archiveIndex.ToString(IConfiguration.ARCHIVE_INDEX_FORMAT)
        );

        var archivePageFilePath = Path.Combine(
            archivePageDirectoryPath,
            IConfiguration.INDEX_FILE_NAME
        );

        var archiveDirectoryInfo = Directory.CreateDirectory(archiveDirectoryPath);
        _fileSystemSecurityService.EnsureSecureMode(archiveDirectoryInfo);

        var archivePageDirectoryInfo = Directory.CreateDirectory(archivePageDirectoryPath);
        _fileSystemSecurityService.EnsureSecureMode(archivePageDirectoryInfo);

        var targetFileInfo = new FileInfo(archivePageFilePath);

        await using var targetFileStream = targetFileInfo.Exists
            ? targetFileInfo.Open(FileMode.Truncate, FileAccess.Write)
            : targetFileInfo.Create();

        _fileSystemSecurityService.EnsureSecureMode(targetFileInfo);

        await content.CopyToAsync(targetFileStream, cancellationToken);
    }

    public async Task CopyAssetFileAsync(FileInfo fileInfo, CancellationToken cancellationToken)
    {
        var assetDirectoryPath = Path.Combine(BaseDirectory.FullName, IConfiguration.ASSETS_DIRECTORY_NAME);
        var assetDirectoryInfo = Directory.CreateDirectory(assetDirectoryPath);
        _fileSystemSecurityService.EnsureSecureMode(assetDirectoryInfo);

        var targetFileInfo = new FileInfo(Path.Combine(assetDirectoryInfo.FullName, fileInfo.Name));

        await using var targetFileStream = targetFileInfo.Exists
            ? targetFileInfo.Open(FileMode.Truncate, FileAccess.Write)
            : targetFileInfo.Create();

        _fileSystemSecurityService.EnsureSecureMode(targetFileInfo);

        await using var readStream = fileInfo.OpenRead();
        await readStream.CopyToAsync(targetFileStream, cancellationToken);
    }
}