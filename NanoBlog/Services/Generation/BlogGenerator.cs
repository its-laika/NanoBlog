namespace NanoBlog.Services.Generation;

public class BlogGenerator(IConfiguration configuration) : IBlogGenerator
{
    public async Task<IList<MemoryStream>> GeneratePageContentsAsync(CancellationToken cancellationToken)
    {
        var allPosts = await PreparePostsAsync(cancellationToken);

        var pagePosts = configuration.PageSize is { } chunkSize && allPosts.Any()
            ? [.. allPosts.Chunk(chunkSize).ToList()]
            : new List<IEnumerable<string>> { allPosts };

        return pagePosts
            .Select(posts => posts.Reverse().ToList())
            .Select((posts, index) => GeneratePage(posts, index, pagePosts.Count))
            .Select(Encoding.UTF8.GetBytes)
            .Select(bytes => new MemoryStream(bytes))
            .ToList();
    }

    private string GeneratePage(IEnumerable<string> posts, int pageNumber, int pageCount)
    {
        var combinedPosts = posts
            .Where(post => !string.IsNullOrWhiteSpace(post))
            .Aggregate(new StringBuilder(), (carry, post) => carry.Append(post))
            .ToString();

        var navigation = BuildNavigation(pageNumber, pageCount);

        return configuration
            .PageTemplate
            .Replace(configuration.PagePlaceholderPosts, combinedPosts)
            .Replace(configuration.PagePlaceholderNavigation, navigation)
            .NormalizeWhitespaces();
    }

    private async Task<IList<string>> PreparePostsAsync(CancellationToken cancellationToken)
    {
        return await Task.WhenAll(
            configuration.GetPostsDirectoryInfo()
                .EnumerateFiles()
                .OrderBy(f => f.Name)
                .Select(async fileInfo =>
                {
                    string content;
                    await using (var stream = fileInfo.OpenRead())
                    {
                        content = await stream.LoadAsStringAsync(cancellationToken);
                    }

                    if (string.IsNullOrWhiteSpace(content))
                    {
                        return string.Empty;
                    }

                    return configuration
                        .PostTemplate
                        .Replace(configuration.PostPlaceholderName, fileInfo.Name)
                        .Replace(configuration.PostPlaceholderContent, content);
                })
        );
    }

    private static string BuildNavigation(int pageNumber, int pageCount)
    {
        if (pageCount == 1)
        {
            return string.Empty;
        }

        var navigation = new StringBuilder("<nav id='pagination'><ul>");

        if (pageNumber == pageCount - 2)
        {
            navigation.Append(@$"
                <li id='nav-following-page'>
                    <a href='../../'>{pageNumber + 2}</a>
                </li>");
        }
        else if (pageNumber < pageCount - 2)
        {
            navigation.Append(@$"
                <li id='nav-following-page'>
                    <a href='../{pageNumber + 1}'>{pageNumber + 2}</a>
                </li>");
        }

        navigation.Append(@$"
            <li id='nav-current-page'>
                <a href='#'>{pageNumber + 1}</a>
            </li>");

        if (pageNumber == pageCount - 1)
        {
            navigation.Append(@$"
                <li id='nav-previous-page'>
                    <a href='archive/{pageNumber - 1}'>{pageNumber}</a>
                </li>");
        }
        else if (pageNumber > 0)
        {
            navigation.Append(@$"
                <li id='nav-previous-page'>
                    <a href='../{pageNumber - 1}'>{pageNumber}</a>
                </li>");
        }

        navigation.Append("</ul></nav>");

        return navigation.ToString();
    }
}