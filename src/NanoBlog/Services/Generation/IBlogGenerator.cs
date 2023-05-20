namespace NanoBlog.Services.Generation;

public interface IBlogGenerator
{
    Task<GeneratedPageContentsContainer> GeneratePageContentsAsync(CancellationToken cancellationToken);
    Task<string> GeneratePreviewAsync(CancellationToken cancellationToken);
}