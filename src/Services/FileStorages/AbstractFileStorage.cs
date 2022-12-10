using NanoBlog.Attributes;

namespace NanoBlog.Services.FileStorages;

public abstract class AbstractFileStorage : IFileStorage
{
    protected abstract string BaseFolder { get; }

    public IEnumerable<string> GetFileNames()
    {
        return Directory.GetFiles(BaseFolder)
            .Select(fullPath => fullPath.Replace(BaseFolder, string.Empty))
            .Select(fullPath => fullPath.Replace("/", string.Empty));
    }

    public async Task<string> LoadContentAsync(
        [ValidFileName] string fileName,
        CancellationToken cancellationToken
    )
    {
        var filePath = Path.Combine(BaseFolder, fileName);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException();
        }

        return await File.ReadAllTextAsync(filePath, cancellationToken);
    }

    public bool FileExists([ValidFileName] string fileName)
    {
        var filePath = Path.Combine(BaseFolder, fileName);
        return File.Exists(filePath);
    }

    public async Task WriteContentAsync(
        [ValidFileName] string fileName,
        Stream content,
        CancellationToken cancellationToken
    )
    {
        var targetFilePath = Path.Combine(BaseFolder, fileName);

        if (!File.Exists(targetFilePath))
        {
            throw new FileNotFoundException();
        }

        await using var fileHandle = File.Open(targetFilePath, FileMode.Open);
        await content.CopyToAsync(fileHandle, cancellationToken);
    }
}