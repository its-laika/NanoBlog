namespace NanoBlog.Services.FileStorages.Export;

public interface IExportFileStorage : IFileStorage
{
    Task WriteMainPageContentAsync(Stream content, CancellationToken cancellationToken);
    Task WriteArchivePageContentAsync(Stream content, int archiveIndex, CancellationToken cancellationToken);
    Task CopyAssetFileAsync(FileInfo fileInfo, CancellationToken cancellationToken);
}