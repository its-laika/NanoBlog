namespace NanoBlog.Services.Generation;

public interface IBlogGenerator
{
    Task<GeneratedPagesContainer> GeneratePageContentsAsync(CancellationToken cancellationToken);
    Task<string> GeneratePreviewAsync(CancellationToken cancellationToken);
}