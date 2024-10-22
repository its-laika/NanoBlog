namespace NanoBlog.Services.Export;

public class ExportService(IBlogGenerator blogGenerator, IConfiguration configuration) : IExportService
{
    private const string IndexFileName = "index.html";
    private const string ArchiveIndexFormat = "D";

    public async Task ExportAsync(CancellationToken cancellationToken)
    {
        var pages = await blogGenerator.GeneratePageContentsAsync(cancellationToken);

        var archivePages = pages.SkipLast(1);
        var frontPage = pages.Last();

        configuration.GetExportDirectoryInfo().Clear(
            configuration.KeepExportFiles,
            StringComparer.InvariantCultureIgnoreCase);

        await Task.WhenAll(
            configuration
                .GetAssetsDirectoryInfo()
                .EnumerateFiles()
                .Select(fileInfo =>
                {
                    CopyAssetFile(fileInfo);
                    return Task.CompletedTask;
                })
                .Concat([WriteFrontPageContentAsync(frontPage, cancellationToken)])
                .Concat(WriteArchivePagesAsync(archivePages, cancellationToken))
        );
    }

    private async Task WriteFrontPageContentAsync(Stream content, CancellationToken cancellationToken)
    {
        await using var targetFileStream = configuration
            .GetExportDirectoryInfo()
            .CreateFile(IndexFileName);

        await content.CopyToAsync(targetFileStream, cancellationToken);
    }

    private IEnumerable<Task> WriteArchivePagesAsync(
        IEnumerable<Stream> archivePageContents,
        CancellationToken cancellationToken)
    {
        return archivePageContents.Select(
            async (content, index) =>
            {
                await using var archivePageFileStream = configuration
                    .GetExportArchiveDirectoryInfo()
                    .EnsureSecureMode()
                    .CreateSubdirectory(index.ToString(ArchiveIndexFormat))
                    .EnsureSecureMode()
                    .CreateFile(IndexFileName);

                await content.CopyToAsync(archivePageFileStream, cancellationToken);
            });
    }

    private void CopyAssetFile(FileInfo fileInfo)
    {
        var assetDirectory = configuration
            .GetExportAssetsDirectoryInfo()
            .EnsureSecureMode();

        var targetPath = Path.Combine(assetDirectory.FullName, fileInfo.Name);

        fileInfo.CopyTo(targetPath);
    }
}
