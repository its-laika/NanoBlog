namespace NanoBlog.Services.FileStorages.Export;

public interface IExportFileStorage
{
    Task WriteContentsAsync(IDictionary<string, Stream> pageMapping, CancellationToken cancellationToken);
}