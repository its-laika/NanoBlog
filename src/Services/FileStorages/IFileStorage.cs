using NanoBlog.Attributes;

namespace NanoBlog.Services.FileStorages;

public interface IFileStorage
{
    IEnumerable<string> GetFileNames();

    Task<string> LoadContentAsync(
        [ValidFileName] string fileName,
        CancellationToken cancellationToken
    );

    bool FileExists([ValidFileName] string fileName);

    Task WriteContentAsync(
        [ValidFileName] string fileName,
        Stream content,
        CancellationToken cancellationToken
    );
}