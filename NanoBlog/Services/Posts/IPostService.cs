namespace NanoBlog.Services.Posts;

public interface IPostService
{
    public const int ExcerptLengthDefault = 50;

    Task<IEnumerable<PostExcerpt>> LoadExcerptsAsync(
        int excerptLength,
        CancellationToken cancellationToken);
}