namespace NanoBlog.Services.Posts;

public class PostService(IConfiguration configuration) : IPostService
{
    public async Task<IEnumerable<PostExcerpt>> LoadExcerptsAsync(
        int excerptLength,
        CancellationToken cancellationToken)
    {
        var fileInfos = configuration
            .GetPostsDirectoryInfo()
            .EnumerateFiles()
            .OrderByDescending(f => f.Name);

        var excerpts = new List<PostExcerpt>();

        foreach (var fileInfo in fileInfos)
        {
            await using var fileStream = fileInfo.OpenRead();

            var excerpt = (await fileStream.LoadAsStringAsync(cancellationToken))
                .NormalizeWhitespaces()
                .Truncate(excerptLength);

            excerpts.Add(new PostExcerpt(fileInfo.Name, excerpt));
        }

        return excerpts;
    }
}