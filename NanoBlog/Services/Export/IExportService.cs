namespace NanoBlog.Services.Export;

public interface IExportService
{
    Task ExportAsync(CancellationToken cancellationToken);
}