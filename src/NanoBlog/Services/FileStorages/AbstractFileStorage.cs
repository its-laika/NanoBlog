using NanoBlog.Services.FileSystemSecurity;

namespace NanoBlog.Services.FileStorages;

public abstract class AbstractFileStorage : IFileStorage
{
    protected readonly DirectoryInfo BaseFolder;
    private readonly IFileSystemSecurityService _fileSystemSecurityService;

    protected AbstractFileStorage(
        IFileSystemSecurityService fileSystemSecurityService,
        DirectoryInfo baseFolder
    )
    {
        _fileSystemSecurityService = fileSystemSecurityService;
        BaseFolder = baseFolder;

        if (!BaseFolder.Exists)
        {
            BaseFolder.Create();
        }

        _fileSystemSecurityService.EnsureSecureMode(BaseFolder);
    }

    public IEnumerable<string> GetFileNames()
    {
        BaseFolder.Refresh();

        return BaseFolder
            .EnumerateFiles()
            .Select(f => f.Name);
    }

    public bool FileExists(string fileName)
    {
        BaseFolder.Refresh();

        return BaseFolder
            .GetFiles()
            .Any(f => f.Name.Equals(fileName, StringComparison.InvariantCulture));
    }

    public FileStream OpenReadStream(string fileName)
    {
        return TryOpenReadStream(fileName)
               ?? throw new FileNotFoundException($"Could not find file {fileName}");
    }

    public FileStream? TryOpenReadStream(string fileName)
    {
        BaseFolder.Refresh();

        var fileInfo = BaseFolder
            .EnumerateFiles()
            .SingleOrDefault(f => f.Name.Equals(fileName, StringComparison.InvariantCulture));

        return fileInfo?.Open(FileMode.Open, FileAccess.Read);
    }

    public FileStream? TryOpenWriteStream(string fileName)
    {
        BaseFolder.Refresh();

        var fileInfo = BaseFolder
            .EnumerateFiles()
            .SingleOrDefault(f => f.Name.Equals(fileName, StringComparison.InvariantCulture));

        if (fileInfo is null)
        {
            return null;
        }

        _fileSystemSecurityService.EnsureSecureMode(fileInfo);
        return fileInfo.Open(FileMode.Truncate, FileAccess.Write);
    }

    public async Task<string> LoadContentAsync(FileStream fileStream, CancellationToken cancellationToken)
    {
        using var streamReader = new StreamReader(fileStream);
        return await streamReader.ReadToEndAsync(cancellationToken);
    }

    public void Delete(string fileName)
    {
        BaseFolder.Refresh();

        var fileInfo = BaseFolder
            .EnumerateFiles()
            .SingleOrDefault(f => f.Name.Equals(fileName, StringComparison.InvariantCulture));

        if (fileInfo is null)
        {
            throw new FileNotFoundException();
        }

        fileInfo.Delete();
    }

    public FileStream CreateWriteStream()
    {
        var fileName = $"{DateTime.UtcNow.Ticks}-{Guid.NewGuid()}.txt";
        var fileInfo = new FileInfo(Path.Combine(BaseFolder.FullName, fileName));

        var fileStream = fileInfo.Create();
        _fileSystemSecurityService.EnsureSecureMode(fileInfo);

        return fileStream;
    }
}