namespace NanoBlog.Services.Generation;

public interface IBlogGenerator
{
    Task<IDictionary<string, Stream>> GeneratePageMappingAsync(CancellationToken cancellationToken);
    Task<string> GeneratePreviewAsync(CancellationToken cancellationToken);
}