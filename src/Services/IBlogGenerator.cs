namespace NanoBlog.Services;

public interface IBlogGenerator
{
    Task<string> GenerateContentAsync(CancellationToken cancellationToken);
}