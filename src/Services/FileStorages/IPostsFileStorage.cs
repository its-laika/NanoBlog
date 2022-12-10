using NanoBlog.Attributes;

namespace NanoBlog.Services.FileStorages;

public interface IPostsFileStorage : IFileStorage
{
    Task<string> CreatePostFileAsync(Stream content, CancellationToken cancellationToken);

    void Delete([ValidFileName] string fileName);
}