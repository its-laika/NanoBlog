using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using NanoBlog.Attributes;

namespace NanoBlog.Services.FileStorages;

public abstract class AbstractFileStorage : IFileStorage
{
    protected abstract string BaseFolder { get; }

    private const UnixFileMode _TARGET_UNIX_FILE_MODE =
        UnixFileMode.UserRead
        | UnixFileMode.UserWrite
        | UnixFileMode.GroupRead
        | UnixFileMode.OtherRead;

    private readonly ILogger<AbstractFileStorage> _logger;

    protected AbstractFileStorage(ILogger<AbstractFileStorage> logger)
    {
        _logger = logger;
    }

    public IEnumerable<string> GetFileNames()
    {
        return Directory.GetFiles(BaseFolder)
            .Select(fullPath => fullPath.Replace(BaseFolder, string.Empty))
            .Select(fullPath => fullPath.Replace("/", string.Empty));
    }

    public bool FileExists([ValidFileName] string fileName)
    {
        var filePath = Path.Combine(BaseFolder, fileName);
        return File.Exists(filePath);
    }

    public FileStream OpenReadStream([ValidFileName] string fileName)
    {
        var filePath = Path.Combine(BaseFolder, fileName);

        return File.Exists(filePath)
            ? File.Open(filePath, FileMode.Open, FileAccess.Read)
            : throw new FileNotFoundException($"Could not find file {fileName}");
    }

    public FileStream? TryOpenReadStream([ValidFileName] string fileName)
    {
        var filePath = Path.Combine(BaseFolder, fileName);

        return File.Exists(filePath)
            ? File.Open(filePath, FileMode.Open, FileAccess.Read)
            : null;
    }

    public FileStream? TryOpenWriteStream([ValidFileName] string fileName)
    {
        var filePath = Path.Combine(BaseFolder, fileName);

        return File.Exists(filePath)
            ? File.Open(filePath, FileMode.Truncate, FileAccess.Write)
            : null;
    }

    public async Task<string> LoadContentAsync(
        FileStream fileStream,
        CancellationToken cancellationToken
    )
    {
        var streamReader = new StreamReader(fileStream);
        return await streamReader.ReadToEndAsync(cancellationToken);
    }

    public async Task WriteContentAsync(
        FileStream fileStream,
        Stream content,
        CancellationToken cancellationToken
    )
    {
        SetSecureUnixFileMode(fileStream.SafeFileHandle);
        await content.CopyToAsync(fileStream, cancellationToken);
    }

    public void Delete([ValidFileName] string fileName)
    {
        var filePath = Path.Combine(BaseFolder, fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException();
        }

        File.Delete(filePath);
    }

    public FileStream Create()
    {
        var fileName = $"{DateTime.UtcNow.Ticks}-{Guid.NewGuid()}.txt";
        var targetFilePath = Path.Combine(BaseFolder, fileName);

        var fileStream = File.Open(targetFilePath, FileMode.CreateNew);

        SetSecureUnixFileMode(fileStream.SafeFileHandle);

        return fileStream;
    }

    protected void SetSecureUnixFileMode(SafeFileHandle fileHandle)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            File.SetUnixFileMode(fileHandle, _TARGET_UNIX_FILE_MODE);
            return;
        }

        // TODO: Set file attributes for windows
        _logger.LogWarning("Secure file mode not supported on this platform.");
    }
}