using System.Text.RegularExpressions;
using NanoBlog.Services.FileStorages.Posts;
using NanoBlog.Services.FileStorages.Structure;
using NanoBlog.Extensions;
using System.Text;

namespace NanoBlog.Services.Generation;

public partial class BlogGenerator : IBlogGenerator
{
    private readonly IPostsFileStorage _postsFileStorage;
    private readonly IStructureFileStorage _structureFileStorage;
    private readonly IConfiguration _configuration;

    [GeneratedRegex("\\s{2,}")]
    private static partial Regex ReduceSpacingRegex();

    public BlogGenerator(
        IPostsFileStorage postsFileStorage,
        IStructureFileStorage structureFileStorage,
        IConfiguration configuration
    )
    {
        _postsFileStorage = postsFileStorage;
        _structureFileStorage = structureFileStorage;
        _configuration = configuration;
    }

    public async Task<IDictionary<string, Stream>> GeneratePageMappingAsync(CancellationToken cancellationToken)
    {
        var fileContents = await LoadFileContentsAsync(cancellationToken);

        int? pageSize = _configuration.UsePagination
            ? _configuration.PageSize
            : null;

        var chunks = pageSize is { } chunkSize
            ? new List<IEnumerable<string>>(fileContents.Posts.Chunk(chunkSize).ToList())
            : new List<IEnumerable<string>> { fileContents.Posts };

        return chunks
            .Select(posts => posts.Reverse())
            .Select((posts, index) => GenerateContent(fileContents.WithPosts(posts), index, chunks.Count))
            .Select(Encoding.UTF8.GetBytes)
            .Select(bytes => new MemoryStream(bytes))
            .ToDictionary(
                (_, index) => BuildPageFileName(index, chunks.Count),
                (stream, _) => stream as Stream
            );
    }

    public async Task<string> GeneratePreviewAsync(CancellationToken cancellationToken)
    {
        var fileContents = await LoadFileContentsAsync(cancellationToken);
        return GenerateContent(fileContents);
    }

    private string GenerateContent(FilesContentContainer fileContents, int pageNumber = 0, int pageCount = 1)
    {
        var combinedPosts = fileContents.Posts
            .Where(post => !string.IsNullOrWhiteSpace(post))
            .Aggregate(new StringBuilder(), (carry, post) => carry.Append(post))
            .ToString();

        const int firstPageNumber = 0;
        var lastPageNumber = pageCount - 1;

        var previousPageNumber = Math.Max(pageNumber - 1, firstPageNumber);
        var followingPageNumber = Math.Min(pageNumber + 1, lastPageNumber);

        var isFirstPage = pageNumber == firstPageNumber;
        var isLastPage = pageNumber == lastPageNumber;

        var previousPageLink = !isFirstPage
            ? $"../{BuildPageFileName(previousPageNumber, pageCount)}"
            : "#";

        var followingPageLink = !isLastPage
            ? $"../{BuildPageFileName(followingPageNumber, pageCount)}"
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
                        {fileContents.Header}
                    </head>
                    <body>
                        {fileContents.Intro}
                        <div id='posts'>
                            {combinedPosts}
                        </div>
                        {(pageCount > 1 ? pagination : string.Empty)}
                        {fileContents.Legal}
                        <footer>
                            {fileContents.Footer}
                        </footer>
                    </body>
                </html>",
                " "
            );
    }

    private async Task<FilesContentContainer> LoadFileContentsAsync(CancellationToken cancellationToken)
    {
        await using var headerStream = _structureFileStorage.OpenReadStream(IStructureFileStorage.FILE_NAME_HEADER);
        await using var introStream = _structureFileStorage.OpenReadStream(IStructureFileStorage.FILE_NAME_INTRO);
        await using var legalStream = _structureFileStorage.OpenReadStream(IStructureFileStorage.FILE_NAME_LEGAL);
        await using var footerStream = _structureFileStorage.OpenReadStream(IStructureFileStorage.FILE_NAME_FOOTER);

        var structureFiles = await Task.WhenAll(
            _structureFileStorage.LoadContentAsync(headerStream, cancellationToken),
            _structureFileStorage.LoadContentAsync(introStream, cancellationToken),
            _structureFileStorage.LoadContentAsync(legalStream, cancellationToken),
            _structureFileStorage.LoadContentAsync(footerStream, cancellationToken)
        );

        var postsFiles = await Task.WhenAll(
            _postsFileStorage
                .GetFileNames()
                .Order()
                .Select(async fileName =>
                {
                    await using var stream = _postsFileStorage.OpenReadStream(fileName);
                    return await _postsFileStorage.LoadContentAsync(stream, cancellationToken);
                })
        );

        return new FilesContentContainer(
            structureFiles[0],
            structureFiles[1],
            structureFiles[2],
            structureFiles[3],
            postsFiles
        );
    }

    private static string BuildPageFileName(int pageNumber, int pageCount)
    {
        var lastPageNumber = pageCount - 1;

        return pageNumber == lastPageNumber
            ? "index.html"
            : $"archive-page-{pageNumber}.html";
    }
}