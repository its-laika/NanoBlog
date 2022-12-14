namespace NanoBlog.Services.FileStorages.Posts;

public class PostsFileStorage : AbstractFileStorage, IPostsFileStorage
{
    protected override string BaseFolder { get; }

    public PostsFileStorage(ILogger<AbstractFileStorage> logger) : base(logger)
    {
        BaseFolder = Path.Combine(Directory.GetCurrentDirectory(), "BlogFiles/Posts");
    }
}