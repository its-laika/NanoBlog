namespace NanoBlog.Services.Posts;

public interface IPostService
{
    public const uint EXCERPT_LENGTH_DEFAULT = 50;

    Task<string?> LoadPostContentByIndexAsync(
        int index,
        CancellationToken cancellationToken
    );

    Task<IEnumerable<PostExcerpt>> LoadExcerptsAsync(
        uint excerptLength,
        CancellationToken cancellationToken
    );
}