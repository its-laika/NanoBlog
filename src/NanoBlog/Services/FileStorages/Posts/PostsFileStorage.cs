using NanoBlog.Services.FileSystemSecurity;

namespace NanoBlog.Services.FileStorages.Posts;

public class PostsFileStorage : AbstractFileStorage, IPostsFileStorage
{
    public PostsFileStorage(IFileSystemSecurityService fileSystemSecurityService)
        : base(
            fileSystemSecurityService,
            new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "BlogFiles/Posts"))
        )
    {
    }
}