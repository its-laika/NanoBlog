namespace NanoBlog.Services.Generation;

public partial class BlogGenerator : IBlogGenerator
{
    private readonly IStageDirectoryContainer _stage;
    private readonly IConfiguration _configuration;

    [GeneratedRegex("\\s{2,}")]
    private static partial Regex ReduceSpacingRegex();

    public BlogGenerator(
        IStageDirectoryContainer stage,
        IConfiguration configuration
    )
    {
        _stage = stage;
        _configuration = configuration;
    }

    public async Task<GeneratedPageContentsContainer> GeneratePageContentsAsync(CancellationToken cancellationToken)
    {
        var fileContents = await LoadStageFileContentsAsync(cancellationToken);

        int? pageSize = _configuration.UsePagination
            ? _configuration.PageSize
            : null;

        var chunks = pageSize is { } chunkSize && fileContents.Posts.Any()
            ? new List<IEnumerable<string>>(fileContents.Posts.Chunk(chunkSize).ToList())
            : new List<IEnumerable<string>> { fileContents.Posts };

        var pages = chunks
           .Select(posts => posts.Reverse())
           .Select((posts, index) => GenerateContent(fileContents.WithPosts(posts), index, chunks.Count))
           .Select(Encoding.UTF8.GetBytes)
           .Select(bytes => new MemoryStream(bytes))
           .ToList();

        return new GeneratedPageContentsContainer(
            pages.Last(),
            pages.Take(pages.Count - 1)
        );
    }

    public async Task<string> GeneratePreviewAsync(CancellationToken cancellationToken)
    {
        var fileContents = await LoadStageFileContentsAsync(cancellationToken);
        return GenerateContent(fileContents);
    }

    private string GenerateContent(
        StageFilesContentContainer stageFileContentContainer,
        int pageNumber = 0,
        int pageCount = 1
    )
    {
        var combinedPosts = stageFileContentContainer.Posts
           .Where(post => !string.IsNullOrWhiteSpace(post))
           .Aggregate(new StringBuilder(), (carry, post) => carry.Append(post))
           .ToString();

        var posts = !string.IsNullOrWhiteSpace(combinedPosts)
            ? $"<div id='posts'>{combinedPosts}</div>"
            : string.Empty;

        const int firstPageNumber = 0;
        var lastPageNumber = pageCount - 1;

        var previousPageNumber = Math.Max(pageNumber - 1, firstPageNumber);
        var followingPageNumber = Math.Min(pageNumber + 1, lastPageNumber);

        var isFirstPage = pageNumber == firstPageNumber;
        var isLastPage = pageNumber == lastPageNumber;

        var previousPageLink = !isFirstPage
            ? BuildPageLink(previousPageNumber, pageCount)
            : "#";

        var followingPageLink = !isLastPage
            ? BuildPageLink(followingPageNumber, pageCount)
            : "#";

        var pagination = $@"
             <nav id='pagination' aria-label='pagination'>
                <ul>
                    <li class='pagination-link previous {(isFirstPage ? "same-page" : string.Empty)}'>
                        <a href='{previousPageLink}'>{previousPageNumber + 1}</a>
                    </li>
                    <li class='pagination-link current same-page'>
                        <a href='#'>{pageNumber + 1}</a>
                    </li>
                    <li class='pagination-link following {(isLastPage ? "same-page" : string.Empty)}'>
                        <a href='{followingPageLink}'>{followingPageNumber + 1}</a>
                    </li>
                </ul>
             </nav>
        ";

        return ReduceSpacingRegex()
           .Replace($@"
                <!DOCTYPE html>
                <html lang='{_configuration.Language}'>
                    <head>
                        {stageFileContentContainer.Header}
                    </head>
                    <body>
                        {stageFileContentContainer.Intro}
                        {posts}
                        {(pageCount > 1 ? pagination : string.Empty)}
                        {stageFileContentContainer.Legal}
                        <footer>
                            {stageFileContentContainer.Footer}
                        </footer>
                    </body>
                </html>",
                " "
            );
    }

    private async Task<StageFilesContentContainer> LoadStageFileContentsAsync(CancellationToken cancellationToken)
    {
        await using var headerStream = _stage.StructureDirectory
           .FindFileInfo(IConfiguration.STAGE_STRUCTURE_FILE_NAME_HEADER)
           .OpenRead();

        await using var introStream = _stage.StructureDirectory
           .FindFileInfo(IConfiguration.STAGE_STRUCTURE_FILE_NAME_INTRO)
           .OpenRead();

        await using var legalStream = _stage.StructureDirectory
           .FindFileInfo(IConfiguration.STAGE_STRUCTURE_FILE_NAME_LEGAL)
           .OpenRead();

        await using var footerStream = _stage.StructureDirectory
           .FindFileInfo(IConfiguration.STAGE_STRUCTURE_FILE_NAME_FOOTER)
           .OpenRead();

        var structureFiles = await Task.WhenAll(
            headerStream.LoadAsStringAsync(cancellationToken),
            introStream.LoadAsStringAsync(cancellationToken),
            legalStream.LoadAsStringAsync(cancellationToken),
            footerStream.LoadAsStringAsync(cancellationToken)
        );

        var postsFiles = await Task.WhenAll(
            _stage.PostsDirectory
               .EnumerateFiles()
               .OrderBy(f => f.Name)
               .Select(async fileInfo =>
                {
                    await using var stream = fileInfo.OpenRead();
                    return await stream.LoadAsStringAsync(cancellationToken);
                })
        );

        return new StageFilesContentContainer(
            structureFiles[0],
            structureFiles[1],
            structureFiles[2],
            structureFiles[3],
            postsFiles
        );
    }

    private string BuildPageLink(int pageNumber, int pageCount)
    {
        if (pageNumber == pageCount - 1)
        {
            return _configuration.BlogRootServerDirectory;
        }

        return Path.Combine(
            _configuration.BlogRootServerDirectory,
            IConfiguration.ARCHIVE_DIRECTORY_NAME,
            pageNumber.ToString(IConfiguration.ARCHIVE_INDEX_FORMAT)
        );
    }
}