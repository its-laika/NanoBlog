namespace NanoBlog.Services.FileStorages.Posts;

public class PostsFileStorage : AbstractFileStorage, IPostsFileStorage
{
    private readonly IFileSystemSecurityService _fileSystemSecurityService;

    public PostsFileStorage(
        IFileSystemSecurityService fileSystemSecurityService
    ) : base(
        fileSystemSecurityService,
        Configuration.GetStagePostsDirectoryInfo()
    )
    {
        _fileSystemSecurityService = fileSystemSecurityService;
    }

    public FileStream CreateNewFileWriteStream()
    {
        var fileName = $"{DateTime.UtcNow.Ticks}-{Guid.NewGuid()}.txt";
        var fileInfo = new FileInfo(Path.Combine(BaseDirectory.FullName, fileName));

        var fileStream = fileInfo.Create();
        _fileSystemSecurityService.EnsureSecureMode(fileInfo);

        return fileStream;
    }
}