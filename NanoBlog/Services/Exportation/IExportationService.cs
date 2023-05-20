namespace NanoBlog.Services.Exportation;

public interface IExportationService
{
    Task ExportAsync(CancellationToken cancellationToken);
}