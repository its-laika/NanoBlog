namespace NanoBlog.Services.Posts;

public partial class PostService(
    IStageDirectoryContainer stage
) : IPostService
{
    [GeneratedRegex(@"\s{1,}")]
    private static partial Regex NormalizeWhitespaceRegex();

    public async Task<string?> LoadPostContentByIndexAsync(
        int index,
        CancellationToken cancellationToken
    )
    {
        var fileInfo = GetPostFileInfoByIndex(index);
        if (fileInfo is null)
        {
            return null;
        }

        await using var fileStream = fileInfo.OpenRead();
        return await fileStream.LoadAsStringAsync(cancellationToken);
    }

    public async Task<IEnumerable<PostExcerpt>> LoadExcerptsAsync(
        int excerptLength,
        CancellationToken cancellationToken
    )
    {
        var fileInfos = stage.PostsDirectory
           .EnumerateFiles()
           .OrderByDescending(f => f.Name);

        var excerpts = new List<PostExcerpt>();

        foreach (var fileInfo in fileInfos)
        {
            await using var fileStream = fileInfo.OpenRead();

            var excerpt = await GenerateExcerptAsync(
                fileStream,
                excerptLength,
                cancellationToken
            );

            excerpts.Add(new PostExcerpt(fileInfo.Name, excerpt));
        }

        return excerpts;
    }

    private FileInfo? GetPostFileInfoByIndex(int index)
    {
        if (index >= 0)
        {
            return stage.PostsDirectory
               .EnumerateFiles()
               .OrderBy(f => f.Name)
               .Skip(index)
               .FirstOrDefault();
        }

        /* We must remove the sign and remove one so that (-1) points to
         * the first post (0) of the reversed list. */
        var reverseIndex = index * -1 - 1;

        return stage.PostsDirectory
           .EnumerateFiles()
           .OrderByDescending(f => f.Name)
           .Skip(reverseIndex)
           .FirstOrDefault();
    }

    private static async Task<string> GenerateExcerptAsync(
        FileStream fileStream,
        int length,
        CancellationToken cancellationToken
    )
    {
        var content = await fileStream.LoadAsStringAsync(cancellationToken);

        return NormalizeWhitespaceRegex()
           .Replace(content, " ")
           .Truncate(length);
    }
}