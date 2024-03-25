namespace NanoBlog.Services.Posts;

public interface IPostService
{
    public const int EXCERPT_LENGTH_DEFAULT = 50;

    Task<string?> LoadPostContentByIndexAsync(
        int index,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<PostExcerpt>> LoadExcerptsAsync(
        int excerptLength,
        CancellationToken cancellationToken
    );
}