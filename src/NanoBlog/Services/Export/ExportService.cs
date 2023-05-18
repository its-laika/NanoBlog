namespace NanoBlog.Services.Export;

public class ExportService : IExportService
{
    private readonly IBlogGenerator _blogGenerator;
    private readonly IExportFileStorage _exportFileStorage;
    private readonly IAssetsFileStorage _assetsFileStorage;

    public ExportService(
        IBlogGenerator blogGenerator,
        IExportFileStorage exportFileStorage,
        IAssetsFileStorage assetsFileStorage
    )
    {
        _blogGenerator = blogGenerator;
        _exportFileStorage = exportFileStorage;
        _assetsFileStorage = assetsFileStorage;
    }

    public async Task ExportAsync(CancellationToken cancellationToken)
    {
        var (
            mainPageContent,
            archivePageContents
            ) = await _blogGenerator.GeneratePageContentsAsync(cancellationToken);

        _exportFileStorage.Truncate();

        await Task.WhenAll(
            _assetsFileStorage.GetFileInfos()
                .Select(
                    async fileInfo => await _exportFileStorage.CopyAssetFileAsync(fileInfo, cancellationToken)
                )
                .Concat(
                    archivePageContents.Select(
                        async (content, index) => await _exportFileStorage.WriteArchivePageContentAsync(
                            content,
                            index,
                            cancellationToken
                        )
                    )
                )
                .Concat(
                    new[] { _exportFileStorage.WriteMainPageContentAsync(mainPageContent, cancellationToken) }
                )
        );
    }
}