using NanoBlog.Services.MimeTypes;

namespace NanoBlog.Services.FileStorages.Assets;

public interface IAssetsFileStorage : IFileStorage
{
    FileStream OpenWriteStream(string fileName);
    new Task<byte[]> LoadContentAsync(FileStream fileStream, CancellationToken cancellationToken);
    FileStream CreateWriteStream(MimeType mimeType);
    Task SynchronizeFilesAsync(CancellationToken cancellationToken);
}