namespace NanoBlog.Services.FileStorages.Export;

public interface IExportFileStorage
{
    public Task WriteContentAsync(Stream content, CancellationToken cancellationToken);
}