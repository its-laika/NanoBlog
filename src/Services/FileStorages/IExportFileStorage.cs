namespace NanoBlog.Services.FileStorages;

public interface IExportFileStorage : IFileStorage
{
    public const string ExportFileName = "index.html";
}