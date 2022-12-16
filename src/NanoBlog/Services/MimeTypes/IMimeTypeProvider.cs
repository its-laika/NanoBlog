using NanoBlog.Attributes;

namespace NanoBlog.Services.MimeTypes;

public interface IMimeTypeProvider
{
    Task<MimeType?> ProvideMimeTypeAsync(
        IFormFile formFile,
        Stream content,
        CancellationToken cancellationToken
    );

    Task<MimeType?> ProvideMimeTypeAsync(
        string fileName,
        Stream content,
        CancellationToken cancellationToken
    );
}