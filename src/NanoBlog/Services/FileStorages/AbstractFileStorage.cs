namespace NanoBlog.Services.FileStorages;

public abstract class AbstractFileStorage : IFileStorage
{
    protected readonly DirectoryInfo BaseDirectory;
    private readonly IFileSystemSecurityService _fileSystemSecurityService;

    protected AbstractFileStorage(
        IFileSystemSecurityService fileSystemSecurityService,
        DirectoryInfo baseDirectory
    )
    {
        _fileSystemSecurityService = fileSystemSecurityService;
        BaseDirectory = baseDirectory;

        if (!BaseDirectory.Exists)
        {
            BaseDirectory.Create();
        }

        _fileSystemSecurityService.EnsureSecureMode(BaseDirectory);
    }

    public IEnumerable<FileInfo> GetFileInfos()
    {
        BaseDirectory.Refresh();
        return BaseDirectory.EnumerateFiles();
    }

    public bool FileExists(string fileName)
    {
        return GetFileInfos()
            .Any(f => f.Name.Equals(fileName, StringComparison.InvariantCulture));
    }

    public FileStream OpenReadStream(string fileName)
    {
        return TryOpenReadStream(fileName)
               ?? throw new FileNotFoundException($"Could not find file {fileName}");
    }

    public FileStream? TryOpenReadStream(string fileName)
    {
        var fileInfo = GetFileInfos()
            .SingleOrDefault(f => f.Name.Equals(fileName, StringComparison.InvariantCulture));

        return fileInfo?.Open(FileMode.Open, FileAccess.Read);
    }

    public FileStream? TryOpenWriteStream(string fileName)
    {
        var fileInfo = GetFileInfos()
            .SingleOrDefault(f => f.Name.Equals(fileName, StringComparison.InvariantCulture));

        if (fileInfo is null)
        {
            return null;
        }

        _fileSystemSecurityService.EnsureSecureMode(fileInfo);
        return fileInfo.Open(FileMode.Truncate, FileAccess.Write);
    }

    public async Task<string> LoadContentAsStringAsync(FileStream fileStream, CancellationToken cancellationToken)
    {
        using var streamReader = new StreamReader(fileStream);
        return await streamReader.ReadToEndAsync(cancellationToken);
    }

    public async Task<byte[]> LoadContentAsBytesAsync(FileStream fileStream, CancellationToken cancellationToken)
    {
        var buffer = new byte[fileStream.Length];
        _ = await fileStream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken);
        return buffer.ToArray();
    }

    public void Delete(string fileName)
    {
        var fileInfo = GetFileInfos()
            .SingleOrDefault(f => f.Name.Equals(fileName, StringComparison.InvariantCulture));

        if (fileInfo is null)
        {
            throw new FileNotFoundException();
        }

        fileInfo.Delete();
    }

    public void Truncate()
    {
        foreach (var fileInfo in BaseDirectory.EnumerateFiles())
        {
            fileInfo.Delete();
        }

        foreach (var directoryInfo in BaseDirectory.EnumerateDirectories())
        {
            Directory.Delete(directoryInfo.FullName, true);
        }
    }
}