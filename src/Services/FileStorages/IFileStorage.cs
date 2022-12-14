using NanoBlog.Attributes;

namespace NanoBlog.Services.FileStorages;

public interface IFileStorage
{
    IEnumerable<string> GetFileNames();
    bool FileExists([ValidFileName] string fileName);

    FileStream OpenReadStream([ValidFileName] string fileName);
    FileStream? TryOpenReadStream([ValidFileName] string fileName);
    FileStream? TryOpenWriteStream([ValidFileName] string fileName);

    Task<string> LoadContentAsync(FileStream fileStream, CancellationToken cancellationToken);
    Task WriteContentAsync(FileStream fileStream, Stream content, CancellationToken cancellationToken);

    void Delete([ValidFileName] string fileName);

    FileStream Create();
}