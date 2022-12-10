namespace NanoBlog.Services.FileStorages;

public class ExportFileStorage : AbstractFileStorage, IExportFileStorage
{
    protected override string BaseFolder => "Export";

    public new async Task WriteContentAsync(string fileName, Stream content, CancellationToken cancellationToken)
    {
        var targetFilePath = Path.Combine(BaseFolder, fileName);

        if (!File.Exists(targetFilePath))
        {
            File.Create(targetFilePath).Close();
        }

        await base.WriteContentAsync(fileName, content, cancellationToken);
    }
}