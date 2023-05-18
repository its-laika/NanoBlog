namespace NanoBlog.Services.FileStorages.Posts;

public interface IPostsFileStorage : IFileStorage
{
    FileStream CreateNewFileWriteStream();
}