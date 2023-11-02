namespace NanoBlog.Services.Exportation;

public class ExportationService : IExportationService
{
    private readonly IBlogGenerator _blogGenerator;
    private readonly IStageDirectoryContainer _stage;
    private readonly IConfiguration _configuration;

    public ExportationService(
        IBlogGenerator blogGenerator,
        IStageDirectoryContainer stage,
        IConfiguration configuration
    )
    {
        _blogGenerator = blogGenerator;
        _stage = stage;
        _configuration = configuration;
    }

    public async Task ExportAsync(CancellationToken cancellationToken)
    {
        var (
            mainPageContent,
            archivePageContents
            ) = await _blogGenerator.GeneratePageContentsAsync(cancellationToken);

        _stage.ExportDirectory.Clear(
            _configuration.ExportKeepFileNames,
            StringComparer.InvariantCultureIgnoreCase
        );

        await Task.WhenAll(
            _stage.AssetsDirectory
               .EnumerateFiles()
               .Select(async fileInfo => await CopyAssetFileAsync(fileInfo, cancellationToken))
               .Concat(new[] { WriteMainPageContentAsync(mainPageContent, cancellationToken) })
               .Concat(WriteArchivePagesAsync(archivePageContents, cancellationToken))
        );
    }

    private async Task WriteMainPageContentAsync(Stream content, CancellationToken cancellationToken)
    {
        await using var targetFileStream = _stage.ExportDirectory
           .CreateFile(IConfiguration.INDEX_FILE_NAME);

        await content.CopyToAsync(targetFileStream, cancellationToken);
    }

    private IEnumerable<Task> WriteArchivePagesAsync(
        IEnumerable<Stream> archivePageContents,
        CancellationToken cancellationToken
    )
    {
        return archivePageContents.Select(
            async (content, index) =>
            {
                await using var archivePageFileStream = _stage.ExportDirectory
                   .CreateSubdirectory(IConfiguration.ARCHIVE_DIRECTORY_NAME)
                   .EnsureSecureMode()
                   .CreateSubdirectory(index.ToString(IConfiguration.ARCHIVE_INDEX_FORMAT))
                   .EnsureSecureMode()
                   .CreateFile(IConfiguration.INDEX_FILE_NAME);

                await content.CopyToAsync(archivePageFileStream, cancellationToken);
            });
    }

    private async Task CopyAssetFileAsync(FileInfo fileInfo, CancellationToken cancellationToken)
    {
        await using var assetFileStream = _stage.ExportDirectory
           .CreateSubdirectory(IConfiguration.ASSETS_DIRECTORY_NAME)
           .EnsureSecureMode()
           .CreateFile(fileInfo.Name);

        await using var readStream = fileInfo.OpenRead();
        await readStream.CopyToAsync(assetFileStream, cancellationToken);
    }
}