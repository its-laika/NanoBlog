using NanoBlog.Services.FileSystemSecurity;
using NanoBlog.Services.MimeTypes;

namespace NanoBlog.Services.FileStorages.Assets;

public class AssetsFileStorage : AbstractFileStorage, IAssetsFileStorage
{
    private readonly IFileSystemSecurityService _fileSystemSecurityService;
    private readonly DirectoryInfo _targetFolder;

    public AssetsFileStorage(IFileSystemSecurityService fileSystemSecurityService) : base(
        fileSystemSecurityService,
        new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "BlogFiles/Assets"))
    )
    {
        _fileSystemSecurityService = fileSystemSecurityService;
        _targetFolder = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "Export/Assets"));

        if (!_targetFolder.Exists)
        {
            _targetFolder.Create();
        }

        _fileSystemSecurityService.EnsureSecureMode(_targetFolder);
    }

    public FileStream OpenWriteStream(string fileName)
    {
        return TryOpenWriteStream(fileName)
               ?? throw new FileNotFoundException($"Could not find file {fileName}");
    }

    public new async Task<byte[]> LoadContentAsync(FileStream fileStream, CancellationToken cancellationToken)
    {
        var buffer = new byte[fileStream.Length];
        _ = await fileStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
        return buffer.ToArray();
    }

    public FileStream CreateWriteStream(MimeType mimeType)
    {
        var fileName = $"{DateTime.UtcNow.Ticks}-{Guid.NewGuid()}.{mimeType.GetExtension()}";
        var fileInfo = new FileInfo(Path.Combine(BaseFolder.FullName, fileName));

        var fileStream = fileInfo.Create();
        _fileSystemSecurityService.EnsureSecureMode(fileInfo);

        return fileStream;
    }

    public async Task TransferAsync(CancellationToken cancellationToken)
    {
        BaseFolder.Refresh();

        foreach (var baseFileInfo in BaseFolder.EnumerateFiles())
        {
            var targetFileInfo = new FileInfo(Path.Combine(_targetFolder.FullName, baseFileInfo.Name));

            await using var targetFileStream = targetFileInfo.Exists
                ? targetFileInfo.Open(FileMode.Truncate, FileAccess.Write)
                : targetFileInfo.Create();
            _fileSystemSecurityService.EnsureSecureMode(targetFileInfo);

            await using var baseFileStream = baseFileInfo.Open(FileMode.Open, FileAccess.Read);
            await baseFileStream.CopyToAsync(targetFileStream, cancellationToken);
        }

        _targetFolder.Refresh();

        var targetFileInfosToRemove = _targetFolder.EnumerateFiles()
            .ExceptBy(BaseFolder.EnumerateFiles().Select(f => f.Name), info => info.Name);

        foreach (var targetFileInfo in targetFileInfosToRemove)
        {
            targetFileInfo.Delete();
        }
    }
}