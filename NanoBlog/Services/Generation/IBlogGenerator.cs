namespace NanoBlog.Services.Generation;

public interface IBlogGenerator
{
    Task<IList<MemoryStream>> GeneratePageContentsAsync(CancellationToken cancellationToken);
}