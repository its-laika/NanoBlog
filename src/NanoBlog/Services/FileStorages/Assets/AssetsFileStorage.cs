namespace NanoBlog.Services.FileStorages.Assets;

public class AssetsFileStorage : AbstractFileStorage, IAssetsFileStorage
{
    private readonly IFileSystemSecurityService _fileSystemSecurityService;

    public AssetsFileStorage(
        IFileSystemSecurityService fileSystemSecurityService
    ) : base(
        fileSystemSecurityService,
        Configuration.GetStageAssetsDirectoryInfo()
    )
    {
        _fileSystemSecurityService = fileSystemSecurityService;
    }

    public FileStream CreateNewFileWriteStreamByMimeType(MimeType mimeType)
    {
        var fileName = $"{DateTime.UtcNow.Ticks}-{Guid.NewGuid()}.{mimeType.GetExtension()}";
        var fileInfo = new FileInfo(Path.Combine(BaseDirectory.FullName, fileName));

        var fileStream = fileInfo.Create();
        _fileSystemSecurityService.EnsureSecureMode(fileInfo);

        return fileStream;
    }
}