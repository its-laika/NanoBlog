namespace NanoBlog.Services.FileStorages.Export;

public class ExportFileStorage : AbstractFileStorage, IExportFileStorage
{
    protected override string BaseFolder { get; }
    private const string _EXPORT_FILE_NAME = "index.html";

    public ExportFileStorage(ILogger<AbstractFileStorage> logger) : base(logger)
    {
        BaseFolder = Path.Combine(Directory.GetCurrentDirectory(), "Export");
    }

    public async Task WriteContentAsync(Stream content, CancellationToken cancellationToken)
    {
        var targetFilePath = Path.Combine(BaseFolder, _EXPORT_FILE_NAME);

        await using var fileStream = File.Exists(targetFilePath)
            ? File.Open(targetFilePath, FileMode.Truncate)
            : File.Create(targetFilePath);

        SetSecureUnixFileMode(fileStream.SafeFileHandle);
        await content.CopyToAsync(fileStream, cancellationToken);
    }
}